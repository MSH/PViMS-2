using PVIMS.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using VPS.Common.Repositories;
using VPS.Common.Utilities;

namespace PVIMS.Entities.EF.Repositories
{
    public class CustomAttributeConfigRepository : ICustomAttributeConfigRepository
    {
        private readonly IRepositoryInt<Core.Entities.CustomAttributeConfiguration> entityRepo;

        public CustomAttributeConfigRepository(IUnitOfWorkInt unitOfwork)
        {
            Check.IsNotNull(unitOfwork, "unitOfwork may not be null");
            this.entityRepo = unitOfwork.Repository<Core.Entities.CustomAttributeConfiguration>();
        }

        /// <summary>
        /// Retrieves the attribute configurations for the specified type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public IList<Core.Entities.CustomAttributeConfiguration> RetrieveAttributeConfigurationsForType(string typeName)
        {
            return entityRepo.Queryable()
                .Where(ca => ca.ExtendableTypeName == typeName)
                .ToList();
        }

        /// <summary>
        /// Retrieves the attribute keys for the specified type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public IList<string> RetrieveAttributeKeysForType(string typeName)
        {
            return entityRepo.Queryable()
                .Where(ca => ca.ExtendableTypeName == typeName)
                .Select(c => c.AttributeKey)
                .ToList();
        }
    }
}
