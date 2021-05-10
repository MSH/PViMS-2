using System.Collections.Generic;
using System.Threading.Tasks;
using PVIMS.Core.CustomAttributes;

using PVIMS.Core.Models;

using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;

namespace PVIMS.Core.Services
{
    public interface ICustomAttributeService
    {
        /// <summary>
        /// Returns entities that have custom attributes
        /// </summary>
        /// <returns></returns>
        IList<string> ListExtendableEntities();

        /// <summary>
        /// Returns custom attributes of an entity
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        IList<CustomAttributeConfigListItem> ListCustomAttributes(string entityName);

        /// <summary>
        /// Returns reference data for selection attribute
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        IList<SelectionDataItemDetail> ListSelectionDataItems(string attributeName);

        /// <summary>
        /// Adds CustomAttributeConfigDetail record
        /// </summary>
        /// <param name="customAttribute"></param>
        /// <returns></returns>
        void AddCustomAttribute(CustomAttributeConfigDetail customAttribute);

        /// <summary>
        /// Updates CustomAttributeConfigDetail record
        /// </summary>
        /// <param name="customAttribute"></param>
        /// <returns></returns>
        Task UpdateCustomAttributeAsync(CustomAttributeConfigDetail customAttribute);

        /// <summary>
        /// Adds SelectionDataItem record
        /// </summary>
        /// <param name="selectionItem"></param>
        void AddSelectionDataItem(SelectionDataItemDetail selectionItem);

        /// <summary>
        /// Get custom attribute value
        /// </summary>
        /// <param name="selectionItem"></param>
        string GetCustomAttributeValue(string extendableTypeName, string attributeKey, IExtendable extended);
    }
}
