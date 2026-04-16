using Application.Helpers;
using Application.Interfaces.Services;
using Application.Models.Helpers;
using Application.Models.Requests.Auth;
using Application.Models.Responses;
using Application.Models.Responses.Auth;
using Twitter.Domain.Database.SqlServer;
using Microsoft.Extensions.Configuration;
using Shared.Constants;

namespace Application.Services;

/// <summary>
/// Servicio de autenticación con JWT y refresh tokens.
/// </summary>
public class AuthService(
    IUnitOfWork unitOfWork,
    IConfiguration configuration,
    ICacheService cacheService) : IAuthService
{
    /// <summary>
    /// Inicia sesión con las credenciales proporcionadas.
    /// </summary>
    public GenericResponse<LoginAuthResponse> Login(LoginAuthRequest model)
    {
        // 1. Buscar usuario por email
        var user = unitOfWork.authRepository.GetByEmail(model.Email)
            ?? throw new UnauthorizedAccessException(ResponseConstants.AUTH_USER_OR_PASSWORD_NOT_FOUND);

        // 2. Validar contraseña
        var validatePassword = unitOfWork.authRepository.VerifyPassword(user.UserId, model.Password);
        if (!validatePassword)
        {
            throw new UnauthorizedAccessException(ResponseConstants.AUTH_USER_OR_PASSWORD_NOT_FOUND);
        }

        // 3. Obtener roles del usuario
        var roles = unitOfWork.roleRepository.GetRolesByUserId(user.UserId)
            .Select(r => r.Name)
            .ToList();

        if (!roles.Any())
        {
            roles = new List<string> { RoleConstants.DefaultRole };
        }

        // 4. Generar tokens
        var token = TokenHelper.Create(user.UserId, roles, configuration, cacheService);
        var refreshToken = TokenHelper.CreateRefresh(user.UserId, configuration, cacheService);

        // 5. Responder
        return ResponseHelper.Create(new LoginAuthResponse
        {
            Token = token,
            RefreshToken = refreshToken
        });
    }

    /// <summary>
    /// Renueva el token de acceso usando un refresh token válido.
    /// </summary>
    public GenericResponse<LoginAuthResponse> Renew(RenewAuthRequest model)
    {
        // 1. Buscar refresh token en cache
        var findRefreshToken = cacheService.Get<RefreshToken>(
            CacheHelper.AuthRefreshTokenKey(model.RefreshToken)
        ) ?? throw new UnauthorizedAccessException(ResponseConstants.AUTH_REFRESH_TOKEN_NOT_FOUND);

        // 2. Obtener usuario
        var user = unitOfWork.authRepository.GetById(findRefreshToken.UserId)
            ?? throw new UnauthorizedAccessException(ResponseConstants.USER_NOT_EXISTS);

        // 3. Obtener roles
        var roles = unitOfWork.roleRepository.GetRolesByUserId(user.UserId)
            .Select(r => r.Name)
            .ToList();

        if (!roles.Any())
        {
            roles = new List<string> { RoleConstants.DefaultRole };
        }

        // 4. Generar nuevos tokens
        var token = TokenHelper.Create(findRefreshToken.UserId, roles, configuration, cacheService);
        var refreshToken = TokenHelper.CreateRefresh(findRefreshToken.UserId, configuration, cacheService);

        // 5. Eliminar refresh token antiguo
        cacheService.Delete(CacheHelper.AuthRefreshTokenKey(model.RefreshToken));

        // 6. Responder
        return ResponseHelper.Create(new LoginAuthResponse
        {
            Token = token,
            RefreshToken = refreshToken
        });
    }
}
