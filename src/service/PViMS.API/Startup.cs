using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.Owin.Security.DataProtection;
using Newtonsoft.Json.Serialization;
using PVIMS.API.OperationFilters;
using PVIMS.API.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerUI;
using VPS.Common.Repositories;
using PVIMS.Services;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.API.Auth;
using PVIMS.API.Settings;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;
using PVIMS.Infrastructure.Shared.Repositories;
using PVIMS.Entities.EF.Repositories;
using PVIMS.API.Helpers;
using PVIMS.Core;
using PVIMS.Entities.EF;
using Newtonsoft.Json;
using Microsoft.Extensions.FileProviders;
using PVIMS.API.Configs.ExceptionHandler;

namespace PViMS.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        private IConfigurationSection ConfigAuthSettings { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors(c =>
            {
                c.AddPolicy("AllowSpecificOrigin", options => options.AllowAnyOrigin().
                        AllowAnyHeader()
                        .AllowAnyMethod());
                //c.AddPolicy("AllowSpecificOrigin", options => options.WithOrigins("http://localhost:4200",
                //                                                                  "https://pvims-test.pharmadexmz.org",
                //                                                                  "https://pvims.pharmadexmz.org",
                //                                                                  "https://pvimstest.msh.org")
                //                                                     .AllowAnyHeader()
                //                                                     .AllowAnyMethod());
            });

            services.AddMvc(setupAction =>
            {
                setupAction.EnableEndpointRouting = false; // Use the routing logic of ASP.NET Core 2.1 or earlier:
                setupAction.RespectBrowserAcceptHeader = true;

                setupAction.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                setupAction.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
                setupAction.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status403Forbidden));
                setupAction.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                setupAction.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status422UnprocessableEntity));
                setupAction.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));

                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.OutputFormatters.Add(
                    new XmlDataContractSerializerOutputFormatter());

                // Cater for custom json output media types
                var jsonOutputFormatter = setupAction.OutputFormatters
                    .OfType<JsonOutputFormatter>().FirstOrDefault();
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
                var xmlOutputFormatter = setupAction.OutputFormatters
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
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                //support application/xml
                .AddXmlDataContractSerializerFormatters()
                //support application/json
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            var connectionString = Configuration.GetConnectionString("PVIMS");

            // Register the ConfigurationBuilder instance of AuthSettings
            ConfigAuthSettings = Configuration.GetSection(nameof(AuthSettings));
            services.Configure<AuthSettings>(ConfigAuthSettings);

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(ConfigAuthSettings[nameof(AuthSettings.SecretKey)]));

            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
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

            // Auth & JWT config
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

            services.AddScoped<ICustomAttributeConfigRepository, CustomAttributeConfigRepository>();
            services.AddScoped<ISelectionDataRepository, SelectionDataRepository>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

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
            services.AddTransient<IJwtTokenHandler, JwtTokenHandler>();
            services.AddTransient<ITokenFactory, TokenFactory>();
            services.AddTransient<IJwtFactory, JwtFactory>();

            services.AddAutoMapper(typeof(Startup));

            services.AddHttpContextAccessor();
            services.AddScoped<UserContext, UserContext>();

            services.AddScoped<FormHandler, FormHandler>();

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

            // Create the container builder.
            var builder = new ContainerBuilder();

            builder.RegisterType<TypeExtensionHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<UserInfoStore>()
                .AsImplementedInterfaces();

            builder.Register(c => new IdentityFactoryOptions<UserInfoManager>
            {
                DataProtectionProvider = new DpapiDataProtectionProvider("PVIMS")
            });

            builder.RegisterType<UserInfoManager>();

            builder.RegisterType<PVIMSDbContext>()
              .AsSelf()
              .InstancePerLifetimeScope()
              .WithParameter("connectionString", Configuration["connectionStrings:PVIMS"]);

            // Register dependencies, populate the services from
            // the collection, and build the container. If you want
            // to dispose of the container at the end of the app,
            // be sure to keep a reference to it as a property or field.
            //builder.RegisterAssemblyTypes(Assembly.Load("VPS.NISSA.Repositories.EntityFramework"))
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
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
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
                RequestPath = "/StaticFiles"
            });

            //app.UseHttpsRedirection();

            app.UseCors("AllowSpecificOrigin");

            app.UseSwagger();

            app.UseSwaggerUI(setupAction =>
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

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
