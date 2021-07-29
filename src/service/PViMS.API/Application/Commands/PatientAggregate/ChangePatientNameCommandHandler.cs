using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    public class ChangePatientNameCommandHandler
        : IRequestHandler<ChangePatientNameCommand, bool>
    {
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangePatientNameCommandHandler> _logger;

        public ChangePatientNameCommandHandler(
            IRepositoryInt<Patient> patientRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangePatientNameCommandHandler> logger)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangePatientNameCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId);
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            patientFromRepo.ChangePatientName(message.FirstName, message.MiddleName, message.LastName);
            _patientRepository.Update(patientFromRepo);

            _logger.LogInformation($"----- Patient {message.PatientId} name details updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
