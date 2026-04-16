namespace Application.Models.Responses.Auth;

/// <summary>
/// Respuesta de autenticación con token JWT y refresh token.
/// </summary>
public class LoginAuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}