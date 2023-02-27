using PVIMS.API.Application.Models;
using PVIMS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Infrastructure.Services
{
    public interface IExcelDocumentService
    {
        void CreateDocument(ArtifactDto model);

        void AddSheet(string sheetName, List<List<string>> data);

        ArtefactInfoModel CreateActiveDatasetForDownload(long[] patientIds, long cohortGroupId);

        ArtefactInfoModel CreateSpontaneousDatasetForDownload();

        Task<ArtefactInfoModel> CreateDatasetInstanceForDownloadAsync(long datasetInstanceId);
    }
}
