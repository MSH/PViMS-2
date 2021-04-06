using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Entities
{
    public class Dataset : AuditedEntityBase
	{
		public Dataset()
		{
            Created = DateTime.Now;
            Active = true;
            IsSystem = false;

			DatasetCategories = new HashSet<DatasetCategory>();
            DatasetInstances = new HashSet<DatasetInstance>();
            DatasetRules = new HashSet<DatasetRule>();
            WorkPlans = new HashSet<WorkPlan>();
        }

        public string DatasetName { get; set; }
        public bool Active { get; set; }
        public string InitialiseProcess { get; set; }
        public string RulesProcess { get; set; }
        public string Help { get; set; }
        public int ContextTypeId { get; set; }
        public int? EncounterTypeWorkPlanId { get; set; }
        public string Uid { get; set; }
        public bool IsSystem { get; set; }
        public int? DatasetXmlId { get; set; }

		public virtual ContextType ContextType { get; set; }
        public virtual EncounterTypeWorkPlan EncounterTypeWorkPlan { get; set; }
        public virtual DatasetXml DatasetXml { get; set; }

		public virtual ICollection<DatasetCategory> DatasetCategories { get; set; }
        public virtual ICollection<DatasetInstance> DatasetInstances { get; set; }
        public virtual ICollection<DatasetRule> DatasetRules { get; set; }
        public virtual ICollection<WorkPlan> WorkPlans { get; set; }

        public DatasetInstance CreateInstance(int contextId, EncounterTypeWorkPlan encounterTypeWorkPlan)
        {
            var instance = new DatasetInstance
            {
                Dataset = this,
                EncounterTypeWorkPlan = encounterTypeWorkPlan,
                ContextId = contextId
            };

            // Create table elements automatically
            foreach(DatasetCategory cat in DatasetCategories)
            {
                foreach(DatasetCategoryElement ele in cat.DatasetCategoryElements)
                {
                    if(ele.DatasetElement.Field.FieldType.Description == "Table")
                    {
                        instance.AddInstanceValue(ele.DatasetElement, "<<Table>>");
                    }
                }
            }

            return instance;
        }

        public DatasetRule GetRule(DatasetRuleType ruleType)
        {
            var rule = DatasetRules.SingleOrDefault(dr => dr.RuleType == ruleType);
            if (rule == null)
            {
                rule = new DatasetRule()
                {
                    Dataset = this,
                    RuleActive = false,
                    RuleType = ruleType
                };
                DatasetRules.Add(rule);
            }

            return rule;
        }
    }
}