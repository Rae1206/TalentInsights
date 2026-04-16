# Patrón UnitOfWork con ServiceCollectionExtension en .NET

Este documento explica cómo implementar el patrón **Unit of Work** junto con **ServiceCollectionExtension** para manejar repositorios y servicios de forma escalable y mantenible en aplicaciones .NET.

---

## 1. Visión General del Patrón

El objetivo de este patrón es:

1. **Centralizar el acceso a datos**: Todas las operaciones de base de datos pasan por un único punto (Unit of Work).
2. **Controlar transacciones**: Permite guardar todos los cambios de una sola vez (`SaveChangesAsync`).
3. **Inyección de dependencias**: Usar el contenedor de servicios de .NET para gestionar el ciclo de vida de repositorios y servicios.
4. **Separación de responsabilidades**: El Domain define interfaces, Infrastructure las implementa, y Presentation las consume.

---

## 2. Estructura del Proyecto

La arquitectura sigue el patrón **Clean Architecture / Domain-Driven Design**:

```
MiProyecto/
├── MiProyecto.Domain/           # Capa de dominio (interfaces, entidades)
│   ├── Database/
│   │   └── SqlServer/
│   │       ├── IUnitOfWork.cs
│   │       └── Context/
│   │           └── MiDbContext.cs
│   └── Interfaces/
│       └── Repositories/
│           └── IRepositorioEntidad.cs
├── MiProyecto.Infrastructure/   # Capa de infraestructura (implementaciones)
│   └── Persistence/
│       └── SqlServer/
│           ├── UnitOfWork.cs
│           └── Repositories/
│               └── RepositorioEntidad.cs
└── MiProyecto.WebApi/           # Capa de presentación (Startup, Extensions)
    └── Extensions/
        └── ServiceCollectionExtension.cs
```

---

## 3. La Interfaz IUnitOfWork (Domain)

Ubicación: `MiProyecto.Domain/Database/SqlServer/IUnitOfWork.cs`

```csharp
using MiProyecto.Domain.Interfaces.Repositories;

namespace MiProyecto.Domain.Database.SqlServer
{
    public interface IUnitOfWork
    {
        ICollaboratorRepository collaboratorRepository { get; set; }
        // Agrega más repositorios según necesites:
        // IOtroRepositorio otroRepositorio { get; set; }
        
        Task SaveChangesAsync();
    }
}
```

**¿Por qué en Domain?**
- El dominio define **qué operaciones** existen, no **cómo** se implementan.
- Cualquier capa superior puede depender de esta interfaz sin conocer la implementación.

---

## 4. La Implementación UnitOfWork (Infrastructure)

Ubicación: `MiProyecto.Infrastructure/Persistence/SqlServer/UnitOfWork.cs`

```csharp
using MiProyecto.Domain.Database.SqlServer;
using MiProyecto.Domain.Database.SqlServer.Context;
using MiProyecto.Domain.Interfaces.Repositories;

namespace MiProyecto.Infrastructure.Persistence.SqlServer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MiDbContext _context;
        
        public ICollaboratorRepository collaboratorRepository { get; set; }
        
        // Inyección de dependencias por constructor
        public UnitOfWork(MiDbContext context, ICollaboratorRepository collaboratorRepository)
        {
            _context = context;
            this.collaboratorRepository = collaboratorRepository;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
```

**Puntos clave:**
- Recibe el `DbContext` de EF Core y los repositorios por inyección de dependencias.
- Un único `SaveChangesAsync()` para toda la unidad de trabajo.
- Si necesitas más repositorios, agrégalos como propiedades.

---

## 5. Interfaz del Repositorio (Domain)

Ubicación: `MiProyecto.Domain/Interfaces/Repositories/ICollaboratorRepository.cs`

```csharp
using MiProyecto.Domain.Database.SqlServer.Entities;

namespace MiProyecto.Domain.Interfaces.Repositories
{
    public interface ICollaboratorRepository
    {
        Task<Collaborator> Create(Collaborator collaborator);
        Task<Collaborator?> Get(Guid collaboratorId);
        Task<Collaborator?> Get(string email);
        IQueryable<Collaborator> Queryable();
        Task<bool> IfExists(Guid collaboratorId);
        Task<Collaborator> Update(Collaborator collaborator);
        
        // Otros métodos específicos del repositorio...
    }
}
```

---

## 6. Implementación del Repositorio (Infrastructure)

