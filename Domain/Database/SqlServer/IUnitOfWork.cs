using Twitter.Domain.Interfaces.Repositories;

namespace Twitter.Domain.Database.SqlServer;

/// <summary>
/// Interfaz que define el patrón Unit of Work.
/// Centraliza el acceso a datos y controla las transacciones.
/// </summary>
public interface IUnitOfWork
{
    IUserRepository userRepository { get; set; }
    IPostRepository postRepository { get; set; }
    IAuthRepository authRepository { get; set; }
    IRoleRepository roleRepository { get; set; }

    /// <summary>
    /// Guarda todos los cambios pendientes en la base de datos.
    /// </summary>
    Task SaveChangesAsync();
}