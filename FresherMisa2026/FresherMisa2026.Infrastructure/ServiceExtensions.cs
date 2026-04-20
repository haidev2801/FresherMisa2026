using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Infrastructure.Implementations;
using FresherMisa2026.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FresherMisa2026.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            //base
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            services.AddMemoryCache();

            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ICacheService, MemoryCacheService>();

            return services;
        }
    }
}
