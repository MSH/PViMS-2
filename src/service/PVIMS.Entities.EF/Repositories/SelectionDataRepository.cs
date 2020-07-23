using PVIMS.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using VPS.Common.Repositories;
using VPS.Common.Utilities;

namespace PVIMS.Infrastructure.Shared.Repositories
{
    public class SelectionDataRepository : ISelectionDataRepository
    {
        private readonly IRepositoryInt<Core.Entities.SelectionDataItem> entityRepo;

        public SelectionDataRepository(IUnitOfWorkInt unitOfwork)
        {
            Check.IsNotNull(unitOfwork, "unitOfwork may not be null");

            this.entityRepo = unitOfwork.Repository<Core.Entities.SelectionDataItem>();
        }

        /// <summary>
        /// Retrieves the selection data for specified attribute attributeKey.
        /// </summary>
        /// <param name="attributeKey">The attribute key.</param>
        /// <returns></returns>
        public ICollection<Core.Entities.SelectionDataItem> RetrieveSelectionDataForAttribute(string attributeKey)
        {
            return entityRepo.Queryable()
                .Where(di => di.AttributeKey == attributeKey)
                .ToList();
        }

        /// <summary>
        /// Retrieves all selection data.
        /// </summary>
        /// <returns></returns>
        public ICollection<Core.Entities.SelectionDataItem> RetrieveAllSelectionData()
        {
            return entityRepo.List();
        }

        public void AddSelectionDataItem(Core.Entities.SelectionDataItem newItem)
        {
            entityRepo.Save(newItem);
        }

        public void RemoveSelectionDataItem(long selectionDataItemId)
        {
            var deleteItem = entityRepo.Get(selectionDataItemId);
            entityRepo.Delete(deleteItem);
        }

    }
}
