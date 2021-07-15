﻿using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.ReportInstanceAggregate
{
    public class ChangeReportClassificationCommandHandler
        : IRequestHandler<ChangeReportClassificationCommand, bool>
    {
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeReportClassificationCommandHandler> _logger;

        public ChangeReportClassificationCommandHandler(
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeReportClassificationCommandHandler> logger)
        {
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeReportClassificationCommand message, CancellationToken cancellationToken)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.WorkFlow.WorkFlowGuid == message.WorkFlowGuid
                    && ri.Id == message.ReportInstanceId,
                    new string[] { "" });

            if(reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate report instance");
            }

            reportInstanceFromRepo.ChangeClassification(message.ReportClassification);

            _reportInstanceRepository.Update(reportInstanceFromRepo);

            _logger.LogInformation($"----- Report {reportInstanceFromRepo.Id} classification updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
