namespace FresherMisa2026.Application.Interfaces.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? ttl = null);
        void Remove(string key);
    }
}
