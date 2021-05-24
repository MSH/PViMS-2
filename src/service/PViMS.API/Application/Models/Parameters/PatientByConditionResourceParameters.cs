namespace PVIMS.API.Models.Parameters
{
    public class PatientByConditionResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Filter patients by custom attribute key
        /// </summary>
        public string CustomAttributeKey { get; set; } = "";

        /// <summary>
        /// Filter patients by custom attribute value
        /// </summary>
        public string CustomAttributeValue { get; set; } = "";
    }
}
