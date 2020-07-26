using System;
using System.Collections.Generic;
using System.Web.WebPages.Html;

namespace VPS.CustomAttributes
{
    // MIKE: 1-1 between DTO and Attribute
    public class CustomAttributeDetail
    {
        public string AttributeKey { get; set; }
        public string Category { get; set; }
        public bool IsRequired { get; set; }
        public bool IsSearchable { get; set; }
        public CustomAttributeType Type { get; set; }
        public object Value { get; set; }
        public List<SelectListItem> RefData { get; set; }

        public string TransformValueToString()
        {
            DateTime parsedDate;

            if (DateTime.TryParse(Value.ToString(), out parsedDate))
            {
                if(parsedDate > DateTime.MinValue)
                {
                    return parsedDate.ToString("yyyy-MM-dd");
                }
                return string.Empty;
            }
            return Value.ToString();
        }
    }
}
