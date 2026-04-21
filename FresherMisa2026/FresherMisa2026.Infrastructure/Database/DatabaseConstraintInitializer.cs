using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace FresherMisa2026.Infrastructure.Database
{
    public static class DatabaseConstraintInitializer
    {
        public static async Task EnsureEmployeeCodeUniqueIndexAsync(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return;
            }

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            const string checkSql = @"
                SELECT COUNT(1)
                FROM information_schema.statistics
                WHERE table_schema = DATABASE()
                  AND table_name = 'employee'
                  AND index_name = 'UX_employee_EmployeeCode';";

            var indexExists = await connection.ExecuteScalarAsync<int>(checkSql);
            if (indexExists > 0)
            {
                return;
            }

            const string createSql = "ALTER TABLE employee ADD CONSTRAINT UX_employee_EmployeeCode UNIQUE (EmployeeCode);";
            await connection.ExecuteAsync(createSql);
        }
    }
}
