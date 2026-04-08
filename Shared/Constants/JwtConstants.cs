namespace Shared.Constants;

/// <summary>
/// Constantes de configuración JWT.
/// </summary>
public static class JwtConstants
{
    public const string Issuer = "Twitter.WebApi";
    public const string Audience = "Twitter.WebApi.Users";
    public const int ExpirationMinutes = 1440; // 24 horas
}
