using Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services;

/// <summary>
/// Implementación del servicio de caché en memoria usando IMemoryCache.
/// </summary>
public class CacheService(IMemoryCache memoryCache) : ICacheService
{
    public T Create<T>(string key, TimeSpan expiration, T value)
    {
        var create = memoryCache.GetOrCreate(key, (factory) =>
        {
            factory.SlidingExpiration = expiration;
            return value;
        });
        return create is null ? throw new Exception("No se pudo establecer la caché") : create;
    }

    public bool Delete(string key)
    {
        memoryCache.Remove(key);
        return true;
    }

    public T? Get<T>(string key)
    {
        return memoryCache.Get<T>(key);
    }
}