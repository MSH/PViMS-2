using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Core.Services
{
    public interface IInfrastructureService
    {
        bool HasAssociatedData(DatasetElement element);
        DatasetElement GetTerminologyMedDra();
        Config GetOrCreateConfig(ConfigType configType);
        void SetConfigValue(ConfigType configType, string configValue);
    }
}
