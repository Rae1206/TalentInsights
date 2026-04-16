using System.ComponentModel.DataAnnotations;
using Shared.Constants;

namespace Application.Models.Requests.Auth;

/// <summary>
/// Request para renovar el token de acceso usando refresh token.
/// </summary>
public class RenewAuthRequest
{
    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    public string RefreshToken { get; set; } = null!;
}
