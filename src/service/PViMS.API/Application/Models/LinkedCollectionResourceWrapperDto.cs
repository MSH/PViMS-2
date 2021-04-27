using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    [DataContract()]
    public class LinkedCollectionResourceWrapperDto<T> : LinkedResourceBaseDto
        where T : LinkedResourceBaseDto
    {
        [DataMember()]
        public IEnumerable<T> Value { get; set; }

        [DataMember()]
        public int recordCount { get; set; }

        // Include parameterless constructor to allow application/xml response (else 406 not accepted error returned)
        public LinkedCollectionResourceWrapperDto()
        {
        }

        public LinkedCollectionResourceWrapperDto(int totalRecordCount, IEnumerable<T> value)
        {
            Value = value;
            recordCount = totalRecordCount;
        }
    }
}
