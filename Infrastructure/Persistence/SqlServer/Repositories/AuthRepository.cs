using Twitter.Domain.Database.SqlServer.Context;
using Twitter.Domain.Database.SqlServer.Entities;
using Twitter.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio de autenticación.
/// </summary>
public class AuthRepository(TwitterDbContext context) : IAuthRepository
{
    public User? GetByEmail(string email) =>
        context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefault(u => u.Email == email);

    public User? GetById(Guid id) =>
        context.Users.Find(id);

    public bool VerifyPassword(Guid userId, string password)
    {
        var entity = context.Users.Find(userId);
        if (entity is null) return false;

        return BCrypt.Net.BCrypt.Verify(password, entity.PasswordHash);
    }
}