using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application
{
    /// <summary>
    /// Extension method đăng ký DI cho Application layer.
    /// Đăng ký BaseService<> theo scoped để inject vào controller/service khi cần.
    /// </summary>
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationDI(
            this IServiceCollection services)
        {
            //base
            services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));

            return services;
        }
    }
}
