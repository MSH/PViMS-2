using MediatR;
using PViMS.Core.Events;
using PVIMS.Core.Aggregates.NotificationAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.ValueTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.DomainEventHandlers.TaskAdded
{
    public class AddNotificationWhenTaskCommentAddedDomainEventHandler
                            : INotificationHandler<TaskCommentAddedDomainEvent>
    {
        private readonly IRepositoryInt<Notification> _notificationRepository;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IUnitOfWorkInt _unitOfWork;

        public AddNotificationWhenTaskCommentAddedDomainEventHandler(
            IRepositoryInt<Notification> notificationRepository,
            IRepositoryInt<Config> configRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(TaskCommentAddedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var config = await _configRepository.GetAsync(c => c.ConfigType == ConfigType.ReportInstanceNewAlertCount);
            var alertCount = 1;
            if (config != null)
            {
                alertCount = Convert.ToInt32(config.ConfigValue);
            }

            var summary = $"A new comment has been added to a task for report {domainEvent.Comment.ReportInstanceTask.ReportInstance.Identifier}";
            var detail = domainEvent.Comment.Comment.Length > 100 ? $"{domainEvent.Comment.Comment.Substring(0, 100)} ..." : domainEvent.Comment.Comment;

            var notificationType = NotificationType.FromName("Informational");
            var notificationClassification = NotificationClassification.FromName("NewTaskComment");
            var contextRoute = $"{domainEvent.Comment.ReportInstanceTask.ReportInstance.WorkFlow.WorkFlowGuid}/{domainEvent.Comment.ReportInstanceTask.ReportInstance.Id}";
            var newNotification = new Notification(domainEvent.Comment.ReportInstanceTask.ReportInstance.CreatedBy, DateTime.Now.AddDays(alertCount), summary, detail, notificationType, notificationClassification, contextRoute);

            await _notificationRepository.SaveAsync(newNotification);
            await _unitOfWork.CompleteAsync();
        }
    }
}
