using MediatR;
using PViMS.Core.Events;
using PVIMS.API.Infrastructure.Services;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.DomainEventHandlers.TaskAdded
{
    public class SendEmailWhenTaskAddedDomainEventHandler
                            : INotificationHandler<TaskAddedDomainEvent>
    {
        private readonly ISMTPMailService _smtpMailService;

        public SendEmailWhenTaskAddedDomainEventHandler(ISMTPMailService smtpMailService)
        {
            _smtpMailService = smtpMailService ?? throw new ArgumentNullException(nameof(smtpMailService));
        }

        public async Task Handle(TaskAddedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var subject = $"Report task: {domainEvent.Task.ReportInstance.Identifier}";

            var sb = new StringBuilder();
            sb.Append($"A new task {TaskType.From(domainEvent.Task.TaskTypeId).Name} has been added that requires your attention. Please note the following details pertaining to the task: ");
            sb.Append("<p><b><u>Adverse Event Details</u></b></p>");
            sb.Append("<table>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Identifier</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Task.ReportInstance.Identifier}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Patient</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Task.ReportInstance.PatientIdentifier}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Created</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Task.ReportInstance.Created}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Description</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Task.ReportInstance.SourceIdentifier}</td></tr>");
            sb.Append("</table>");
            sb.Append("<p><b><u>Task Details</u></b></p>");
            sb.Append("<table>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Source</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Task.TaskDetail.Source}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Description</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Task.TaskDetail.Description}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Status</b></td><td style='padding: 10px; border: 1px solid black;'>{Core.Aggregates.ReportInstanceAggregate.TaskStatus.From(domainEvent.Task.TaskStatusId).Name}</td></tr>");
            sb.Append("</table>");

            await _smtpMailService.SendEmailAsync(subject, sb.ToString(), domainEvent.Task.ReportInstance.CreatedBy.FullName, domainEvent.Task.ReportInstance.CreatedBy.Email );
        }
    }
}
