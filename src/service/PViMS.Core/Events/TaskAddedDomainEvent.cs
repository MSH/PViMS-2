﻿using MediatR;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;

namespace PViMS.Core.Events
{
    /// <summary>
    /// Event used when a new report instance task is added
    /// </summary>
    public class TaskAddedDomainEvent : INotification
    {
        public ReportInstanceTask Task { get; }

        public TaskAddedDomainEvent(ReportInstanceTask task)
        {
            Task = task;
        }
    }
}
