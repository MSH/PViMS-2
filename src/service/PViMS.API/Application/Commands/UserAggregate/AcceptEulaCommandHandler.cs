﻿using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.UserAggregate
{
    public class AcceptEulaCommandHandler
        : IRequestHandler<AcceptEulaCommand, bool>
    {
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<AcceptEulaCommandHandler> _logger;

        public AcceptEulaCommandHandler(
            IRepositoryInt<User> userRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<AcceptEulaCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(AcceptEulaCommand message, CancellationToken cancellationToken)
        {
            var userFromRepo = await _userRepository.GetAsync(u => u.Id == message.UserId,
                    new string[] { "" });

            if(userFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate user");
            }

            userFromRepo.AcceptEula();
            _userRepository.Update(userFromRepo);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- User {userFromRepo.Id} EULA accepted");

            return true;
        }
    }
}
