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
    /// Extension method đăng ký DI cho Infrastructure layer.
    /// Đăng ký BaseRepository<> theo scoped để inject vào service khi cần.
    /// </summary>
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            //base
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            return services;
        }
    }
}
