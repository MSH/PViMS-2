using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.FacilityAggregate
{
    public class AddFacilityCommandHandler
        : IRequestHandler<AddFacilityCommand, FacilityDetailDto>
    {
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<FacilityType> _facilityTypeRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddFacilityCommandHandler> _logger;

        public AddFacilityCommandHandler(
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<FacilityType> facilityTypeRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<AddFacilityCommandHandler> logger)
        {
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _facilityTypeRepository = facilityTypeRepository ?? throw new ArgumentNullException(nameof(facilityTypeRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<FacilityDetailDto> Handle(AddFacilityCommand message, CancellationToken cancellationToken)
        {
            var facilityTypeFromRepo = await _facilityTypeRepository.GetAsync(c => c.Description == message.FacilityType);
            if (facilityTypeFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate facility type");
            }

            if (_facilityRepository.Exists(f => f.FacilityName == message.FacilityName ||
                f.FacilityCode == message.FacilityCode))
            {
                throw new DomainException("Facility with same name or code already exists");
            }

            var newFacility = new Facility(message.FacilityName, message.FacilityCode, facilityTypeFromRepo, message.TelNumber, message.MobileNumber, message.FaxNumber);

            await _facilityRepository.SaveAsync(newFacility);

            _logger.LogInformation($"----- Facility {message.FacilityName} created");

            var mappedFacility = _mapper.Map<FacilityDetailDto>(newFacility);

            return CreateLinks(mappedFacility);
        }

        private FacilityDetailDto CreateLinks(FacilityDetailDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Facility", dto.Id), "self", "GET"));

            return dto;
        }
    }
}
