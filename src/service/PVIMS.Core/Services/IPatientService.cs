using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using System.Collections.Generic;

namespace PVIMS.Core.Services
{
    public interface IPatientService
    {
        SeriesValueList[] GetElementValues(long patientId, string elementName, int records);
        SeriesValueListItem GetCurrentElementValueForPatient(long patientId, string elementName);

        bool isUnique(List<CustomAttributeParameter> parameters, int patientId = 0);
        bool Exists(List<CustomAttributeParameter> parameters);
        int AddPatient(PatientDetail patientDetail);
        int AddEncounter(Patient patient, EncounterDetail encounterDetail);
    }
}
