using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Queries.ReportInstanceAggregate;

namespace PVIMS.API.Application.Validations
{
    public class ReportInstancesAnalysisQueryValidator : AbstractValidator<ReportInstancesAnalysisQuery>
    {
        public ReportInstancesAnalysisQueryValidator(ILogger<ReportInstancesAnalysisQueryValidator> logger)
        {
            RuleFor(command => command.QualifiedName)
                .Length(0, 50);

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
