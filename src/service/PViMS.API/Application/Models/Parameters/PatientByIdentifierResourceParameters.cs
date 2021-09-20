namespace PVIMS.API.Models.Parameters
{
    public class PatientByIdentifierResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Filter patients by identifier
        /// </summary>
        public string SearchTerm { get; set; } = "";
    }
}
