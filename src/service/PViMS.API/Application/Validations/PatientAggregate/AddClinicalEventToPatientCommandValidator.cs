﻿using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.PatientAggregate;
using System;

namespace PVIMS.API.Application.Validations
{
    public class AddClinicalEventToPatientCommandValidator : AbstractValidator<AddClinicalEventToPatientCommand>
    {
        public AddClinicalEventToPatientCommandValidator(ILogger<AddClinicalEventToPatientCommandValidator> logger)
        {
            RuleFor(command => command.PatientIdentifier)
                .Length(0, 50)
                .Matches(@"[-a-zA-Z0-9()']")
                .When(c => !string.IsNullOrEmpty(c.PatientIdentifier))
                .WithMessage("Patient identifier contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, parentheses, apostrophe)");

            RuleFor(command => command.SourceDescription)
                .Length(0, 500)
                .Matches(@"[-a-zA-Z0-9 .,()']")
                .When(c => !string.IsNullOrEmpty(c.SourceDescription))
                .WithMessage("Source description contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period, comma, parentheses, apostrophe)");

            RuleFor(command => command.OnsetDate)
                .LessThanOrEqualTo(p => DateTime.Now.Date)
                .WithMessage("Onset Date should be before current date")
                .GreaterThanOrEqualTo(p => DateTime.Now.Date.AddYears(-10))
                .WithMessage("Onset Date should be within the past 10 years");

            RuleFor(command => command.ResolutionDate)
                .LessThanOrEqualTo(p => DateTime.Now.Date)
                .WithMessage("Resolution Date should be before current date")
                .When(p => p.ResolutionDate.HasValue)
                .GreaterThanOrEqualTo(p => p.OnsetDate)
                .WithMessage("Resolution Date should be after Onset Date")
                .When(p => p.ResolutionDate.HasValue);

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
