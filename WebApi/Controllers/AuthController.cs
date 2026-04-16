using Application.Interfaces.Services;
using Application.Models.Requests.Auth;
using Application.Models.Responses;
using Application.Models.Responses.Auth;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Controlador de autenticación.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Inicia sesión con las credenciales proporcionadas.
    /// </summary>
    /// <returns>Token JWT y refresh token</returns>
    [HttpPost("login")]
    [EndpointSummary("Inicia sesión como usuario")]
    [EndpointDescription("Este endpoint permite al usuario iniciar sesión en el sistema utilizando sus credenciales de usuario y contraseña. Genera un token JWT (1-5 min) y un refresh token (15 días).")]
    [ProducesResponseType<GenericResponse<LoginAuthResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginAuthRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = authService.Login(model);
        return Ok(response);
    }

    /// <summary>
    /// Renueva el token de acceso usando un refresh token válido.
    /// </summary>
    /// <returns>Nuevo token JWT y nuevo refresh token</returns>
    [HttpPost("renew")]
    [EndpointSummary("Renovar token de acceso")]
    [EndpointDescription("Este endpoint permite renovar el token de acceso usando un refresh token válido. Devuelve un nuevo token JWT y un nuevo refresh token.")]
    [ProducesResponseType<GenericResponse<LoginAuthResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Renew([FromBody] RenewAuthRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = authService.Renew(model);
        return Ok(response);
    }
}