Ubicación: `MiProyecto.Infrastructure/Persistence/SqlServer/Repositories/CollaboratorRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using MiProyecto.Domain.Database.SqlServer.Context;
using MiProyecto.Domain.Database.SqlServer.Entities;
using MiProyecto.Domain.Interfaces.Repositories;

namespace MiProyecto.Infrastructure.Persistence.SqlServer.Repositories
{
    public class CollaboratorRepository : ICollaboratorRepository
    {
        private readonly MiDbContext _context;

        public CollaboratorRepository(MiDbContext context)
        {
            _context = context;
        }

        public async Task<Collaborator> Create(Collaborator collaborator)
        {
            await _context.Collaborators.AddAsync(collaborator);
            return collaborator;
        }

        public async Task<Collaborator?> Get(Guid collaboratorId)
        {
            return await _context.Collaborators
                .Include(c => c.Roles)
                .FirstOrDefaultAsync(x => x.Id == collaboratorId && x.DeletedAt == null);
        }

        public async Task<Collaborator> Update(Collaborator collaborator)
        {
            _context.Collaborators.Update(collaborator);
            return collaborator;
        }

        // ... otros métodos
    }
}
```

---

## 7. DbContext de Entity Framework Core

Ubicación: `MiProyecto.Domain/Database/SqlServer/Context/MiDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using MiProyecto.Domain.Database.SqlServer.Entities;

namespace MiProyecto.Domain.Database.SqlServer.Context
{
    public class MiDbContext : DbContext
    {
        public MiDbContext(DbContextOptions<MiDbContext> options) : base(options)
        {
        }

        public DbSet<Collaborator> Collaborators { get; set; }
        public DbSet<Role> Roles { get; set; }
        // Agrega más DbSets según tus entidades

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuraciones de entidades (Fluent API)
            modelBuilder.Entity<Collaborator>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired();
                // ...
            });
        }
    }
}
```

---

## 8. ServiceCollectionExtension (Capa de Presentación)

Ubicación: `MiProyecto.WebApi/Extensions/ServiceCollectionExtension.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using MiProyecto.Application.Interfaces.Services;
using MiProyecto.Application.Services;
using MiProyecto.Domain.Database.SqlServer;
using MiProyecto.Domain.Database.SqlServer.Context;
using MiProyecto.Domain.Interfaces.Repositories;
using MiProyecto.Infrastructure.Persistence.SqlServer;
using MiProyecto.Infrastructure.Persistence.SqlServer.Repositories;

namespace MiProyecto.WebApi.Extensions
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Registra todos los servicios de la aplicación
        /// </summary>
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICollaboratorService, CollaboratorService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IOtroService, OtroService>();
        }

        /// <summary>
        /// Registra todos los repositorios y el UnitOfWork
        /// </summary>
        public static void AddRepositories(this IServiceCollection services)
        {
            // Primero los repositorios individuales
            services.AddScoped<ICollaboratorRepository, CollaboratorRepository>();
            
            // Luego el UnitOfWork (que depende de los repositorios)
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        /// <summary>
        /// Método principal que configura toda la aplicación
        /// </summary>
        public static async Task AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Configurar DbContext con SQL Server
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? Environment.GetEnvironmentVariable("CONNECTION_STRING_DATABASE");

            services.AddDbContext<MiDbContext>(options =>
                options.UseSqlServer(connectionString));

            // 2. Registrar repositorios y servicios (orden mattera!)
            services.AddRepositories();
            services.AddServices();

            // 3. Otros configuraciones (Auth, Swagger, Middlewares, etc.)
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddAuthentication();
            services.AddAuthorization();
        }
    }
}
```

---

## 9. Cómo usar en el Startup o Program.cs

```csharp
// Program.cs (.NET 6+)
var builder = WebApplication.CreateBuilder(args);

// Agregar la configuración central
await builder.Services.AddCore(builder.Configuration);

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
```

O si usas `Startup.cs` (manera tradicional):

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCore(Configuration).GetAwaiter().Get();
    }
}
```

---

## 10. Cómo Usar UnitOfWork en un Servicio

Ubicación: `MiProyecto.Application/Services/CollaboratorService.cs`

```csharp
using MiProyecto.Domain.Database.SqlServer;
using MiProyecto.Domain.Interfaces.Repositories;

namespace MiProyecto.Application.Services
{
    public class CollaboratorService : ICollaboratorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CollaboratorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Collaborator> CreateCollaborator(Collaborator collaborator)
        {
            // Usar el repositorio a través del UnitOfWork
            var created = await _unitOfWork.collaboratorRepository.Create(collaborator);
            
            // Guardar todos los cambios
            await _unitOfWork.SaveChangesAsync();
            
            return created;
        }

