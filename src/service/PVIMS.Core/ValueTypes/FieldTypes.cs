using System.ComponentModel;

namespace PVIMS.Core.ValueTypes
{
    public enum FieldTypes
    {
        [Description("Listbox")]
        Listbox = 1,
        [Description("DropDownList")]
        DropDownList = 2,
        [Description("AlphaNumericTextbox")]
        AlphaNumericTextbox = 3,
        [Description("NumericTextbox")]
        NumericTextbox = 4,
        [Description("YesNo")]
        YesNo = 5,
        [Description("Date")]
        Date = 6,
        [Description("Table")]
        Table = 7,
        [Description("System")]
        System = 8
    }
}
