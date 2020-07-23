namespace PVIMS.API.Models.Parameters
{
    public class CohortGroupResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like cohort groups returned in  
        /// Default order attribute is Id  
        /// Other valid options are CohortName, CohortCode, StartDate
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Id";
    }
}
