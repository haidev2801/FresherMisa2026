using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            //base
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();

            // Một instance EmployeeRepository cho cả IEmployeeRepository và IBaseRepository<Employee> (Insert/Update override bắt trùng mã).
            services.AddScoped<EmployeeRepository>();
            services.AddScoped<IEmployeeRepository>(sp => sp.GetRequiredService<EmployeeRepository>());
            services.AddScoped<IBaseRepository<Employee>>(sp => sp.GetRequiredService<EmployeeRepository>());

            return services;
        }
    }
}
