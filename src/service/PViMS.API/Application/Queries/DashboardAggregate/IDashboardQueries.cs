using PVIMS.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.DashboardAggregate
{
    public interface IDashboardQueries
    {
        Task<IEnumerable<StratValueDto>> ExecuteValueBasedQuery(string valueBasedQuery);
    }
}
