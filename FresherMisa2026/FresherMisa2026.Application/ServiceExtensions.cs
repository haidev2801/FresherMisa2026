using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application
{
    /// <summary>
    /// Đăng ký Dependency Injection cho tầng Application
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Thêm các service của tầng Application vào DI container
        /// </summary>
        /// <param name="services">DI container</param>
        /// <returns>DI container sau khi đăng ký</returns>
        public static IServiceCollection AddApplicationDI(
            this IServiceCollection services)
        {
            //base
            services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            services.AddScoped<IDepartmentSerice, DepartmentService>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<IEmployeeService, EmployeeService>();

            return services;
        }
    }
}
