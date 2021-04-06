using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using System;
using System.Linq;

namespace PVIMS.Services
{
    public class InfrastructureService : IInfrastructureService 
    {
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IRepositoryInt<DatasetInstanceValue> _instanceValueRepository;

        public InfrastructureService(IUnitOfWorkInt unitOfWork,
            IRepositoryInt<DatasetInstanceValue> instanceValueRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _instanceValueRepository = instanceValueRepository ?? throw new ArgumentNullException(nameof(instanceValueRepository));
        }

        #region "Referential Checks"

        public bool HasAssociatedData(DatasetElement element)
        {
            var hasData = false;

            hasData = (element.DatasetCategoryElements.Count > 0 || element.DatasetElementSubs.Count > 0 || _instanceValueRepository.Queryable().Any(div => div.DatasetElement.Id == element.Id));

            return hasData;
        }

        public DatasetElement GetTerminologyMedDra()
        {
            var meddraElement = _unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == "TerminologyMedDra");
            if (meddraElement == null)
            {
                meddraElement = new DatasetElement()
                {
                    // Prepare new element
                    DatasetElementType = _unitOfWork.Repository<DatasetElementType>().Queryable().Single(x => x.Description == "Generic"),
                    Field = new Field()
                    {
                        Anonymise = false,
                        Mandatory = false,
                        FieldType = _unitOfWork.Repository<FieldType>().Queryable().Single(x => x.Description == "AlphaNumericTextbox")
                    },
                    ElementName = "TerminologyMedDra",
                    DefaultValue = string.Empty,
                    Oid = string.Empty,
                    System = true
                };
                var rule = meddraElement.GetRule(DatasetRuleType.ElementCanoOnlyLinkToSingleDataset);
                rule.RuleActive = true;

                _unitOfWork.Repository<DatasetElement>().Save(meddraElement);
            }
            return meddraElement;
        }

        public Config GetOrCreateConfig(ConfigType configType)
        {
            var config = _unitOfWork.Repository<Config>().Queryable().
                FirstOrDefault(c => c.ConfigType == configType);

            if (config == null)
            {
                config = new Config()
                {
                    // Prepare new config
                    ConfigType = configType,
                    ConfigValue = ""
                };
                _unitOfWork.Repository<Config>().Save(config);
            }
            return config;
        }

        public void SetConfigValue(ConfigType configType, string configValue)
        {
            var config = _unitOfWork.Repository<Config>().Queryable().
                FirstOrDefault(c => c.ConfigType == configType);

            if (config == null)
            {
                config = new Config()
                {
                    // Prepare new config
                    ConfigType = configType,
                    ConfigValue = configValue
                };
                _unitOfWork.Repository<Config>().Save(config);
            }
            else
            {
                config.ConfigValue = configValue;
                _unitOfWork.Repository<Config>().Update(config);
            }
            _unitOfWork.Complete();
        }

        #endregion

        #region "Private"

        #endregion
    }
}
