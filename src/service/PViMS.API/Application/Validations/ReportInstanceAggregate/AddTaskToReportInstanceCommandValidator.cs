﻿using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.ReportInstanceAggregate;

namespace PVIMS.API.Application.Validations
{
    public class AddTaskToReportInstanceCommandValidator : AbstractValidator<AddTaskToReportInstanceCommand>
    {
        public AddTaskToReportInstanceCommandValidator(ILogger<AddTaskToReportInstanceCommandValidator> logger)
        {
            RuleFor(command => command.Source)
                .NotEmpty()
                .Length(1, 200)
                .Matches(@"[a-zA-Z0-9 ]")
                .WithMessage("Source contains invalid characters (Enter A-Z, a-z, 0-9, space)");

            RuleFor(command => command.Description)
                .NotEmpty()
                .Length(1, 500)
                .Matches(@"[-a-zA-Z0-9()?,. ]")
                .WithMessage("Description contains invalid characters (Enter A-Z, a-z, 0-9, space, parenthesis, question mark, comma, period)");

            RuleFor(command => command.Description).NotEmpty().Length(1, 500);

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
