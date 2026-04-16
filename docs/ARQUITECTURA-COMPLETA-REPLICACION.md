# Arquitectura Completa de TalentInsights - Replicación Exacta

Este documento describe la estructura completa del proyecto para replicar en otros proyectos .NET 9 con Clean Architecture.

---

## 1. Estructura General del Proyecto (Solution)

```
TalentInsights.slnx
│
├── TalentInsights.Domain/           # Entidades, interfaces de repositorio, excepciones
├── TalentInsights.Application/      # Lógica de negocio, servicios, DTOs, helpers
├── TalentInsights.Infrastructure/   # Implementación de repositorios, EF Core, Unit of Work
├── TalentInsights.WebApi/           # Controllers, Middlewares, Program.cs
└── TalentInsights.Shared/           # Constantes, helpers genéricos, utilidades
```

---

## 2. Capa: TalentInsights.Domain

**Propósito**: Definir entidades, interfaces de repositorio, y excepciones del dominio. NO tiene dependencias de otras capas.

### 2.1 Estructura de Carpetas

```
TalentInsights.Domain/
├── Database/
│   └── SqlServer/
│       ├── Entities/                    # Entidades de base de datos
│       │   ├── User.cs
│       │   ├── Role.cs
│       │   ├── Permission.cs
│       │   ├── Collaborator.cs          # Entidad principal de usuarios
│       │   ├── CollaboratorRole.cs
│       │   ├── CollaboratorPermission.cs
│       │   ├── CollaboratorSkill.cs
│       │   ├── Team.cs
│       │   ├── TeamMember.cs
│       │   ├── Project.cs
│       │   ├── ProjectCollaborator.cs
│       │   ├── ProjectMessage.cs
│       │   ├── Post.cs
│       │   ├── Menu.cs
│       │   ├── MenuPermission.cs
│       │   ├── Skill.cs
│       │   ├── UsersRole.cs
│       │   └── RolePermission.cs
│       └── Context/
│           └── TalentInsightsContext.cs  # DbContext de EF Core
├── Interfaces/
│   └── Repositories/                    # Interfaces de repositorios
│       └── ICollaboratorRepository.cs
├── Exceptions/                           # Excepciones personalizadas
│   ├── BadRequestException.cs
│   ├── NotFoundException.cs
│   └── UnauthorizedException.cs
└── IUnitOfWork.cs                       # Interfaz Unit of Work
```

### 2.2 Detalle de Archivos Importantes

#### IUnitOfWork.cs
```csharp
// Ubicación: TalentInsights.Domain/Database/SqlServer/IUnitOfWork.cs
using TalentInsights.Domain.Interfaces.Repositories;

namespace TalentInsights.Domain.Database.SqlServer
{
    public interface IUnitOfWork
    {
        ICollaboratorRepository collaboratorRepository { get; set; }
        Task SaveChangesAsync();
    }
}
```

#### ICollaboratorRepository.cs
```csharp
// Ubicación: TalentInsights.Domain/Interfaces/Repositories/ICollaboratorRepository.cs
using TalentInsights.Domain.Database.SqlServer.Entities;

namespace TalentInsights.Domain.Interfaces.Repositories
{
    public interface ICollaboratorRepository
    {
        Task<Collaborator> Create(Collaborator collaborator);
        Task<Collaborator?> Get(Guid collaboratorId);
        Task<Collaborator?> Get(string email);
        IQueryable<Collaborator> Queryable();
        Task<bool> IfExists(Guid collaboratorId);
        Task<Collaborator> Update(Collaborator collaborator);
        Task<bool> HasCreated();

        // Roles
        Task<Role?> GetRole(string name);
        Task<Role?> GetRole(Guid id);
    }
}
```

#### Excepciones (todas en TalentInsights.Domain/Exceptions/)

```csharp
// BadRequestException.cs
namespace TalentInsights.Domain.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }
}

// NotFoundException.cs
namespace TalentInsights.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}

// UnauthorizedException.cs
namespace TalentInsights.Domain.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
```

---

## 3. Capa: TalentInsights.Shared

**Propósito**: Código compartido entre todas las capas. Constantes, helpers genéricos, utilidades de hashing, generación de texto, cache base, SMTP.

### 3.1 Estructura de Carpetas

