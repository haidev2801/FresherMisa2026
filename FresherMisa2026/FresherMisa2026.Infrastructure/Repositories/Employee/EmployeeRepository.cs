using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System;
using FresherMisa2026.Entities.Extensions;
using System.Text.Json;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IConfiguration configuration, IMemoryCache cache) : base(configuration, cache)
        {
        }

        public async Task<Employee> GetEmployeeByCode(string code)
        {
            // Build SQL dynamically to avoid referencing IsDeleted when the column doesn't exist
            var query = new StringBuilder($"select * from {_tableName} where EmployeeCode = @EmployeeCode");
            if (_modelType.GetHasDeletedColumn())
            {
                query.Append(" and IsDeleted = FALSE");
            }

            var param = new DynamicParameters();
            param.Add("@EmployeeCode", code);

            try
            {
            using var conn = await GetOpenConnectionAsync();
            try
            {
                return await conn.QueryFirstOrDefaultAsync<Employee>(query.ToString(), param, commandType: System.Data.CommandType.Text);
            }
            catch (MySqlException mex)
            {
                if (mex.Number == 1062)
                {
                    throw new FresherMisa2026.Infrastructure.Exceptions.DuplicateKeyException(null, mex.Message);
                }

                throw;
            }
            }
            catch (Exception ex)
            {
                if (ex.Message != null && ex.Message.IndexOf("Unknown column 'IsDeleted'", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Retry without IsDeleted condition
                    var q = query.ToString().Replace(" where IsDeleted = FALSE", "").Replace(" and IsDeleted = FALSE", "");
                    return await _dbConnection.QueryFirstOrDefaultAsync<Employee>(q, param, commandType: System.Data.CommandType.Text);
                }

                throw;
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId)
        {
            var query = new StringBuilder($"select * from {_tableName} where DepartmentID=@DepartmentID");
            if (_modelType.GetHasDeletedColumn())
            {
                query.Append(" and IsDeleted = FALSE");
            }

            var param = new DynamicParameters();
            param.Add("@DepartmentID", departmentId);

            try { Console.WriteLine("[GetEmployeesByDepartmentId] SQL: " + query.ToString()); Console.WriteLine("[GetEmployeesByDepartmentId] Params: " + JsonSerializer.Serialize(new { DepartmentID = departmentId })); } catch { }

            try
            {
            using var conn = await GetOpenConnectionAsync();
            try
            {
                return await conn.QueryAsync<Employee>(query.ToString(), param, commandType: System.Data.CommandType.Text);
            }
            catch (MySqlException mex)
            {
                if (mex.Number == 1062)
                {
                    throw new FresherMisa2026.Infrastructure.Exceptions.DuplicateKeyException(null, mex.Message);
                }

                throw;
            }
            }
            catch (Exception ex)
            {
                if (ex.Message != null && ex.Message.IndexOf("Unknown column 'IsDeleted'", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var q = query.ToString().Replace(" where IsDeleted = FALSE", "").Replace(" and IsDeleted = FALSE", "");
                    return await _dbConnection.QueryAsync<Employee>(q, param, commandType: System.Data.CommandType.Text);
                }

                throw;
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId)
        {
            var query = new StringBuilder($"select * from {_tableName} where PositionID=@PositionID");
            if (_modelType.GetHasDeletedColumn())
            {
                query.Append(" and IsDeleted = FALSE");
            }

            var param = new DynamicParameters();
            param.Add("@PositionID", positionId);

            try
            {
                return await _dbConnection.QueryAsync<Employee>(query.ToString(), param, commandType: System.Data.CommandType.Text);
            }
            catch (Exception ex)
            {
                if (ex.Message != null && ex.Message.IndexOf("Unknown column 'IsDeleted'", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var q = query.ToString().Replace(" where IsDeleted = FALSE", "").Replace(" and IsDeleted = FALSE", "");
                    return await _dbConnection.QueryAsync<Employee>(q, param, commandType: System.Data.CommandType.Text);
                }

                throw;
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByFilter(Guid? departmentId, Guid? positionId, decimal? salaryFrom, decimal? salaryTo, int? gender, DateTime? hireDateFrom, DateTime? hireDateTo)
        {
            var query = new StringBuilder($"select * from {_tableName}");
            var parameters = new DynamicParameters();
            var logParams = new Dictionary<string, object?>();

            // Nếu model có cột IsDeleted, thêm điều kiện lọc; nếu không thì bắt đầu query không có where
            bool whereStarted = false;
            if (_modelType.GetHasDeletedColumn())
            {
                query.Append(" where IsDeleted = FALSE");
                whereStarted = true;
            }

            if (departmentId.HasValue)
            {
                if (!whereStarted) { query.Append(" where "); whereStarted = true; } else { query.Append(" and "); }
                query.Append("DepartmentID = @DepartmentID");
                parameters.Add("@DepartmentID", departmentId.Value);
                logParams["@DepartmentID"] = departmentId.Value;
            }

            if (positionId.HasValue)
            {
                if (!whereStarted) { query.Append(" where "); whereStarted = true; } else { query.Append(" and "); }
                query.Append("PositionID = @PositionID");
                parameters.Add("@PositionID", positionId.Value);
                logParams["@PositionID"] = positionId.Value;
            }

            if (salaryFrom.HasValue)
            {
                if (!whereStarted) { query.Append(" where "); whereStarted = true; } else { query.Append(" and "); }
                query.Append("Salary >= @SalaryFrom");
                parameters.Add("@SalaryFrom", salaryFrom.Value);
                logParams["@SalaryFrom"] = salaryFrom.Value;
            }

            if (salaryTo.HasValue)
            {
                if (!whereStarted) { query.Append(" where "); whereStarted = true; } else { query.Append(" and "); }
                query.Append("Salary <= @SalaryTo");
                parameters.Add("@SalaryTo", salaryTo.Value);
                logParams["@SalaryTo"] = salaryTo.Value;
            }

            if (gender.HasValue)
            {
                if (!whereStarted) { query.Append(" where "); whereStarted = true; } else { query.Append(" and "); }
                query.Append("Gender = @Gender");
                parameters.Add("@Gender", gender.Value);
                logParams["@Gender"] = gender.Value;
            }

            if (hireDateFrom.HasValue)
            {
                if (!whereStarted) { query.Append(" where "); whereStarted = true; } else { query.Append(" and "); }
                query.Append("CreatedDate >= @HireDateFrom");
                parameters.Add("@HireDateFrom", hireDateFrom.Value);
                logParams["@HireDateFrom"] = hireDateFrom.Value;
            }

            if (hireDateTo.HasValue)
            {
                if (!whereStarted) { query.Append(" where "); whereStarted = true; } else { query.Append(" and "); }
                query.Append("CreatedDate <= @HireDateTo");
                parameters.Add("@HireDateTo", hireDateTo.Value);
                logParams["@HireDateTo"] = hireDateTo.Value;
            }

            // Log query and parameters for debugging
            try
            {
                Console.WriteLine("[EmployeeRepository.GetEmployeesByFilter] SQL: " + query.ToString());
                Console.WriteLine("[EmployeeRepository.GetEmployeesByFilter] Params: " + JsonSerializer.Serialize(logParams));
            }
            catch { }

            using var conn = await GetOpenConnectionAsync();
            try
            {
                return await conn.QueryAsync<Employee>(query.ToString(), parameters, commandType: System.Data.CommandType.Text);
            }
            catch (MySqlException mex)
            {
                if (mex.Number == 1062)
                {
                    throw new FresherMisa2026.Infrastructure.Exceptions.DuplicateKeyException(null, mex.Message);
                }

                throw;
            }
        }
    }
}