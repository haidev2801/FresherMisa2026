using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Department;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(IConfiguration configuration) : base(configuration)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<Department> GetDepartmentByCode(string code)
        {
            string query = SQLExtension.GetQuery("Department.GetByCode");
            var @param = new Dictionary<string, object>
            {
                {"@DepartmentCode", code }
            };
            return await _dbConnection.QueryFirstOrDefaultAsync<Department>(query, @param, commandType: System.Data.CommandType.Text);
        }
    }
}