```
TalentInsights.Shared/
├── Constants/                           # Constantes centralizadas
│   ├── ValidationConstants.cs          # Mensajes de validación
│   ├── ConfigurationConstants.cs        # Claves de configuración appsettings
│   ├── ClaimsConstants.cs               # Nombres de claims JWT
│   ├── RoleConstants.cs                 # Nombres de roles del sistema
│   └── ResponseConstants.cs             # Mensajes de respuesta predefinidos
├── Helpers/                            # Helpers estáticos genéricos
│   └── DateTimeHelper.cs                # Manejo de fechas UTC
├── Cache.cs                            # Cache genérico en memoria (utilidad básica)
├── Hasher.cs                           # Utilidad para hashing/verificación de contraseñas
├── Generate.cs                         # Utilidad para generar textos aleatorios
└── SMTP.cs                             # Utilidad para envío de emails
```

### 3.2 Detalle de Archivos

#### Constants/ValidationConstants.cs
```csharp
namespace TalentInsights.Shared.Constants
{
    public static class ValidationConstants
    {
        public const string MAX_LENGTH = "El máximo de caracteres de {0} es {1}";
        public const string MIN_LENGTH = "El mínimo de caracteres de {0} es {1}";
        public const string REQUIRED = "La propiedad {0} es requerida";
        public const string EMAIL_ADDRESS = "La dirección de correo electrónico, no es correcta {0}";
        public const string VALIDATION_MESSAGE = "Una o más validaciones necesitan atención";
    }
}
```

#### Constants/ConfigurationConstants.cs
```csharp
namespace TalentInsights.Shared.Constants
{
    public static class ConfigurationConstants
    {
        // First app - usuario inicial
        public const string FIRST_APP_TIME_USER_FULLNAME = "FirstAppTime:User:FullName";
        public const string FIRST_APP_TIME_USER_EMAIL = "FirstAppTime:User:Email";
        public const string FIRST_APP_TIME_USER_PASSWORD = "FirstAppTime:User:Password";
        public const string FIRST_APP_TIME_USER_POSITION = "FirstAppTime:User:Position";

        // Connection strings
        public const string CONNECTION_STRING_DATABASE = "ConnectionStrings:Database";

        // JWT
        public const string JWT_PRIVATE_KEY = "Jwt:PrivateKey";
        public const string JWT_AUDIENCE = "Jwt:Audience";
        public const string JWT_ISSUER = "Jwt:Issuer";
        public const string JWT_EXPIRATION_IN_MINUTES_MIN = "Jwt:ExpirationInMinutesMin";
        public const string JWT_EXPIRATION_IN_MINUTES_MAX = "Jwt:ExpirationInMinutesMax";

        // Auth
        public const string AUTH_REFRESH_TOKEN_EXPIRATION_IN_DAYS = "Auth:RefreshToken:ExpirationInDays";

        // SMTP
        public const string SMTP_HOST = "SMTP:Host";
        public const string SMTP_PORT = "SMTP:Port";
        public const string SMTP_USER = "SMTP:User";
        public const string SMTP_PASSWORD = "SMTP:Password";
        public const string SMTP_FROM = "SMTP:From";
    }
}
```

#### Constants/ClaimsConstants.cs
```csharp
namespace TalentInsights.Shared.Constants
{
    public static class ClaimsConstants
    {
        public const string COLLABORATOR_ID = "CollaboratorId";
    }
}
```

#### Constants/RoleConstants.cs
```csharp
namespace TalentInsights.Shared.Constants
{
    public static class RoleConstants
    {
        public const string Developer = "Developer";
        public const string Admin = "Admin";
        public const string TeamLeader = "TeamLeader";
        public const string HR = "HR";
    }
}
```

#### Constants/ResponseConstants.cs
```csharp
namespace TalentInsights.Shared.Constants
{
    public static class ResponseConstants
    {
        public const string COLLABORATOR_NOT_EXISTS = "El colaborador no existe";

        public static string RoleNotFound(string name) => $"El rol {name} no existe";
        public static string RoleNotFound(Guid id) => $"El rol con ID: {id} no existe";

        public const string PROJECT_NOT_EXISTS = "El proyecto no existe";

        public const string AUTH_TOKEN_NOT_FOUND = "El token no es correcto, expiró o no se argumentó";
        public const string AUTH_USER_OR_PASSWORD_NOT_FOUND = "Usuario o contraseña incorrectos";
        public const string AUTH_REFRESH_TOKEN_NOT_FOUND = "El token para refrescar la sesión expiró, no existe o es incorrecto";

        public static string ErrorUnexpected(string traceId)
        {
            return $"Ha ocurrido un error inesperado: Contacto con soporte, mencionando el siguiente código de error: {traceId}";
        }

        public static string ConfigurationPropertyNotFound(string property)
        {
            return $"Falta la propiedad '{property}' por establecer en la configuración del aplicativo.";
        }
    }
}
```

