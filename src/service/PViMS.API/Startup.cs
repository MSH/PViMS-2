using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PVIMS.API;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.OperationFilters;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Infrastructure.Settings;
using PVIMS.API.Helpers;
using PVIMS.Core;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Infrastructure;
using PVIMS.Infrastructure.Identity;
using PViMS.Infrastructure.Identity.Entities;
using PVIMS.Infrastructure.Repositories;
using PVIMS.Services;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PViMS.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddCustomMvc()
                .AddCustomDbContext(Configuration)
                .AddCustomSwagger(Configuration)
                .AddCustomIntegrations(Configuration)
                .AddCustomConfiguration(Configuration)
                .AddCustomAuthentication(Configuration)
                .AddDependencies(Configuration);

            // Create the container builder.
            var builder = new ContainerBuilder();

            builder.RegisterType<TypeExtensionHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // Register dependencies, populate the services from
            // the collection, and build the container. If you want
            // to dispose of the container at the end of the app,
            // be sure to keep a reference to it as a property or field.

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<EntityFrameworkUnitOfWork>()
                .AsImplementedInterfaces()
                .As<EntityFrameworkUnitOfWork>() // for internal factories.
                .InstancePerLifetimeScope()
                .OnActivating(u => u.Instance.Start());

            builder.RegisterGeneric(typeof(EntityFrameworkRepository<>)).As(typeof(IRepositoryInt<>));

            builder.Populate(services);
            this.ApplicationContainer = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(this.ApplicationContainer);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("An unexpected fault has occurred. Please try again later.");
                });
            });

            //app.UseMiddleware<ExceptionMiddleware>();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
                RequestPath = "/StaticFiles"
            });

            app.UseSwagger()
                .UseSwaggerUI(setupAction =>
                {
                    setupAction.SwaggerEndpoint(
                        "/swagger/PVIMSOpenAPISpecification/swagger.json",
                        "PVIMS Open API UI");

                    setupAction.RoutePrefix = "";
                    setupAction.DefaultModelExpandDepth(2);
                    setupAction.DefaultModelRendering(ModelRendering.Model);
                    setupAction.DocExpansion(DocExpansion.None);
                    setupAction.EnableDeepLinking();
                    setupAction.DisplayOperationId();

                });

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });
        }
    }

    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                //setupAction.EnableEndpointRouting = false; // Use the routing logic of ASP.NET Core 2.1 or earlier:
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;

                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status403Forbidden));
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status422UnprocessableEntity));
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));

                options.OutputFormatters.Add(
                    new XmlDataContractSerializerOutputFormatter());

                // Cater for custom json output media types
                var jsonOutputFormatter = options.OutputFormatters
                    .OfType<SystemTextJsonOutputFormatter>().FirstOrDefault();
                if (jsonOutputFormatter != null)
                {
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.identifier.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.detail.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.expanded.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.search.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.groupvalue.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.commonmeddra.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.newreports.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.feedback.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.patientsummary.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.dataset.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.spontaneousdataset.v1+json");

                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.outstandingvisitreport.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.adverseventreport.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.quarterlyadverseventreport.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.annualadverseventreport.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.patienttreatmentreport.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.patientmedicationreport.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.causalityreport.v1+json");

                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.activitystatusconfirm.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.analyserpatientset.v1+json");
                }

                // Cater for custom XML output media types
                var xmlOutputFormatter = options.OutputFormatters
                    .OfType<XmlDataContractSerializerOutputFormatter>().FirstOrDefault();
                if (xmlOutputFormatter != null)
                {
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.identifier.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.detail.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.expanded.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.search.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.groupvalue.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.commonmeddra.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.newreports.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.feedback.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.dataset.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.spontaneousdataset.v1+xml");

                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.outstandingvisitreport.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.adverseventreport.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.quarterlyadverseventreport.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.annualadverseventreport.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.patienttreatmentreport.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.patientmedicationreport.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.causalityreport.v1+xml");

                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.activitystatusconfirm.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.analyserpatientset.v1+xml");

                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.auditlog.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.pvims.attachment.v1+xml");
                }
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddXmlDataContractSerializerFormatters()
            ;

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEntityFrameworkSqlServer()
                   .AddDbContext<PVIMSDbContext>(options =>
                   {
                       options.UseSqlServer(configuration["ConnectionString"],
                           sqlServerOptionsAction: sqlOptions =>
                           {
                               sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                               sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                           });
                   },
                       ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                   )
                   .AddDbContext<IdentityDbContext>(options =>
                   {
                       options.UseSqlServer(configuration["ConnectionString"],
                           sqlServerOptionsAction: sqlOptions =>
                           {
                               sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                               sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                           });
                   },
                       ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                   );

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc(
                    "PVIMSOpenAPISpecification",
                    new OpenApiInfo()
                    {
                        Title = "PVIMS Open API",
                        Version = "1",
                        Description = "PVIMS Open API Layer",
                        Contact = new OpenApiContact()
                        {
                            Email = "shaun.krog@columbussa.co.za",
                            Name = "Shaun Krog"
                        }
                    });

                setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "basic",
                });

                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" }
                        }, new List<string>() }
                });

                setupAction.OperationFilter<GetAppointmentOperationFilter>();

                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

                setupAction.IncludeXmlComments(xmlCommentsFullPath);
            });

            return services;
        }

        public static IServiceCollection AddCustomIntegrations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddAutoMapper(typeof(Startup));

            return services;
        }

        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<PVIMSSettings>(configuration);
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var actionExecutingContext =
                        actionContext as ActionExecutingContext;

                    // if there are modelstate errors & all keys were correctly
                    // found/parsed we're dealing with validation errors
                    if (actionContext.ModelState.ErrorCount > 0
                        && actionExecutingContext?.ActionArguments.Count == actionContext.ActionDescriptor.Parameters.Count)
                    {
                        return new UnprocessableEntityObjectResult(actionContext.ModelState);
                    }

                    // if one of the keys wasn't correctly found / couldn't be parsed
                    // we're dealing with null/unparsable input
                    return new BadRequestObjectResult(actionContext.ModelState);
                };
            });

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection configAuthSettings = configuration.GetSection(nameof(AuthSettings));
            services.Configure<AuthSettings>(configAuthSettings);

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configAuthSettings[nameof(AuthSettings.SecretKey)]));

            var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512Signature);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(configureOptions =>
                {
                    configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                    configureOptions.TokenValidationParameters = tokenValidationParameters;
                    configureOptions.SaveToken = true;

                    configureOptions.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Access-Control-Expose-Headers", "X-Token-Expired");
                                context.Response.Headers.Add("X-Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtConstants.Strings.JwtClaims.ApiAccess, policy =>
                    policy.RequireClaim(JwtConstants.Strings.JwtClaimIdentifiers.Rol, JwtConstants.Strings.JwtClaims.ApiAccess));
            });

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(o =>
            {
                // configure identity options
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IJwtTokenHandler, JwtTokenHandler>();
            services.AddTransient<ITokenFactory, TokenFactory>();
            services.AddTransient<IJwtFactory, JwtFactory>();

            return services;
        }

        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<ICustomAttributeConfigRepository, CustomAttributeConfigRepository>();
            services.AddScoped<ISelectionDataRepository, SelectionDataRepository>();

            services.AddScoped<UserContext, UserContext>();
            services.AddScoped<FormHandler, FormHandler>();

            services.AddScoped<IUrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<ITypeHelperService, TypeHelperService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IInfrastructureService, InfrastructureService>();
            services.AddTransient<ICustomAttributeService, CustomAttributeService>();
            services.AddTransient<IPatientService, PatientService>();
            services.AddTransient<IArtefactService, ArtefactService>();
            services.AddTransient<IWorkFlowService, WorkFlowService>();
            services.AddTransient<IMedDraService, MedDraService>();

            return services;
        }
    }
}
