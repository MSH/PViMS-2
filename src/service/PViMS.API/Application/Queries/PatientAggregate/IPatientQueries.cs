using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    public interface IPatientQueries
    {
        Task<IEnumerable<PatientDto>> GetPatientsWithMissingTLDAsync();
    }
}
