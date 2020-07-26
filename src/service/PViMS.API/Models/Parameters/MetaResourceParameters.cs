namespace PVIMS.API.Models.Parameters
{
    public class MetaResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like meta resources returned in  
        /// Default order attribute is Id  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Id";
    }
}
