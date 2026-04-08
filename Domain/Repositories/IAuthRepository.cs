using Domain.Entities;

namespace Domain.Repositories;

public interface IAuthRepository
{
    User? GetByEmail(string email);
    bool VerifyPassword(Guid userId, string password);
}
