using VPS.Common.Domain;

namespace PVIMS.Infrastructure.Shared.Repositories
{
	public class EntityFrameworkRepository<T> : DomainRepository<T> where T : Entity<int>
	{
		public EntityFrameworkRepository(EntityFrameworkUnitOfWork unitOfWork) : base(unitOfWork._dbContext)
		{
		}
	}
}
