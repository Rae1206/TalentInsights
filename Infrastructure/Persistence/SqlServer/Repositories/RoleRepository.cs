using Twitter.Domain.Database.SqlServer.Context;
using Twitter.Domain.Database.SqlServer.Entities;
using Twitter.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio de roles.
/// </summary>
public class RoleRepository(TwitterDbContext context) : IRoleRepository
{
    public Role? GetById(Guid roleId) =>
        context.Roles.Find(roleId);

    public Role? GetByName(string name) =>
        context.Roles.FirstOrDefault(r => r.Name == name);

    public List<Role> GetAll() =>
        context.Roles.Where(r => r.IsActive).ToList();

    public Guid? GetRoleIdByName(string roleName)
    {
        return context.Roles
            .Where(r => r.Name == roleName && r.IsActive)
            .Select(r => r.RoleId)
            .FirstOrDefault();
    }

    public List<Role> GetRolesByUserId(Guid userId)
    {
        return context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role)
            .ToList();
    }

    public string? GetPrimaryRoleName(Guid userId)
    {
        var userRole = context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .OrderBy(ur => ur.AssignedAt)
            .FirstOrDefault();
        
        return userRole?.Role?.Name;
    }
}