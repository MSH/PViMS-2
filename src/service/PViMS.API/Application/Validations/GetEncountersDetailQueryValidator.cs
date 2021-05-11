using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Queries.EncounterAggregate;

namespace PVIMS.API.Application.Validations
{
    public class GetEncountersDetailQueryValidator : AbstractValidator<GetEncountersDetailQuery>
    {
        public GetEncountersDetailQueryValidator(ILogger<GetEncountersDetailQueryValidator> logger)
        {
            RuleFor(command => command.FirstName).Matches(@"[-a-zA-Z ']").When(v => !string.IsNullOrEmpty(v.FirstName));
            RuleFor(command => command.LastName).Matches(@"[-a-zA-Z ']").When(v => !string.IsNullOrEmpty(v.LastName));
            RuleFor(command => command.CustomAttributeValue).Matches(@"[-a-zA-Z0-9 ']").When(v => !string.IsNullOrEmpty(v.CustomAttributeValue));

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
