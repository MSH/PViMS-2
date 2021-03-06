﻿using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A product representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class ProductDetailDto : ProductIdentifierDto
    {
        /// <summary>
        /// The concept the product has implemented
        /// </summary>
        [DataMember]
        public string ConceptName { get; set; }

        /// <summary>
        /// The form of the concept
        /// </summary>
        [DataMember]
        public string FormName { get; set; }

        /// <summary>
        /// The manufacturer
        /// </summary>
        [DataMember]
        public string Manufacturer { get; set; }
    }
}
