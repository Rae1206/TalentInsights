using Domain.Entities;

namespace Domain.Repositories;

public interface IUserRepository
{
    User Create(User user);
    User? GetById(Guid userId);
    User? GetByEmail(string email);
    List<User> GetAll(int limit, int offset, string? fullName = null, string? email = null);
    User? Update(Guid userId, User user);
    bool Delete(Guid userId);
    bool ChangePassword(Guid userId, string newPassword);
    bool Exists(Guid userId);
    bool ExistsByEmail(string email);
    bool VerifyPassword(Guid userId, string password);
}
