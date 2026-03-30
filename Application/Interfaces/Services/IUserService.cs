using Application.Models.DTOs;
using Application.Models.Requests.User;
using Application.Models.Responses;

namespace Application.Interfaces.Services;

public interface IUserService
{
    GenericResponse<UserDto> Create(CreateUserRequest model);
    GenericResponse<UserDto> Update(Guid userId, UpdateUserRequest model);
    GenericResponse<List<UserDto>> Get(int limit, int offset);
    GenericResponse<UserDto?> Get(Guid userId);
    GenericResponse<bool> ChangePassword(Guid userId, ChangePasswordUserRequest model);
    GenericResponse<bool> Delete(Guid userId);
}
