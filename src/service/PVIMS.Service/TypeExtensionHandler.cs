﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using VPS.CustomAttributes;
using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using ICustomAttributeConfigRepository = PVIMS.Core.Repositories.ICustomAttributeConfigRepository;
using ISelectionDataRepository = PVIMS.Core.Repositories.ISelectionDataRepository;
using ITypeExtensionHandler = PVIMS.Core.Services.ITypeExtensionHandler;
using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;

namespace PVIMS.Services
{
    public class TypeExtensionHandler : ITypeExtensionHandler
    {
        private readonly ICustomAttributeConfigRepository attributeConfigRepository;
        private readonly ISelectionDataRepository selectionDataRepository;
        private ICollection<SelectionDataItem> allSelectionDataItems = new Collection<SelectionDataItem>();

        public TypeExtensionHandler(ICustomAttributeConfigRepository attributeConfigRepository,
            ISelectionDataRepository selectionDataRepository)
        {
            this.attributeConfigRepository = attributeConfigRepository;
            this.selectionDataRepository = selectionDataRepository;
        }

        /// <summary>
        /// Caches all selection data items.
        /// </summary>
        public void CacheSelectionDataItems()
        {
            allSelectionDataItems = selectionDataRepository.RetrieveAllSelectionData();
        }

        /// <summary>
        /// Returns a list of unpopulated Custom Attributes for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<CustomAttributeDetail> BuildModelExtension<T>() where T : IExtendable
        {
            var attributeConfigs = attributeConfigRepository.RetrieveAttributeConfigurationsForType(typeof(T).Name);

            var modelExtension = new List<CustomAttributeDetail>();

            if (attributeConfigs == null || !attributeConfigs.Any())
                return modelExtension;

            foreach (var customAttributeConfig in attributeConfigs)
            {
                var attributeDetail = new CustomAttributeDetail();
                attributeDetail.AttributeKey = customAttributeConfig.AttributeKey;
                attributeDetail.Category = customAttributeConfig.Category;
                attributeDetail.Type = customAttributeConfig.CustomAttributeType;
                attributeDetail.Value = GetAttributeValue(null, customAttributeConfig);
                attributeDetail.IsRequired = customAttributeConfig.IsRequired;

                //if (customAttributeConfig.CustomAttributeType == CustomAttributeType.Selection)
                //{
                //    var refData = RetrieveSelectionDataForAttribute(customAttributeConfig.AttributeKey);

                //    if (refData != null && refData.Any())
                //        attributeDetail.RefData = refData.Select(s =>
                //                new SelectListItem
                //                {
                //                    Value = s.SelectionKey.ToString(),
                //                    Text = s.Value
                //                }).ToList();
                //}

                modelExtension.Add(attributeDetail);
            }

            return modelExtension;
        }

        private ICollection<SelectionDataItem> RetrieveSelectionDataForAttribute(string attributeKey)
        {
            var selectionDataItems = allSelectionDataItems.Where(s => s.AttributeKey == attributeKey).ToList();

            if (selectionDataItems != null && selectionDataItems.Count() > 0) return selectionDataItems;

            return selectionDataRepository.RetrieveSelectionDataForAttribute(attributeKey);
        }

        /// <summary>
        /// Returns list of Custom Attributes for the specified extendable object with the values prepopulated
        /// </summary>
        /// <param name="extendableObject">The extendable object.</param>
        /// <returns></returns>
        public List<CustomAttributeDetail> BuildModelExtension(IExtendable extendableObject)
        {
            var modelExtension = new List<CustomAttributeDetail>();

            var attributeConfigs = attributeConfigRepository.RetrieveAttributeConfigurationsForType(extendableObject.GetType().Name);

            if (attributeConfigs == null || !attributeConfigs.Any())
                attributeConfigs = attributeConfigRepository.RetrieveAttributeConfigurationsForType(extendableObject.GetType().BaseType.Name);

            if (attributeConfigs == null || !attributeConfigs.Any())
                return modelExtension;

            foreach (var customAttributeConfig in attributeConfigs)
            {
                var attributeDetail = new CustomAttributeDetail();
                attributeDetail.AttributeKey = customAttributeConfig.AttributeKey;
                attributeDetail.Category = customAttributeConfig.Category;
                attributeDetail.Type = customAttributeConfig.CustomAttributeType;
                attributeDetail.Value = GetAttributeValue(extendableObject, customAttributeConfig);

                //if (customAttributeConfig.CustomAttributeType == CustomAttributeType.Selection)
                //{
                //    var refData = RetrieveSelectionDataForAttribute(customAttributeConfig.AttributeKey);

                //    if (refData != null && refData.Any())
                //        attributeDetail.RefData = refData.Select(s =>
                //                new SelectListItem
                //                {
                //                    Value = s.SelectionKey.ToString(),
                //                    Text = s.Value,
                //                    Selected = (attributeDetail.Value != null && s.Id == Convert.ToInt32(attributeDetail.Value))
                //                }).ToList();
                //}

                modelExtension.Add(attributeDetail);
            }

            return modelExtension;
        }


