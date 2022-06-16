﻿using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientLabTestIdentifierQuery
        : IRequest<PatientLabTestIdentifierDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientLabTestId { get; private set; }

        public PatientLabTestIdentifierQuery()
        {
        }

        public PatientLabTestIdentifierQuery(int patientId, int patientLabTestId) : this()
        {
            PatientId = patientId;
            PatientLabTestId = patientLabTestId;
        }
    }
}