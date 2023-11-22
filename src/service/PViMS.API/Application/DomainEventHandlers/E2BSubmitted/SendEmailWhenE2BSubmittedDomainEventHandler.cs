﻿using MediatR;
using MimeKit;
using PViMS.Core.Events;
using PVIMS.API.Infrastructure.Services;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.DomainEventHandlers.E2BSubmitted
{
    public class SendEmailWhenE2BSubmittedDomainEventHandler
                            : INotificationHandler<E2BSubmittedDomainEvent>
    {
        private readonly ISMTPMailService _smtpMailService;

        public SendEmailWhenE2BSubmittedDomainEventHandler(ISMTPMailService smtpMailService)
        {
            _smtpMailService = smtpMailService ?? throw new ArgumentNullException(nameof(smtpMailService));
        }

        public async Task Handle(E2BSubmittedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            if (!_smtpMailService.CheckIfEnabled())
            {
                return;
            }

            if (ReportInstanceHasInvalidContactInformation(domainEvent))
            {
                return;
            }

            var subject = $"Report: {domainEvent.ReportInstance.PatientIdentifier}";

            var sb = new StringBuilder();
            sb.Append($"E2B file has been submitted for this report. Please note the following details pertaining to the report: ");
            sb.Append("<p><b><u>Adverse Event Details</u></b></p>");
            sb.Append("<table>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Identifier</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.Identifier}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Patient</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.PatientIdentifier}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Created</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.Created}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Description</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.SourceIdentifier}</td></tr>");
            sb.Append("</table>");
            sb.Append("<p><b><u>Causality and Terminology Details</u></b></p>");
            sb.Append("<table>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Terminology</b></td><td style='padding: 10px; border: 1px solid black;'>{domainEvent.ReportInstance.TerminologyMedDra.DisplayName}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Classification</b></td><td style='padding: 10px; border: 1px solid black;'>{ReportClassification.From(domainEvent.ReportInstance.ReportClassificationId)}</td></tr>");
            sb.Append($"<tr><td style='padding: 10px; border: 1px solid black;'><b>Causality</b></td><td style='padding: 10px; border: 1px solid black;'>Please browse PV feedback for more details</td></tr>");
            sb.Append("</table>");
            sb.Append("<p><b>*** This is system generated. Please do not reply to this message ***</b></p>");

            await _smtpMailService.SendEmailAsync(subject, sb.ToString(), PrepareDestinationMailBoxes(domainEvent), null);
        }

        private bool ReportInstanceHasInvalidContactInformation(E2BSubmittedDomainEvent domainEvent)
        {
            return String.IsNullOrWhiteSpace(domainEvent.ReportInstance.ReporterFullName) || String.IsNullOrWhiteSpace(domainEvent.ReportInstance.ReporterEmail);
        }

        private List<MailboxAddress> PrepareDestinationMailBoxes(E2BSubmittedDomainEvent domainEvent)
        {
            var destinationAddresses = new List<MailboxAddress>
            {
                new MailboxAddress(domainEvent.ReportInstance.CreatedBy.FullName, domainEvent.ReportInstance.CreatedBy.Email),
                new MailboxAddress(domainEvent.ReportInstance.ReporterFullName, domainEvent.ReportInstance.ReporterEmail)
            };
            return destinationAddresses;
        }
    }
}
