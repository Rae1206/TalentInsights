using Microsoft.IdentityModel.Tokens;

namespace Application.Models.Helpers;

/// <summary>
/// Configuración de tokens JWT.
/// </summary>
public class TokenConfiguration
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required SymmetricSecurityKey SecurityKey { get; set; }
    public required DateTime Expiration { get; set; }
    public required TimeSpan ExpirationTimeSpan { get; set; }
}