        /// <summary>
        /// Refreshes the reference data for the selection type attributes in the specified collection of attributes.
        /// </summary>
        /// <param name="customAttributes">The custom attributes.</param>
        /// <returns></returns>
        public List<CustomAttributeDetail> RefreshReferenceData(List<CustomAttributeDetail> customAttributes)
        {
            if (customAttributes == null || !customAttributes.Any())
                return new List<CustomAttributeDetail>();

            foreach (var customAttribute in customAttributes)
            {
                customAttribute.Value = ExtractValue(customAttribute);

                //if (customAttribute.Type == CustomAttributeType.Selection)
                //{
                //    var refData = RetrieveSelectionDataForAttribute(customAttribute.AttributeKey);

                //    if (refData != null && refData.Any())
                //        customAttribute.RefData = refData.Select(s =>
                //                new SelectListItem
                //                {
                //                    Value = s.SelectionKey.ToString(),
                //                    Text = s.Value,
                //                    Selected = (customAttribute.Value != null && s.Id == Convert.ToInt32(ExtractValue(customAttribute)))
                //                }).ToList();
                //}
            }

            return customAttributes;
        }

        /// <summary>
        /// Updates the extendable object with values from the custom attribute collection.
        /// </summary>
        /// <param name="extendableToUpdate">The extendable to update.</param>
        /// <param name="customAttributeDetails">The custom attribute details.</param>
        /// <returns>The updated Extendable object</returns>
        /// <exception cref="CustomAttributeException">Unknown AttributeType for AttributeKey: {0}</exception>
        public IExtendable UpdateExtendable(IExtendable extendableToUpdate, IEnumerable<CustomAttributeDetail> customAttributeDetails, string updatedByUser)
        {
            if (customAttributeDetails == null || !customAttributeDetails.Any())
                return extendableToUpdate;

            foreach (var customAttribute in customAttributeDetails)
            {
                try
                {
                    switch (customAttribute.Type)
                    {
                        case CustomAttributeType.Numeric:
                            extendableToUpdate.SetAttributeValue<Decimal>(customAttribute.AttributeKey, customAttribute.Value == null ? 0 : Convert.ToDecimal(customAttribute.Value), updatedByUser);
                            break;
                        case CustomAttributeType.String:
                            extendableToUpdate.SetAttributeValue<String>(customAttribute.AttributeKey, customAttribute.Value == null ? string.Empty : customAttribute.Value.ToString(), updatedByUser);
                            break;
                        case CustomAttributeType.Selection:
                            extendableToUpdate.SetAttributeValue<Int32>(customAttribute.AttributeKey, customAttribute.Value == null ? 0 : Convert.ToInt32(customAttribute.Value), updatedByUser);
                            break;
                        case CustomAttributeType.DateTime:
                            extendableToUpdate.SetAttributeValue<DateTime>(customAttribute.AttributeKey, customAttribute.Value == null || String.IsNullOrEmpty(customAttribute.Value.ToString()) ? DateTime.MinValue : Convert.ToDateTime(customAttribute.Value), updatedByUser);
                            break;
                        default:
                            throw new CustomAttributeException("Unknown AttributeType for AttributeKey: {0}", customAttribute.AttributeKey);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error setting attribute value for {customAttribute.AttributeKey} with error {ex.Message}");
                }
            }

            return extendableToUpdate;
        }

        private object GetAttributeValue(IExtendable extendable, CustomAttributeConfiguration config)
        {
            switch (config.CustomAttributeType)
            {
                case CustomAttributeType.Numeric:
                    return (extendable == null || extendable.GetAttributeValue(config.AttributeKey) == null)
                        ? default(decimal)
                        : extendable.GetAttributeValue(config.AttributeKey);
                case CustomAttributeType.String:
                    return (extendable == null || extendable.GetAttributeValue(config.AttributeKey) == null)
                        ? " "// Not a big fan of this but Razor does not recognise empty strings and ends up not rendering a control for the attribute.
                        : extendable.GetAttributeValue(config.AttributeKey);
                case CustomAttributeType.Selection:
                    return (extendable == null || extendable.GetAttributeValue(config.AttributeKey) == null)
                        ? default(int)
                        : extendable.GetAttributeValue(config.AttributeKey);
                case CustomAttributeType.DateTime:
                    return (extendable == null || extendable.GetAttributeValue(config.AttributeKey) == null)
                        ? default(DateTime)
                        : extendable.GetAttributeValue(config.AttributeKey);
                default:
                    throw new CustomAttributeException("Unknown AttributeType for AttributeKey: {0}", config.AttributeKey);
            }
        }

        /// <summary>
        /// This is to handle the issue where the MVC Razor Engine is rendering the values for custom attribute controls as Arrays. Need to determine why this is happening. 
        /// </summary>
        /// <param name="customAttributeDetail"></param>
        /// <returns></returns>
        private object ExtractValue(CustomAttributeDetail customAttributeDetail)
        {
            object returnValue = null;

            if (customAttributeDetail.Value != null)
            {
                if (customAttributeDetail.Value.GetType().IsArray)
                {
                    if (customAttributeDetail.Value.GetType() == typeof(string[]) && ((string[])customAttributeDetail.Value).Length == 1)
                        returnValue = ((string[])customAttributeDetail.Value).First();
                }
                else
                    returnValue = customAttributeDetail.Value;
            }

            switch (customAttributeDetail.Type)
            {
                case CustomAttributeType.Numeric:
                    return returnValue == null ? default(decimal) : Decimal.Parse(returnValue.ToString(), CultureInfo.InvariantCulture);
                case CustomAttributeType.String:
                    return returnValue == null ? string.Empty : returnValue.ToString();
                case CustomAttributeType.Selection:
                    return returnValue == null ? default(int) : Int32.Parse(returnValue.ToString(), CultureInfo.InvariantCulture);
                case CustomAttributeType.DateTime:
                    return returnValue == null ? default(DateTime) : Convert.ToDateTime(returnValue);
            }

            throw new CustomAttributeException("Unable to extract custom attribute value for attribute {0}", customAttributeDetail.AttributeKey);
        }
    }
}
