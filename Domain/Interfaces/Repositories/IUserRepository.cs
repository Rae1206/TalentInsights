using Twitter.Domain.Database.SqlServer.Entities;

namespace Twitter.Domain.Interfaces.Repositories;

/// <summary>
/// Interfaz del repositorio de usuarios.
/// </summary>
public interface IUserRepository
{
    User Create(User user);
    User? GetById(Guid userId);
    User? GetByEmail(string email);
    List<User> GetAll(int limit, int offset, string? fullName = null, string? email = null);
    User? Update(Guid userId, User user);
    bool Delete(Guid userId);
    bool ExistsByEmail(string email);
    bool ChangePassword(Guid userId, string newPassword);
}