using System.ComponentModel.DataAnnotations;
using Shared.Constants;

namespace Application.Models.Requests.Auth;

public class LoginAuthRequest
{
    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    public string Password { get; set; } = null!;
}
