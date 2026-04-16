using Twitter.Domain.Database.SqlServer.Entities;

namespace Twitter.Domain.Interfaces.Repositories;

/// <summary>
/// Interfaz para el repositorio de roles.
/// </summary>
public interface IRoleRepository
{
    Role? GetById(Guid roleId);
    Role? GetByName(string name);
    List<Role> GetAll();
    Guid? GetRoleIdByName(string roleName);
    List<Role> GetRolesByUserId(Guid userId);
    string? GetPrimaryRoleName(Guid userId);
}