        public async Task<Collaborator?> GetCollaborator(Guid id)
        {
            return await _unitOfWork.collaboratorRepository.Get(id);
        }
    }
}
```

---

## 11. Diagrama de Flujo de la Inyección de Dependencias

```
┌─────────────────────────────────────────────────────────────────┐
│                    ServiceCollection                            │
│  (AddCore → AddRepositories → AddServices)                     │
└──────────────────────────┬──────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────────┐
│  services.AddScoped<IUnitOfWork, UnitOfWork>                  │
│  services.AddScoped<ICollaboratorRepository,                   │
│                      CollaboratorRepository>                   │
│  services.AddScoped<ICollaboratorService,                     │
│                      CollaboratorService>                       │
└──────────────────────────┬──────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────────┐
│  Cuando se solicita un Controller / Servicio:                  │
│                                                                 │
│  1. EF Core crea MiDbContext ( scoped )                        │
│  2. Crea CollaboratorRepository inyectándole el DbContext      │
│  3. Crea UnitOfWork inyectándole el DbContext y                │
│     el CollaboratorRepository                                   │
│  4. Crea el Servicio inyectándole el UnitOfWork                │
└─────────────────────────────────────────────────────────────────┘
```

---

## 12. Beneficios de Este Patrón

| Beneficio | Descripción |
|-----------|-------------|
| **Acoplamiento débil** | Las capas superiores dependen de abstracciones (interfaces), no de implementaciones concretas. |
| **Testabilidad** | Puedes inyectar implementaciones mock de repositorios y servicios para testing unitario. |
| **Centralización** | Un único punto de entrada (`IUnitOfWork`) para todas las operaciones de base de datos. |
| **Transacciones** | Un solo `SaveChangesAsync()` para atomicidad de operaciones. |
| **Mantenibilidad** | Agregar nuevos repositorios es trivial: solo añadir interfaz, implementación, y registrarlos. |

---

## 13. Checklist para Replicar en Otro Proyecto

- [ ] **Domain Layer**
  - [ ] Crear interfaz `IUnitOfWork` con las propiedades de repositorios y el método `SaveChangesAsync()`
  - [ ] Crear interfaz de cada repositorio (`IRepositorioEntidad`)
  - [ ] Crear el `DbContext` con todas las entidades

- [ ] **Infrastructure Layer**
  - [ ] Implementar cada repositorio (`RepositorioEntidad`)
  - [ ] Implementar `UnitOfWork` inyectando el DbContext y todos los repositorios

- [ ] **Presentation / WebApi Layer**
  - [ ] Crear `ServiceCollectionExtension.cs`
  - [ ] Método `AddRepositories()` → registra repositorios + UnitOfWork
  - [ ] Método `AddServices()` → registra servicios
  - [ ] Método `AddCore()` → configura DbContext y llama a los anteriores
  - [ ] Llamar `services.AddCore()` en `Program.cs` o `Startup.cs`

- [ ] **Uso en Servicios**
  - [ ] Inyectar `IUnitOfWork` en el constructor del servicio
  - [ ] Usar `_unitOfWork.repositorioMetodo()` para operaciones
  - [ ] Llamar `_unitOfWork.SaveChangesAsync()` al finalizar operaciones

---

## 14. Ejemplo Completo con Múltiples Repositorios

### IUnitOfWork con múltiples entidades:

```csharp
public interface IUnitOfWork
{
    ICollaboratorRepository collaboratorRepository { get; set; }
    IOrderRepository orderRepository { get; set; }
    IProductRepository productRepository { get; set; }
    
    Task SaveChangesAsync();
}
```

### UnitOfWork con múltiples repositorios:

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly MiDbContext _context;
    
    public ICollaboratorRepository collaboratorRepository { get; set; }
    public IOrderRepository orderRepository { get; set; }
    public IProductRepository productRepository { get; set; }
    
    public UnitOfWork(
        MiDbContext context,
        ICollaboratorRepository collaboratorRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _context = context;
        this.collaboratorRepository = collaboratorRepository;
        this.orderRepository = orderRepository;
        this.productRepository = productRepository;
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
```

### ServiceCollectionExtension:

```csharp
public static void AddRepositories(this IServiceCollection services)
{
    services.AddScoped<ICollaboratorRepository, CollaboratorRepository>();
    services.AddScoped<IOrderRepository, OrderRepository>();
    services.AddScoped<IProductRepository, ProductRepository>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
}
```

---

## 15. Notas Importantes

1. **Orden de registro**: Siempre registra primero los repositorios individuales, luego el `UnitOfWork`, y finalmente los servicios que dependen de él.

2. **Scoped vs Singleton vs Transient**:
   - `Scoped`: Recomendado para repositorios y servicios porque viven durante toda la request HTTP.
   - `Singleton`: Solo para servicios que no dependen de estado por request (como configuration).
   - `Transient`: Para servicios ligeros sin estado.

3. **DbContext es scoped por defecto**: No necesitas registrarlo manualmente si usas `AddDbContext<T>()`, .NET lo maneja automáticamente como scoped.

4. **Transacciones implícitas**: EF Core maneja transacciones automáticamente. Si llamas `SaveChangesAsync()` una vez al final de la operación, todas las cambios se aplican atómicamente.

---

## Conclusión

Este patrón proporciona una arquitectura limpia, mantenible y testeable para aplicaciones .NET que necesitan acceso a datos. El `IUnitOfWork` actúa como el orquestador central que agrupa todas las operaciones de repositorio, mientras que `ServiceCollectionExtension` configura todo el contenedor de dependencias de forma centralizada y ordenada.
