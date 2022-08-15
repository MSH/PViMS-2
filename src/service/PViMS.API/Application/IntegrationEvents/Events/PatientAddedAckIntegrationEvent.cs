﻿using PViMS.BuildingBlocks.EventBus.Events;
using System;
using System.Text.Json.Serialization;

namespace PVIMS.API.Application.IntegrationEvents.Events
{
    public record PatientAddedAckIntegrationEvent : IntegrationEvent
    {
        [JsonInclude]
        public bool Processed { get; private init; }

        public PatientAddedAckIntegrationEvent(Guid acknowledgeId, bool processed): base(acknowledgeId)
        {
            Processed = processed;
        }
    }
}