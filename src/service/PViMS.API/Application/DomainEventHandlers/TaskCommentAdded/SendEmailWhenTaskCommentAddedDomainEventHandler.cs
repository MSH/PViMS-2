using MediatR;
using PViMS.Core.Events;
using PVIMS.API.Infrastructure.Services;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.DomainEventHandlers.TaskAdded
{
    public class SendEmailWhenTaskCommentAddedDomainEventHandler
                            : INotificationHandler<TaskCommentAddedDomainEvent>
    {
        private readonly ISMTPMailService _smtpMailService;

        public SendEmailWhenTaskCommentAddedDomainEventHandler(ISMTPMailService smtpMailService)
        {
            _smtpMailService = smtpMailService ?? throw new ArgumentNullException(nameof(smtpMailService));
        }

        public async Task Handle(TaskCommentAddedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var subject = $"Report task: {domainEvent.Comment.ReportInstanceTask.ReportInstance.Identifier}";

            var sb = new StringBuilder();
            sb.Append($"A new comment has been added for task {domainEvent.Comment.ReportInstanceTask.ReportInstance.Identifier}: ");
            sb.Append("<p><b><u>Comment</u></b></p>");
            sb.Append("<table>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Comment</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Comment.Comment}</td></tr>");
            sb.Append("</table>");
            sb.Append("<p><b><u>Adverse Event Details</u></b></p>");
            sb.Append("<table>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Identifier</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Comment.ReportInstanceTask.ReportInstance.Identifier}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Patient</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Comment.ReportInstanceTask.ReportInstance.PatientIdentifier}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Created</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Comment.ReportInstanceTask.ReportInstance.Created}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Description</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Comment.ReportInstanceTask.ReportInstance.SourceIdentifier}</td></tr>");
            sb.Append("</table>");
            sb.Append("<p><b><u>Task Details</u></b></p>");
            sb.Append("<table>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Source</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Comment.ReportInstanceTask.TaskDetail.Source}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Description</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.Comment.ReportInstanceTask.TaskDetail.Description}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Status</b></td><td style='padding: 10px; border: 1px solid black;'>{Core.Aggregates.ReportInstanceAggregate.TaskStatus.From(domainEvent.Comment.ReportInstanceTask.TaskStatusId).Name}</td></tr>");
            sb.Append("</table>");

            await _smtpMailService.SendEmailAsync(subject, sb.ToString(), domainEvent.Comment.ReportInstanceTask.ReportInstance.CreatedBy.FullName, domainEvent.Comment.ReportInstanceTask.ReportInstance.CreatedBy.Email );
        }
    }
}
