using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Infrastructure.Repositories;
using FresherMisa2026.Application.Services;
using Microsoft.Extensions.Configuration;
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

            // Repositories
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            // Services
            services.AddScoped<IDepartmentSerice, DepartmentService>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<IEmployeeService, EmployeeService>();

            return services;
        }
    }
}
