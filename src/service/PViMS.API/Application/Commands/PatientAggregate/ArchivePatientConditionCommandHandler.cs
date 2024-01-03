using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    public class ArchivePatientConditionCommandHandler
        : IRequestHandler<ArchivePatientConditionCommand, bool>
    {
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ArchivePatientConditionCommandHandler> _logger;

        public ArchivePatientConditionCommandHandler(
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<User> userRepository,
            IUnitOfWorkInt unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ArchivePatientConditionCommandHandler> logger)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ArchivePatientConditionCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId, new string[] { "PatientConditions" });
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromRepo = await _userRepository.GetAsync(u => u.UserName == userName);
            if (userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }

            patientFromRepo.ArchiveCondition(message.PatientConditionId, message.Reason, userFromRepo);
            _patientRepository.Update(patientFromRepo);

            _logger.LogInformation($"----- Condition {message.PatientConditionId} archived");

            return await _unitOfWork.CompleteAsync();
        }
    }
}
