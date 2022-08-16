using PVIMS.Core.Extensions;
using PVIMS.Core.Exceptions;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Entities
{
    public enum CausalityScaleType
    {
        WHO = 1,
        Naranjo = 2
    }

    public class DatasetElement : EntityBase
	{
		public DatasetElement()
		{
            DatasetElementGuid = Guid.NewGuid();
            System = false;

            DatasetCategoryElements = new HashSet<DatasetCategoryElement>();
			DatasetElementSubs = new HashSet<DatasetElementSub>();
            DatasetInstanceValues = new HashSet<DatasetInstanceValue>();
            DatasetRules = new HashSet<DatasetRule>();
            DatasetXmlAttributes = new HashSet<DatasetXmlAttribute>();
            DatasetXmlNodes = new HashSet<DatasetXmlNode>();
		}

        public string ElementName { get; set; }
        public int FieldId { get; set; }
        public int DatasetElementTypeId { get; set; }
        public string Oid { get; set; }
        public string DefaultValue { get; set; }
        public bool System { get; set; }
        public string Uid { get; set; }
        public Guid DatasetElementGuid { get; set; }

		public virtual DatasetElementType DatasetElementType { get; set; }
		public virtual Field Field { get; set; }

        public virtual ICollection<DatasetCategoryElement> DatasetCategoryElements { get; set; }
        public virtual ICollection<DatasetElementSub> DatasetElementSubs { get; set; }
        public virtual ICollection<DatasetInstanceValue> DatasetInstanceValues { get; set; }
        public virtual ICollection<DatasetRule> DatasetRules { get; set; }
        public virtual ICollection<DatasetXmlAttribute> DatasetXmlAttributes { get; set; }
        public virtual ICollection<DatasetXmlNode> DatasetXmlNodes { get; set; }

        public void Validate(string instanceValue)
        {
            if (string.IsNullOrWhiteSpace(instanceValue))
            {
                if (Field.Mandatory)
                    throw new DatasetFieldSetException(ElementName, string.Format("{0} is required.", ElementName));
                else
                    return;
            }

            var fieldType = (FieldTypes)Field.FieldType.Id;

            switch (fieldType)
            {
                case FieldTypes.AlphaNumericTextbox:
                    if (Field.MaxLength.HasValue)
                    {
                        if (instanceValue != null && instanceValue.Length > Field.MaxLength.Value)
                            throw new DatasetFieldSetException(ElementName, string.Format("{0} may not contain more than {1} characters.", ElementName, Field.MaxLength.Value));
                    }
                    break;
                case FieldTypes.NumericTextbox:
                    if (!instanceValue.IsNumeric())
                    {
                        throw new DatasetFieldSetException(ElementName, string.Format("{0} must be a numeric value.", ElementName));
                    }

                    var decimalValue = decimal.Parse(instanceValue);


                    if (Field.MinSize.HasValue)
                    {
                        if(decimalValue < Field.MinSize.Value)
                            throw new DatasetFieldSetException(ElementName, string.Format("{0} may not be less than {1}.", ElementName, Field.MinSize.Value));
                    }

                    if (Field.MaxSize.HasValue)
                    {
                        if (decimalValue > Field.MaxSize.Value)
                            throw new DatasetFieldSetException(ElementName, string.Format("{0} may not be more than {1}.", ElementName, Field.MaxSize.Value));
                    }
                    break;
                case FieldTypes.Date:
                    break;
                default:
                    break;
            }
        }

        public DatasetRule GetRule(DatasetRuleType ruleType)
        {
            var rule = DatasetRules.SingleOrDefault(dr => dr.RuleType == ruleType);
            if (rule == null)
            {
                rule = new DatasetRule()
                {
                    DatasetElement = this,
                    RuleActive = false,
                    RuleType = ruleType
                };
                DatasetRules.Add(rule);
            }

            return rule;
        }

        public DatasetElementSub GetCausalityTermForDrug(CausalityScaleType type, FieldType alphaNumericTextbox)
        {
            if (this.Field.FieldType.Description != "Table") { throw new Exception("Invalid element for function call..."); };

            var elementName = String.Format("Terminology{0}", type.ToString());
            var subElement = DatasetElementSubs.SingleOrDefault(ds => ds.ElementName == elementName);
            if (subElement == null)
            {
                subElement = new DatasetElementSub()
                {
                    DatasetElement = this,
                    DefaultValue = null,
                    ElementName = elementName,
                    Field = new Field()
                    {
                        Anonymise = false,
                        Mandatory = false,
                        FieldType = alphaNumericTextbox
                    },
                    FieldOrder = 0,
                    FriendlyName = elementName,
                    Oid = string.Empty,
                    System = true
                };

                DatasetElementSubs.Add(subElement);
            }
            return subElement;
        }
    }
}