#### Helpers/DateTimeHelper.cs
```csharp
namespace TalentInsights.Shared.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime UtcNow()
        {
            return DateTimeOffset.UtcNow.DateTime;
        }
    }
}
```

#### Hasher.cs
```csharp
// Ubicación: TalentInsights.Shared/Hasher.cs
// Utilidad para hashing de contraseñas usando BCrypt o similar
```

#### Generate.cs
```csharp
// Ubicación: TalentInsights.Shared/Generate.cs
// Utilidad para generar textos aleatorios (ej: refresh tokens)
```

#### Cache.cs
```csharp
// Ubicación: TalentInsights.Shared/Cache.cs
// Cache genérico en memoria (clase base)
```

---

## 4. Capa: TalentInsights.Application

**Propósito**: Lógica de negocio, servicios, DTOs, modelos de request/response, helpers de aplicación.

### 4.1 Estructura de Carpetas

```
TalentInsights.Application/
├── Interfaces/
│   └── Services/                       # Interfaces de servicios (DI)
│       ├── IAuthService.cs
│       ├── ICacheService.cs
│       └── ICollaboratorService.cs
├── Services/                           # Implementaciones de servicios
│   ├── AuthService.cs
│   ├── CacheService.cs
│   └── CollaboratorService.cs
├── Helpers/                            # Helpers de aplicación
│   ├── TokenHelper.cs                  # Generación JWT + Refresh Tokens
│   ├── CacheHelper.cs                  # Generación de claves de cache
│   └── ResponseHelper.cs              # Helpers para respuestas genéricas
├── Models/
│   ├── Requests/
│   │   ├── BaseRequest.cs
│   │   └── Auth/
│   │       ├── LoginAuthRequest.cs
│   │       └── RenewAuthRequest.cs
│   │   └── Collaborator/
│   │       ├── CreateCollaboratorRequest.cs
│   │       ├── UpdateCollaboratorRequest.cs
│   │       ├── ChangePasswordCollaboratorRequest.cs
│   │       └── FilterColaboratorRequest.cs
│   ├── Responses/
│   │   ├── GenericResponse.cs         # Wrapper genérico de respuesta
│   │   └── Auth/
│   │       └── LoginAuthResponse.cs
│   ├── DTOs/
│   │   └── CollaboratorDto.cs
│   └── Helpers/
│       ├── TokenConfiguration.cs      # Configuración para crear JWT
│       ├── RefreshToken.cs            # Modelo para guardar refresh token en cache
│       └── CacheKey.cs                # Modelo para clave de cache
```

### 4.2 Detalle de Archivos Importantes

#### Interfaces/Services/IAuthService.cs
```csharp
using TalentInsights.Application.Models.Requests.Auth;
using TalentInsights.Application.Models.Responses;
using TalentInsights.Application.Models.Responses.Auth;

namespace TalentInsights.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<GenericResponse<LoginAuthResponse>> Login(LoginAuthRequest model);
        Task<GenericResponse<LoginAuthResponse>> Renew(RenewAuthRequest model);
    }
}
```

#### Interfaces/Services/ICacheService.cs
```csharp
namespace TalentInsights.Application.Interfaces.Services
{
    public interface ICacheService
    {
        public T Create<T>(string key, TimeSpan expiration, T value);
        public T? Get<T>(string key);
        public bool Delete(string key);
    }
}
```

#### Interfaces/Services/ICollaboratorService.cs
```csharp
using TalentInsights.Application.Models.DTOs;
using TalentInsights.Application.Models.Requests.Collaborator;
using TalentInsights.Application.Models.Responses;

namespace TalentInsights.Application.Interfaces.Services
{
    public interface ICollaboratorService
    {
        public Task<GenericResponse<CollaboratorDto>> Create(CreateCollaboratorRequest model);
        public Task<GenericResponse<CollaboratorDto>> Update(Guid collaboratorId, UpdateCollaboratorRequest model);
        public GenericResponse<List<CollaboratorDto>> Get(FilterColaboratorRequest model);
        public Task<GenericResponse<CollaboratorDto>> Get(Guid collaboratorId);
        public Task<GenericResponse<bool>> Delete(Guid collaboratorId);
        public Task CreateFirstUser();
    }
}
```

