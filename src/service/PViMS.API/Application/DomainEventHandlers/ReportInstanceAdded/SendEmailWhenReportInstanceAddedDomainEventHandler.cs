using MediatR;
using MimeKit;
using PViMS.Core.Events;
using PVIMS.API.Infrastructure.Services;
using PVIMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.DomainEventHandlers.ReportInstanceAdded
{
    public class SendEmailWhenReportInstanceAddedDomainEventHandler
                            : INotificationHandler<ReportInstanceAddedDomainEvent>
    {
        private readonly ISMTPMailService _smtpMailService;
        private readonly IArtefactService _artefactService;

        public SendEmailWhenReportInstanceAddedDomainEventHandler(ISMTPMailService smtpMailService,
            IArtefactService artefactService)
        {
            _smtpMailService = smtpMailService ?? throw new ArgumentNullException(nameof(smtpMailService));
            _artefactService = artefactService ?? throw new ArgumentNullException(nameof(artefactService));
        }

        public async Task Handle(ReportInstanceAddedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            if (!_smtpMailService.CheckIfEnabled())
            {
                return;
            }

            if (ReportInstanceHasInvalidContactInformation(domainEvent))
            {
                return;
            }

            var attachments = new List<ArtefactInfoModel>();
            var artefactModel = domainEvent.ReportInstance.WorkFlow.Description == "New Active Surveilliance Report" ?
                await _artefactService.CreatePatientSummaryForActiveReportAsync(domainEvent.ReportInstance.ContextGuid) :
                await _artefactService.CreatePatientSummaryForSpontaneousReportAsync(domainEvent.ReportInstance.ContextGuid);
            attachments.Add(artefactModel);

            var subject = $"New Report Added: {domainEvent.ReportInstance.Identifier}";

            var sb = new StringBuilder();
            sb.Append($"This email acknowledges that a new adverse event has been reported and received. Please find attached a copy of the report for your records.");
            sb.Append("<p><b><u>Adverse Event Details</u></b></p>");
            sb.Append("<table>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Identifier</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.Identifier}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Patient</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.PatientIdentifier}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Created</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.Created}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Description</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.SourceIdentifier}</td></tr>");
            sb.Append("</table>");
            sb.Append("<p><b>*** This email is system generated. Please do not reply to this message ***</b></p>");


            await _smtpMailService.SendEmailAsync(subject, sb.ToString(), PrepareDestinationMailBoxes(domainEvent), attachments);
        }

        private bool ReportInstanceHasInvalidContactInformation(ReportInstanceAddedDomainEvent domainEvent)
        {
            return String.IsNullOrWhiteSpace(domainEvent.ReportInstance.ReporterFullName) || String.IsNullOrWhiteSpace(domainEvent.ReportInstance.ReporterEmail);
        }

        private List<MailboxAddress> PrepareDestinationMailBoxes(ReportInstanceAddedDomainEvent domainEvent)
        {
            var destinationAddresses = new List<MailboxAddress>
            {
                new MailboxAddress(domainEvent.ReportInstance.ReporterFullName, domainEvent.ReportInstance.ReporterEmail)
            };
            return destinationAddresses;
        }
    }
}