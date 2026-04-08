using Application.Models.DTOs;
using Application.Models.Requests.User;
using Application.Models.Responses;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    LoginResponse Login(LoginUserRequest model, string jwtSecretKey);
}
