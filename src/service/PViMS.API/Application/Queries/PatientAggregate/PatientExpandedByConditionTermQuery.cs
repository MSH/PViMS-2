using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientExpandedByConditionTermQuery
        : IRequest<PatientExpandedDto>
    {
        [DataMember]
        public string CustomAttributeKey { get; private set; }

        [DataMember]
        public string CustomAttributeValue { get; private set; }

        public PatientExpandedByConditionTermQuery()
        {
        }

        public PatientExpandedByConditionTermQuery(string customAttributeKey, string customAttributeValue) : this()
        {
            CustomAttributeKey = customAttributeKey;
            CustomAttributeValue = customAttributeValue;
        }
    }
}
