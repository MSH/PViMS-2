using System.Threading.Tasks;
using PVIMS.Core.Models;

namespace PVIMS.Core.Services
{
    public interface IArtefactService
    {
        ArtefactInfoModel CreateActiveDatasetForDownload(long[] patientIds, long cohortGroupId);

        ArtefactInfoModel CreateSpontaneousDatasetForDownload();

        Task<ArtefactInfoModel> CreateDatasetInstanceForDownloadAsync(long datasetInstanceId);

        Task<ArtefactInfoModel> CreateE2BAsync(long datasetInstanceId);
    }
}
