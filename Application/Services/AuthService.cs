using Application.Interfaces.Services;
using Application.Models.DTOs;
using Application.Models.Requests.User;
using Application.Models.Responses;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Shared.Constants;
using Shared.Exceptions;
using Shared.Helpers;

namespace Application.Services;

public class AuthService(
    IAuthRepository authRepository,
    ILogger<AuthService> logger) : IAuthService
{
    public LoginResponse Login(LoginUserRequest model, string jwtSecretKey)
    {
        logger.LogInformation("Intento de inicio de sesión con email: {Email}", model.Email);

        var user = authRepository.GetByEmail(model.Email);

        if (user is null)
        {
            logger.LogWarning("Credenciales inválidas para email: {Email}", model.Email);
            throw new UnauthorizedAccessException(ErrorConstants.INVALID_CREDENTIALS);
        }

        if (!user.IsActive)
        {
            logger.LogWarning("Intento de inicio de sesión con cuenta deshabilitada: {Email}", model.Email);
            throw new ForbiddenException(ErrorConstants.ACCOUNT_DISABLED);
        }

        if (!authRepository.VerifyPassword(user.UserId, model.Password))
        {
            logger.LogWarning("Contraseña incorrecta para email: {Email}", model.Email);
            throw new UnauthorizedAccessException(ErrorConstants.INVALID_CREDENTIALS);
        }

        var token = TokenHelper.GenerateJwtToken(
            user.UserId, user.FullName, user.Role, jwtSecretKey);

        var expiresAt = DateTime.UtcNow.AddMinutes(JwtConstants.ExpirationMinutes);

        logger.LogInformation("Inicio de sesión exitoso para usuario: {Email} | ID: {UserId}",
            model.Email, user.UserId);

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role
        };
    }
}
