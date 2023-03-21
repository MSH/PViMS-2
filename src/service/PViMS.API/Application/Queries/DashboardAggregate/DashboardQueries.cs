using Dapper;
using Microsoft.Data.SqlClient;
using PVIMS.API.Models;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.DashboardAggregate
{
    public class DashboardQueries
        : IDashboardQueries
    {
        private string _connectionString = string.Empty;

        public DashboardQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<StratValueDto>> ExecuteValueBasedQuery(string valueBasedQuery)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<StratValueDto>(valueBasedQuery);
            }
        }
    }
}