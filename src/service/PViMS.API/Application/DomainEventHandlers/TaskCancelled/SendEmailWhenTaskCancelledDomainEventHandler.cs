using MediatR;
using MimeKit;
using PViMS.Core.Events;
using PVIMS.API.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.DomainEventHandlers.TaskCancelled
{
    public class SendEmailWhenTaskCancelledDomainEventHandler
                            : INotificationHandler<TaskCancelledDomainEvent>
    {
        private readonly ISMTPMailService _smtpMailService;

        public SendEmailWhenTaskCancelledDomainEventHandler(ISMTPMailService smtpMailService)
        {
            _smtpMailService = smtpMailService ?? throw new ArgumentNullException(nameof(smtpMailService));
        }

        public async Task Handle(TaskCancelledDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            if (!_smtpMailService.CheckIfEnabled())
            {
                return;
            }

            if (ReportInstanceHasInvalidContactInformation(domainEvent))
            {
                return;
            }

            var subject = $"Report task: {domainEvent.Task.ReportInstance.Identifier}";

            var sb = new StringBuilder();
            sb.Append($"Please note task {domainEvent.Task.ReportInstance.Identifier} has been cancelled: ");
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
            sb.Append("<p><b>*** This is system generated. Please do not reply to this message ***</b></p>");

            await _smtpMailService.SendEmailAsync(subject, sb.ToString(), PrepareDestinationMailBoxes(domainEvent), null);
        }

        private bool ReportInstanceHasInvalidContactInformation(TaskCancelledDomainEvent domainEvent)
        {
            return String.IsNullOrWhiteSpace(domainEvent.Task.ReportInstance.ReporterFullName) || String.IsNullOrWhiteSpace(domainEvent.Task.ReportInstance.ReporterEmail);
        }

        private List<MailboxAddress> PrepareDestinationMailBoxes(TaskCancelledDomainEvent domainEvent)
        {
            var destinationAddresses = new List<MailboxAddress>
            {
                new MailboxAddress(domainEvent.Task.ReportInstance.CreatedBy.FullName, domainEvent.Task.ReportInstance.CreatedBy.Email),
                new MailboxAddress(domainEvent.Task.ReportInstance.ReporterFullName, domainEvent.Task.ReportInstance.ReporterEmail)
            };
            return destinationAddresses;
        }
    }
}
