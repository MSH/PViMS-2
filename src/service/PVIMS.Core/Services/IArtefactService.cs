using System;

using PVIMS.Core.Models;

namespace PVIMS.Core.Services
{
    public interface IArtefactService
    {
        ArtefactInfoModel CreateActiveDatasetForDownload(long[] patientIds, long cohortGroupId);

        ArtefactInfoModel CreateDatasetInstanceForDownload(long datasetInstanceId);

        ArtefactInfoModel CreateE2B(long datasetInstanceId);

        ArtefactInfoModel CreatePatientSummaryForActiveReport(Guid contextGuid);

        ArtefactInfoModel CreatePatientSummaryForSpontaneousReport(Guid contextGuid);

        ArtefactInfoModel CreateSpontaneousDatasetForDownload();
    }
}