#### Services/CacheService.cs
```csharp
namespace TalentInsights.Application.Services
{
    public class CacheService(IMemoryCache memoryCache) : ICacheService
    {
        public T Create<T>(string key, TimeSpan expiration, T value)
        {
            var create = memoryCache.GetOrCreate(key, (factory) =>
            {
                factory.SlidingExpiration = expiration;
                return value;
            });
            return create is null ? throw new Exception("No se pudo establecer la caché") : create;
        }

        public bool Delete(string key)
        {
            memoryCache.Remove(key);
            return true;
        }

        public T? Get<T>(string key)
        {
            return memoryCache.Get<T>(key);
        }
    }
}
```

#### Services/AuthService.cs
```csharp
// Ubicación: TalentInsights.Application/Services/AuthService.cs
// Implementa IAuthService
// Maneja Login y Renew (refresh token)
```

#### Helpers/TokenHelper.cs
```csharp
// Ubicación: TalentInsights.Application/Helpers/TokenHelper.cs
// Corazón del sistema de autenticación
// Métodos:
// - Create(Guid collaboratorId, List<string> roles, IConfiguration configuration, ICacheService cache)
// - CreateRefresh(Guid collaboratorId, IConfiguration configuration, ICacheService cacheService)
// - Configuration(IConfiguration configuration)
```

#### Helpers/CacheHelper.cs
```csharp
namespace TalentInsights.Application.Helpers
{
    public static class CacheHelper
    {
        public static string AuthTokenKey(string value)
        {
            return $"auth:tokens:{value}";
        }

        public static CacheKey AuthTokenCreation(string value, TimeSpan expiration)
        {
            return new CacheKey
            {
                Key = AuthTokenKey(value),
                Expiration = expiration
            };
        }

        public static string AuthRefreshTokenKey(string value)
        {
            return $"auth:refresh_tokens:{value}";
        }

        public static CacheKey AuthRefreshTokenCreation(string value, IConfiguration configuration)
        {
            return new CacheKey
            {
                Key = AuthRefreshTokenKey(value),
                Expiration = TimeSpan.FromDays(Convert.ToInt32(configuration[ConfigurationConstants.AUTH_REFRESH_TOKEN_EXPIRATION_IN_DAYS] ?? "15"))
            };
        }
    }
}
```

#### Helpers/ResponseHelper.cs
```csharp
namespace TalentInsights.Application.Helpers
{
    public static class ResponseHelper
    {
        public static GenericResponse<T> Create<T>(T data, List<string>? errors = null, string? message = null)
        {
            var response = new GenericResponse<T>
            {
                Data = data,
                Message = message ?? "Solicitud realizada correctamente",
                Errors = errors ?? []
            };

            return response;
        }
    }
}
```

#### Models/Responses/GenericResponse.cs
```csharp
namespace TalentInsights.Application.Models.Responses
{
    public class GenericResponse<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = [];
    }
}
```

#### Models/Responses/Auth/LoginAuthResponse.cs
```csharp
namespace TalentInsights.Application.Models.Responses.Auth
{
    public class LoginAuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
```

#### Models/Helpers/TokenConfiguration.cs
```csharp
using Microsoft.IdentityModel.Tokens;

namespace TalentInsights.Application.Models.Helpers
{
    public class TokenConfiguration
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required SymmetricSecurityKey SecurityKey { get; set; }
        public required DateTime Expiration { get; set; }
        public required TimeSpan ExpirationTimeSpan { get; set; }
    }
}
```

#### Models/Helpers/RefreshToken.cs
```csharp
namespace TalentInsights.Application.Models.Helpers
{
    public class RefreshToken
    {
        public required Guid CollaboratorId { get; set; }
        public required TimeSpan ExpirationInDays { get; set; }
    }
}
```

#### Models/Helpers/CacheKey.cs
```csharp
namespace TalentInsights.Application.Models.Helpers
{
    public class CacheKey
    {
        public required string Key { get; set; }
        public required TimeSpan Expiration { get; set; }
    }
}
```

---

## 5. Capa: TalentInsights.Infrastructure

**Propósito**: Implementación de repositorios, DbContext, Unit of Work.

### 5.1 Estructura de Carpetas

