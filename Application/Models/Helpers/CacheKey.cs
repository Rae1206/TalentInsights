namespace Application.Models.Helpers;

/// <summary>
/// Representa una clave de caché con tiempo de expiración asociado.
/// </summary>
public class CacheKey
{
    public required string Key { get; set; }
    public required TimeSpan Expiration { get; set; }
}