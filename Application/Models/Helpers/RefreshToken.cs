namespace Application.Models.Helpers;

/// <summary>
/// Representa un token de refresco almacenado en caché.
/// </summary>
public class RefreshToken
{
    public required Guid UserId { get; set; }
    public required TimeSpan ExpirationInDays { get; set; }
}