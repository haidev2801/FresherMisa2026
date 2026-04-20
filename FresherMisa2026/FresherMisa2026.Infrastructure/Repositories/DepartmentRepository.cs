using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Department;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace FresherMisa2026.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for Department entity
    /// </summary>
    /// Created By: dvhai (09/04/2026)
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<int> CountEmployeeByDepartmentAsync(string code)
        {
            string storedProcedureName = "Proc_CountEmployeeByDepartmentCode";
            using var conn = await CreateConnectionAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@p_DepartmentCode", code);

            var result = await conn.QuerySingleAsync<int>(
                storedProcedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        /// <summary>
        /// Lấy department theo code
        /// </summary>
        /// <param name="code">Mã department</param>
        /// <returns>Department tìm thấy hoặc null</returns>
        /// CREATED BY: dvhai (09/04/2026)
        public async Task<Department> GetDepartmentByCode(string code)
        {
            string query = SQLExtension.GetQuery("Department.GetByCode");
            using var conn = await CreateConnectionAsync();
            var @param = new Dictionary<string, object>
            {
                {"@DepartmentCode", code }
            };
            return await conn.QueryFirstOrDefaultAsync<Department>(query,
                @param, commandType: System.Data.CommandType.Text);
        }
    }
}
