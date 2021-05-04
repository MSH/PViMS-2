using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.ReportInstance
{
    public interface IReportInstanceQueries
    {
        Task<IEnumerable<ReportInstanceEventDto>> GetExecutionStatusEventsForPatientViewAsync(int patientId);

        Task<IEnumerable<ReportInstanceEventDto>> GetExecutionStatusEventsForEventViewAsync(int patientClinicalEventId);
    }
}
