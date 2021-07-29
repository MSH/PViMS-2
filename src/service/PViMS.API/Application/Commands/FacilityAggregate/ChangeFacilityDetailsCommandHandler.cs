using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Entities;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.FacilityAggregate
{
    public class ChangeFacilityDetailsCommandHandler
        : IRequestHandler<ChangeFacilityDetailsCommand, bool>
    {
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<FacilityType> _facilityTypeRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ChangeFacilityDetailsCommandHandler> _logger;

        public ChangeFacilityDetailsCommandHandler(
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<FacilityType> facilityTypeRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ChangeFacilityDetailsCommandHandler> logger)
        {
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _facilityTypeRepository = facilityTypeRepository ?? throw new ArgumentNullException(nameof(facilityTypeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeFacilityDetailsCommand message, CancellationToken cancellationToken)
        {
            var facilityFromRepo = await _facilityRepository.GetAsync(f => f.Id == message.Id);
            if (facilityFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate facility");
            }

            var facilityTypeFromRepo = await _facilityTypeRepository.GetAsync(c => c.Description == message.FacilityType);
            if (facilityTypeFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate facility type");
            }

            if (_facilityRepository.Exists(l => (l.FacilityName == message.FacilityName || l.FacilityCode == message.FacilityCode) && l.Id != message.Id))
            {
                throw new DomainException("Item with same name already exists");
            }

            facilityFromRepo.ChangeDetails(message.FacilityName, message.FacilityCode, facilityTypeFromRepo, message.TelNumber, message.MobileNumber, message.FaxNumber);
            _facilityRepository.Update(facilityFromRepo);

            _logger.LogInformation($"----- Facility {message.FacilityName} details updated");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