```
TalentInsights.Infrastructure/
├── Persistence/
│   └── SqlServer/
│       ├── Repositories/
│       │   └── CollaboratorRepository.cs
│       └── UnitOfWork.cs
└── TalentInsights.Infrastructure.csproj
```

### 5.2 Detalle de Archivos

#### Persistence/SqlServer/Repositories/CollaboratorRepository.cs
```csharp
// Implementación de ICollaboratorRepository
// Usa TalentInsightsContext (EF Core)
```

#### Persistence/SqlServer/UnitOfWork.cs
```csharp
// Implementación de IUnitOfWork
// Expone collaboratorRepository
// Implementa SaveChangesAsync()
```

---

## 6. Capa: TalentInsights.WebApi

**Propósito**: Controllers, Middlewares, configuración de servicios (Program.cs).

### 6.1 Estructura de Carpetas

```
TalentInsights.WebApi/
├── Controllers/
│   ├── AuthController.cs
│   └── CollaboratorsController.cs
├── Middlewares/
│   └── ErrorHandlerMiddleware.cs
├── Extensions/
│   └── ServiceCollectionExtension.cs
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
└── TalentInsights.WebApi.csproj
```

### 6.2 Detalle de Archivos

#### Extensions/ServiceCollectionExtension.cs
```csharp
// Registro de todos los servicios en DI
// Configuración de JWT, Cache, Repositorios, Unit of Work, Servicios
```

#### Controllers/AuthController.cs
```csharp
// Endpoints:
// POST /api/auth/login
// POST /api/auth/renew
```

#### Middlewares/ErrorHandlerMiddleware.cs
```csharp
// Manejo centralizado de excepciones
```

---

## 7. Resumen de Arquitectura - Ubicaciones Exactas

| Componente | Ubicación | Propósito |
|------------|-----------|-----------|
| **CONSTANTS** | `TalentInsights.Shared/Constants/` | Valores centralizados |
| ValidationConstants | `Shared/Constants/ValidationConstants.cs` | Mensajes de validación |
| ConfigurationConstants | `Shared/Constants/ConfigurationConstants.cs` | Claves de appsettings |
| ClaimsConstants | `Shared/Constants/ClaimsConstants.cs` | Nombres de claims JWT |
| RoleConstants | `Shared/Constants/RoleConstants.cs` | Nombres de roles |
| ResponseConstants | `Shared/Constants/ResponseConstants.cs` | Mensajes de respuesta |
| **HELPERS** | | |
| DateTimeHelper | `Shared/Helpers/DateTimeHelper.cs` | Fechas UTC |
| ResponseHelper | `Application/Helpers/ResponseHelper.cs` | Crear respuestas genéricas |
| CacheHelper | `Application/Helpers/CacheHelper.cs` | Generar claves de cache |
| TokenHelper | `Application/Helpers/TokenHelper.cs` | Generar JWT y refresh tokens |
| **SERVICIOS** | `TalentInsights.Application/Services/` | |
| ICacheService / CacheService | `Application/Interfaces/Services/` + `Application/Services/` | Abstracción de IMemoryCache |
| IAuthService / AuthService | `Application/Interfaces/Services/` + `Application/Services/` | Lógica de autenticación |
| ICollaboratorService / CollaboratorService | `Application/Interfaces/Services/` + `Application/Services/` | Lógica de colaboradores |
| **MODELOS** | `TalentInsights.Application/Models/` | |
| GenericResponse | `Models/Responses/GenericResponse.cs` | Wrapper de respuesta |
| LoginAuthResponse | `Models/Responses/Auth/LoginAuthResponse.cs` | Respuesta de login |
| TokenConfiguration | `Models/Helpers/TokenConfiguration.cs` | Configuración JWT |
| RefreshToken | `Models/Helpers/RefreshToken.cs` | Modelo refresh token |
| CacheKey | `Models/Helpers/CacheKey.cs` | Modelo clave de cache |
| LoginAuthRequest | `Models/Requests/Auth/LoginAuthRequest.cs` | Request de login |
| RenewAuthRequest | `Models/Requests/Auth/RenewAuthRequest.cs` | Request de renew |
| **DOMINIO** | `TalentInsights.Domain/` | |
| IUnitOfWork | `Domain/Database/SqlServer/IUnitOfWork.cs` | Interfaz UoW |
| ICollaboratorRepository | `Domain/Interfaces/Repositories/ICollaboratorRepository.cs` | Interfaz repositorio |
| Excepciones | `Domain/Exceptions/` | Excepciones personalizadas |
| Entidades | `Domain/Database/SqlServer/Entities/` | Entidades EF Core |
| DbContext | `Domain/Database/SqlServer/Context/TalentInsightsContext.cs` | Contexto EF Core |
| **INFRAESTRUCTURA** | `TalentInsights.Infrastructure/` | |
| UnitOfWork | `Infrastructure/Persistence/SqlServer/UnitOfWork.cs` | Implementación UoW |
| Repositorios | `Infrastructure/Persistence/SqlServer/Repositories/` | Implementación repositorios |
| **WEB API** | `TalentInsights.WebApi/` | |
| Controllers | `WebApi/Controllers/` | Controladores API |
| Middlewares | `WebApi/Middlewares/` | Middlewares |
| ServiceCollectionExtension | `WebApi/Extensions/ServiceCollectionExtension.cs` | Registro de servicios |

