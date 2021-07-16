using MediatR;
using PViMS.Core.Events;
using PVIMS.API.Infrastructure.Services;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.DomainEventHandlers.TaskAdded
{
    public class SendEmailWhenCausalityConfirmedDomainEventHandler
                            : INotificationHandler<CausalityConfirmedDomainEvent>
    {
        private readonly ISMTPMailService _smtpMailService;

        public SendEmailWhenCausalityConfirmedDomainEventHandler(ISMTPMailService smtpMailService)
        {
            _smtpMailService = smtpMailService ?? throw new ArgumentNullException(nameof(smtpMailService));
        }

        public async Task Handle(CausalityConfirmedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var subject = $"Report task: {domainEvent.ReportInstance.Identifier}";

            var sb = new StringBuilder();
            sb.Append($"Causality and terminology has been set for this report. Please note the following details pertaining to the report: ");
            sb.Append("<p><b><u>Adverse Event Details</u></b></p>");
            sb.Append("<table>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Identifier</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.Identifier}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Patient</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.PatientIdentifier}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Created</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.Created}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Description</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.SourceIdentifier}</td></tr>");
            sb.Append("</table>");

            await _smtpMailService.SendEmailAsync(subject, sb.ToString(), domainEvent.ReportInstance.CreatedBy.FullName, domainEvent.ReportInstance.CreatedBy.Email );
        }
    }
}
