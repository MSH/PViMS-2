﻿using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.ConceptAggregate;

namespace PVIMS.API.Application.Validations
{
    public class ChangeConceptDetailsCommandValidator : AbstractValidator<ChangeConceptDetailsCommand>
    {
        public ChangeConceptDetailsCommandValidator(ILogger<ChangeConceptDetailsCommandValidator> logger)
        {
            RuleFor(command => command.ConceptName)
                .NotEmpty()
                .Length(1, 1000)
                .Matches(@"[a-zA-Z0-9 .,()%/]")
                .WithMessage("Concept name contains invalid characters (Enter A-Z, a-z, 0-9, space, period, comma, brackets, percentage, forward slash)");

            RuleFor(command => command.Strength)
                .NotEmpty()
                .Length(1, 250)
                .Matches(@"[-a-zA-Z0-9 ,()/]")
                .WithMessage("Strength contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, comma, brackets, forward slash)");

            RuleFor(command => command.MedicationForm)
                .NotEmpty()
                .Length(1, 50)
                .Matches(@"[-a-zA-Z0-9 ,]")
                .WithMessage("Medication form contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, comma)");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
