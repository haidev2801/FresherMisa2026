using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Infrastructure.Repositories;
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
            // Add in-memory cache for repository-level caching
            services.AddMemoryCache();

            //base
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            return services;
        }
    }
}
