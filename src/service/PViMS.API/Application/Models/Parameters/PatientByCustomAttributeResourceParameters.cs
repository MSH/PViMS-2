namespace PVIMS.API.Models.Parameters
{
    public class PatientByCustomAttributeResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Filter patients by cohort group
        /// </summary>
        public int CohortGroupId { get; set; } = 0;

        /// <summary>
        /// Filter patients by medical record number
        /// </summary>
        public string MedicalRecordNumber { get; set; } = "";
    }
}
