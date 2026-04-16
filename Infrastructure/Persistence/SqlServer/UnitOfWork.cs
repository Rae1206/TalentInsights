using Twitter.Domain.Database.SqlServer.Context;
using Twitter.Domain.Database.SqlServer;
using Twitter.Domain.Interfaces.Repositories;

namespace Infrastructure.Persistence;

/// <summary>
/// Implementación del patrón Unit of Work.
/// Centraliza el acceso a datos y controla las transacciones.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly TwitterDbContext _context;

    public IUserRepository userRepository { get; set; }
    public IPostRepository postRepository { get; set; }
    public IAuthRepository authRepository { get; set; }
    public IRoleRepository roleRepository { get; set; }

    public UnitOfWork(
        TwitterDbContext context,
        IUserRepository userRepository,
        IPostRepository postRepository,
        IAuthRepository authRepository,
        IRoleRepository roleRepository)
    {
        _context = context;
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.authRepository = authRepository;
        this.roleRepository = roleRepository;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
