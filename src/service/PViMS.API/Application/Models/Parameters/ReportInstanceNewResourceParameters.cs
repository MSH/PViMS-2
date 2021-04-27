using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models.Parameters
{
    public class ReportInstanceNewResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like report instances returned in  
        /// Default order attribute is Id  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Id";

        /// <summary>
        /// Filter reports by generic search term
        /// </summary>
        [StringLength(100)]
        public string SearchTerm { get; set; } = "";
    }
}
