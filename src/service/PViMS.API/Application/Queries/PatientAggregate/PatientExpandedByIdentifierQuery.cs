using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientExpandedByIdentifierQuery
        : IRequest<PatientExpandedDto>
    {
        [DataMember]
        public string SearchTerm { get; private set; }

        public PatientExpandedByIdentifierQuery()
        {
        }

        public PatientExpandedByIdentifierQuery(string searchTerm) : this()
        {
            SearchTerm = searchTerm;
        }
    }
}
