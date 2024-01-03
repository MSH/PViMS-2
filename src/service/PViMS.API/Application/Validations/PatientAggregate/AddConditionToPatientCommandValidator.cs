using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.PatientAggregate;
using System;

namespace PVIMS.API.Application.Validations
{
    public class AddConditionToPatientCommandValidator : AbstractValidator<AddConditionToPatientCommand>
    {
        public AddConditionToPatientCommandValidator(ILogger<AddConditionToPatientCommandValidator> logger)
        {
            RuleFor(command => command.Outcome)
                .Length(0, 50)
                .Matches(@"[-a-zA-Z0-9 .]")
                .When(c => !string.IsNullOrEmpty(c.Outcome))
                .WithMessage("Outcome contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period)");

            RuleFor(command => command.TreatmentOutcome)
                .Length(0, 50)
                .Matches(@"[-a-zA-Z0-9 .]")
                .When(c => !string.IsNullOrEmpty(c.TreatmentOutcome))
                .WithMessage("Treatment outcome contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period)");

            RuleFor(command => command.SourceDescription)
                .Length(0, 200)
                .Matches(@"[-a-zA-Z0-9 .,()']")
                .When(c => !string.IsNullOrEmpty(c.SourceDescription))
                .WithMessage("Source description contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period, comma, parentheses, apostrophe)");

            RuleFor(command => command.StartDate)
                .LessThanOrEqualTo(p => DateTime.Now.Date)
                .WithMessage("Start Date should be before current date");

            RuleFor(command => command.OutcomeDate)
                .LessThanOrEqualTo(p => DateTime.Now.Date)
                .WithMessage("Outcome Date should be before current date")
                .When(p => p.OutcomeDate.HasValue)
                .GreaterThanOrEqualTo(p => p.StartDate)
                .WithMessage("Outcome Date should be after Start Date")
                .When(p => p.OutcomeDate.HasValue);

            RuleFor(command => command.Comments)
                .Length(0, 500)
                .When(c => !string.IsNullOrEmpty(c.Comments));

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