---

## 8. appsettings.json - Configuración Necesaria

```json
{
  "ConnectionStrings": {
    "Database": "Server=.;Database=TalentInsights;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "PrivateKey": "TuClaveSecretaDeAlMenos32Caracteres123456!",
    "Audience": "TalentInsights API",
    "Issuer": "TalentInsights",
    "ExpirationInMinutesMin": "1",
    "ExpirationInMinutesMax": "5"
  },
  "Auth": {
    "RefreshToken": {
      "ExpirationInDays": "15"
    }
  },
  "FirstAppTime": {
    "User": {
      "FullName": "Admin",
      "Email": "admin@talentinsights.com",
      "Password": "Admin123!",
      "Position": "Administrator"
    }
  },
  "SMTP": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "User": "tuemail@gmail.com",
    "Password": "tuapppassword",
    "From": "noreply@talentinsights.com"
  }
}
```

---

## 9. Program.cs / ServiceCollectionExtension - Registro de Servicios

```csharp
// En ServiceCollectionExtension.cs o Program.cs:

// Memory Cache
builder.Services.AddMemoryCache();

// Cache Service
builder.Services.AddScoped<ICacheService, CacheService>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repositorios
builder.Services.AddScoped<ICollaboratorRepository, CollaboratorRepository>();

// Servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICollaboratorService, CollaboratorService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:PrivateKey"]!)
            )
        };
    });
```

---

## 10. Paquetes NuGet Requeridos

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.0" />
<PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="9.0.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

---

## 11. Convenciones de Código

1. **Interfaces de servicios**: Prefijo `I` (ej: `IAuthService`)
2. **Interfaces de repositorios**: Prefijo `I` en carpeta `Interfaces/Repositories`
3. **Clases de constantes**: Nombre ending en `Constants`, miembros `public const string`
4. **Helpers estáticos**: Nombre ending en `Helper`, métodos estáticos
5. **Modelos DTO**: Sufijo `Dto` (ej: `CollaboratorDto`)
6. **Modelos Request**: Sufijo `Request` (ej: `LoginAuthRequest`)
7. **Modelos Response**: Carpeta `Responses`, suffix `Response`
8. **Servicios**: Implementan interfaz, ubicados en `Services/`
9. **Excepciones personalizadas**: Ubicadas en `Domain/Exceptions/`
10. **Entidades**: Ubicadas en `Domain/Database/SqlServer/Entities/`

---

## 12. Flujo de Autenticación (Resumen)

```
1. POST /api/auth/login {email, password}
       ↓
2. AuthController → AuthService.Login()
       ↓
3. Buscar collaborator por email
       ↓
4. Validar contraseña con Hasher.ComparePassword()
       ↓
5. Obtener roles del collaborator
       ↓
6. TokenHelper.Create() → genera JWT + guarda en cache
       ↓
7. TokenHelper.CreateRefresh() → genera refresh token + guarda en cache
       ↓
8. Return { token, refreshToken }

---

RENEW:
1. POST /api/auth/renew {refreshToken}
       ↓
2. Buscar refresh token en cache
       ↓
3. Si existe → obtener collaboratorId
       ↓
4. Generar nuevos JWT + refresh token
       ↓
5. Eliminar refresh token antiguo
       ↓
6. Return { nuevoToken, nuevoRefreshToken }
```

---

*Documento generado para replicación exacta en nuevos proyectos .NET 9 con Clean Architecture.*