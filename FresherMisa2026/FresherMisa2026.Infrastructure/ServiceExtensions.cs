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
    /// <summary>
    /// Đăng ký Dependency Injection cho tầng Infrastructure
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Thêm các repository và hạ tầng vào DI container
        /// </summary>
        /// <param name="services">DI container</param>
        /// <returns>DI container sau khi đăng ký</returns>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
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
