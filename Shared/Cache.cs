using Microsoft.Extensions.Caching.Memory;

namespace Shared;

/// <summary>
/// Cache genérico en memoria - Clase base de utilidad
/// </summary>
public class Cache
{
    private readonly IMemoryCache _memoryCache;

    public Cache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Crea o actualiza una entrada en caché
    /// </summary>
    public T Set<T>(string key, T value, TimeSpan expiration)
    {
        var options = new MemoryCacheEntryOptions
        {
            SlidingExpiration = expiration
        };

        return _memoryCache.Set(key, value, options);
    }

    /// <summary>
    /// Obtiene una entrada del caché
    /// </summary>
    public T? Get<T>(string key)
    {
        return _memoryCache.Get<T>(key);
    }

    /// <summary>
    /// Elimina una entrada del caché
    /// </summary>
    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }

    /// <summary>
    /// Verifica si una clave existe en caché
    /// </summary>
    public bool Exists(string key)
    {
        return _memoryCache.TryGetValue(key, out _);
    }
}
