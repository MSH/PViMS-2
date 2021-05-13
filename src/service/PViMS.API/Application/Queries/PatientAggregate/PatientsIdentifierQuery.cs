﻿using MediatR;
using PVIMS.API.Models;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientsIdentifierQuery
        : IRequest<LinkedCollectionResourceWrapperDto<PatientIdentifierDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public string FacilityName { get; private set; }

        [DataMember]
        public int CustomAttributeId { get; private set; }

        [DataMember]
        public string CustomAttributeValue { get; private set; }

        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public string FirstName { get; private set; }

        [DataMember]
        public string LastName { get; private set; }

        [DataMember]
        public DateTime DateOfBirth { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public PatientsIdentifierQuery()
        {
        }

        public PatientsIdentifierQuery(string orderBy, string facilityName, int customAttributeId, string customAttributeValue, int patientId, DateTime dateOfBirth, string firstName, string lastName, int pageNumber, int pageSize) : this()
        {
            OrderBy = orderBy;
            FacilityName = facilityName;
            CustomAttributeId = customAttributeId;
            CustomAttributeValue = customAttributeValue;
            PatientId = patientId;
            DateOfBirth = dateOfBirth;
            FirstName = firstName;
            LastName = lastName;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
