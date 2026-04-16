using Twitter.Domain.Database.SqlServer.Entities;

namespace Twitter.Domain.Interfaces.Repositories;

/// <summary>
/// Interfaz del repositorio de autenticación.
/// </summary>
public interface IAuthRepository
{
    User? GetByEmail(string email);
    User? GetById(Guid id);
    bool VerifyPassword(Guid userId, string password);
}