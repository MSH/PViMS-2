using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using PVIMS.Infrastructure;
using System;
using System.Threading.Tasks;

namespace PVIMS.API.Infrastructure
{
    public class PVIMSContextSeed
    {
        public async Task SeedAsync(PVIMSDbContext context, IWebHostEnvironment env, IOptions<PVIMSSettings> settings, ILogger<PVIMSContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(PVIMSContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var contentRootPath = env.ContentRootPath;

                using (context)
                {
                    context.Database.Migrate();

                    //context.ConceptElements.AddRange(GetPredefinedConceptElementsForExposure(context));

                    await context.SaveEntitiesAsync();
                }
            });
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<PVIMSContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}
