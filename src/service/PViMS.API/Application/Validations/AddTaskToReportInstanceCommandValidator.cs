using FluentValidation;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.ReportInstanceAggregate;

namespace PVIMS.API.Application.Validations
{
    public class AddTaskToReportInstanceCommandValidator : AbstractValidator<AddTaskToReportInstanceCommand>
    {
        public AddTaskToReportInstanceCommandValidator(ILogger<AddTaskToReportInstanceCommandValidator> logger)
        {
            RuleFor(command => command.Source).NotEmpty().Length(1, 200);
            RuleFor(command => command.Description).NotEmpty().Length(1, 500);

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
