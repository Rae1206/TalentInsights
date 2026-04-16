# Arquitectura de Autenticación, Helpers y Cache en TalentInsights

Este documento detalla la arquitectura implementada en el proyecto para replicarla en otros proyectos .NET. La arquitectura está diseñada con separación de responsabilidades siguiendo principios de Clean Architecture.

---

## 1. Estructura del Proyecto

```
TalentInsights/
├── TalentInsights.Shared/           # Código compartido entre capas
│   ├── Constants/                   # Constantes centralizadas
│   │   ├── ValidationConstants.cs
│   │   ├── ClaimsConstants.cs
│   │   ├── ConfigurationConstants.cs
│   │   ├── ResponseConstants.cs
│   │   └── RoleConstants.cs
│   ├── Helpers/
│   │   └── DateTimeHelper.cs
│   ├── Cache.cs                     # Cache genérico en memoria
│   ├── Hasher.cs                    # Utilidad para hashing de contraseñas
│   └── Generate.cs                  # Utilidad para generar textos aleatorios
│
├── TalentInsights.Application/      # Capa de aplicación (lógica de negocio)
│   ├── Helpers/
│   │   ├── TokenHelper.cs           # Generación de JWT y Refresh Tokens
│   │   ├── CacheHelper.cs           # Generación de claves de cache
│   │   └── ResponseHelper.cs        # Helpers para respuestas genéricas
│   ├── Models/
│   │   └── Helpers/
│   │       ├── TokenConfiguration.cs
│   │       ├── RefreshToken.cs
│   │       └── CacheKey.cs
│   ├── Services/
│   │   ├── CacheService.cs          # Implementación del servicio de cache
│   │   └── AuthService.cs           # Lógica de autenticación
│   └── Interfaces/
│       └── Services/
│           └── ICacheService.cs     # Interfaz del servicio de cache
│
└── TalentInsights.WebApi/           # API REST (presentación)
    └── Controllers/
        └── AuthController.cs
```

---

## 2. Constantes (Constants)

Las constantes proporcionan un punto único de definición para valores que se usan en múltiples lugares del proyecto. Esto mejora el mantenimiento y la consistencia.

### 2.1 ValidationConstants

**Ubicación**: `TalentInsights.Shared/Constants/ValidationConstants.cs`

Define mensajes de validación centralizados.

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

**Uso en otro proyecto**:
```csharp
// En validators o directamente en código
throw new ValidationException(string.Format(ValidationConstants.MAX_LENGTH, "Nombre", 100));
```

### 2.2 ConfigurationConstants

**Ubicación**: `TalentInsights.Shared/Constants/ConfigurationConstants.cs`

Centraliza las claves de configuración del `appsettings.json` y variables de entorno. Esto permite cambiar el nombre de una configuración en un solo lugar.

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

**appsettings.json ejemplo**:
```json
{
  "Jwt": {
    "PrivateKey": "TuClaveSecretaDeAlMenos32Caracteres!",
    "Audience": "TalentInsights API",
    "Issuer": "TalentInsights",
    "ExpirationInMinutesMin": "1",
    "ExpirationInMinutesMax": "5"
  },
  "Auth": {
    "RefreshToken": {
      "ExpirationInDays": "15"
    }
  }
}
```

### 2.3 ClaimsConstants

**Ubicación**: `TalentInsights.Shared/Constants/ClaimsConstants.cs`

Define las claves de claims utilizadas en el JWT.

```csharp
namespace TalentInsights.Shared.Constants
{
    public static class ClaimsConstants
    {
        public const string COLLABORATOR_ID = "CollaboratorId";
    }
}
```

### 2.4 RoleConstants

**Ubicación**: `TalentInsights.Shared/Constants/RoleConstants.cs`

Define los nombres de roles disponibles en el sistema.

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

### 2.5 ResponseConstants

**Ubicación**: `TalentInsights.Shared/Constants/ResponseConstants.cs`

Mensajes de respuesta predefinidos para errores comunes.

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

---

## 3. Helpers

Los helpers son clases estáticas con métodos de utilidad que encapsulan lógica repetitiva.

### 3.1 DateTimeHelper

**Ubicación**: `TalentInsights.Shared/Helpers/DateTimeHelper.cs`

Proporciona una forma consistente de obtener la hora UTC.

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

**Propósito**: Centraliza el uso de `DateTime.UtcNow` para evitar inconsistencias de zona horaria. Útil para testing donde se puede mockear.

### 3.2 ResponseHelper

**Ubicación**: `TalentInsights.Application/Helpers/ResponseHelper.cs`

Crea respuestas genéricas de forma consistente.

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

**Modelo GenericResponse**:
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

**Uso**:
```csharp
return ResponseHelper.Create(new LoginAuthResponse { Token = token });
return ResponseHelper.Create(null, errors: ["Error 1", "Error 2"]);
```

### 3.3 CacheHelper

**Ubicación**: `TalentInsights.Application/Helpers/CacheHelper.cs`

Genera claves de cache consistentes para autenticación.

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

**Propósito**: Estandariza el formato de las claves de cache para que sean fáciles de identificar y mantener. El prefijo `auth:tokens:` y `auth:refresh_tokens:` permite identificar rápidamente el tipo de dato cacheado.

### 3.4 TokenHelper

**Ubicación**: `TalentInsights.Application/Helpers/TokenHelper.cs`

Es el corazón del sistema de autenticación. Maneja la creación de JWT y refresh tokens.

```csharp
namespace TalentInsights.Application.Helpers
{
    public static class TokenHelper
    {
        public static readonly Random rnd = new();

        public static string Create(Guid collaboratorId, List<string> roles, IConfiguration configuration, ICacheService cache)
        {
            // 1. Obtener configuración del JWT
            var tokenConfiguration = Configuration(configuration);
            
            // 2. Crear credenciales de firma
            var signingCredentials = new SigningCredentials(tokenConfiguration.SecurityKey, SecurityAlgorithms.HmacSha256);

            // 3. Definir claims del token
            var claims = new[]
            {
                new Claim(ClaimsConstants.COLLABORATOR_ID, collaboratorId.ToString()),
                new Claim(ClaimTypes.Role, roles[0])
            };

            // 4. Crear el JWT
            var securityToken = new JwtSecurityToken(
                audience: tokenConfiguration.Audience,
                issuer: tokenConfiguration.Issuer,
                expires: tokenConfiguration.Expiration,
                signingCredentials: signingCredentials,
                claims: claims
            );
            
            // 5. Convertir a string
            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            // 6. Guardar en cache con expiración igual al token
            var cacheKey = CacheHelper.AuthTokenCreation(token, tokenConfiguration.ExpirationTimeSpan);
            cache.Create(cacheKey.Key, cacheKey.Expiration, token);

            return token;
        }

        public static string CreateRefresh(Guid collaboratorId, IConfiguration configuration, ICacheService cacheService)
        {
            // 1. Generar token aleatorio largo (100 caracteres)
            var token = Generate.RandomText(100);
            
            // 2. Crear clave de cache
            var cacheKey = CacheHelper.AuthRefreshTokenCreation(token, configuration);

            // 3. Guardar en cache con datos del refresh token
            cacheService.Create(cacheKey.Key, cacheKey.Expiration, new RefreshToken
            {
                CollaboratorId = collaboratorId,
                ExpirationInDays = cacheKey.Expiration
            });

            return token;
        }

        public static TokenConfiguration Configuration(IConfiguration configuration)
        {
            // Cargar configuración desde env vars o appsettings
            var issuer = Environment.GetEnvironmentVariable(ConfigurationConstants.JWT_ISSUER)
                ?? configuration[ConfigurationConstants.JWT_ISSUER]
                ?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.JWT_ISSUER));

            var audience = Environment.GetEnvironmentVariable(ConfigurationConstants.JWT_AUDIENCE)
                ?? configuration[ConfigurationConstants.JWT_AUDIENCE]
                ?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.JWT_AUDIENCE));

            var privateKey = Environment.GetEnvironmentVariable(ConfigurationConstants.JWT_PRIVATE_KEY)
                ?? configuration[ConfigurationConstants.JWT_PRIVATE_KEY]
                ?? throw new Exception(ResponseConstants.ConfigurationPropertyNotFound(ConfigurationConstants.JWT_PRIVATE_KEY));

            // Crear clave de seguridad simétrica
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));

            // Calcular expiración aleatoria entre min y max
            var now = DateTimeHelper.UtcNow();
            var randomExpiration = rnd.Next(
                Convert.ToInt32(configuration[ConfigurationConstants.JWT_EXPIRATION_IN_MINUTES_MIN] ?? "1"),
                Convert.ToInt32(configuration[ConfigurationConstants.JWT_EXPIRATION_IN_MINUTES_MAX] ?? "5")
            );
            var timespanExpiration = TimeSpan.FromMinutes(randomExpiration);
            var datetimeExpiration = now.Add(TimeSpan.FromMinutes(randomExpiration));

            return new TokenConfiguration
            {
                Issuer = issuer,
                Audience = audience,
                SecurityKey = securityKey,
                Expiration = datetimeExpiration,
                ExpirationTimeSpan = timespanExpiration
            };
        }
    }
}
```

**Características importantes**:
- **Expiración aleatoria**: El token de acceso dura entre 1 y 5 minutos (configurable) para mayor seguridad
- **Double expiration**: El token se guarda en cache con la misma duración que el JWT para poder invalidarlo
- **Soporte para variables de entorno**: Prioriza env vars sobre appsettings
- **Claims minimalistas**: Solo incluye CollaboratorId y Role

---

## 4. Sistema de Cache

El sistema de cache usa `IMemoryCache` de ASP.NET Core para almacenar tokens y refresh tokens en memoria.

### 4.1 Interfaz ICacheService

**Ubicación**: `TalentInsights.Application/Interfaces/Services/ICacheService.cs`

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

### 4.2 Implementación CacheService

**Ubicación**: `TalentInsights.Application/Services/CacheService.cs`

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

**Características**:
- **SlidingExpiration**: El tiempo de expiración se renueva cada vez que se accede al cache
- **Generic**: Funciona con cualquier tipo de objeto
- **Throw en create**: Lanza excepción si no puede crear la entrada

### 4.3 Registro en DI

En `Program.cs` o extensión de servicios:

```csharp
// En ServiceCollectionExtension.cs
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();
```

### 4.4 Modelo CacheKey

**Ubicación**: `TalentInsights.Application/Models/Helpers/CacheKey.cs`

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

## 5. Flujo de Autenticación JWT con Refresh Token

### 5.1 Modelo de Datos

**RefreshToken** (`TalentInsights.Application/Models/Helpers/RefreshToken.cs`):
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

**TokenConfiguration** (`TalentInsights.Application/Models/Helpers/TokenConfiguration.cs`):
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

### 5.2 Diagrama del Flujo

```
┌─────────────────────────────────────────────────────────────────────────┐
│                              LOGIN                                       │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│   Cliente                                                                │
│      │                                                                   │
│      │ 1. POST /api/auth/login                                         │
│      │    { email, password }                                           │
│      ▼                                                                   │
│   ┌─────────────────┐                                                   │
│   │ AuthController  │                                                   │
│   └────────┬────────┘                                                   │
│            │                                                             │
│            ▼                                                             │
│   ┌─────────────────┐                                                   │
│   │   AuthService   │                                                   │
│   │    .Login()     │                                                   │
│   └────────┬────────┘                                                   │
│            │                                                             │
│      2. Validar usuario/contraseña                                      │
│            │                                                             │
│      3. Obtener roles del usuario                                       │
│            │                                                             │
│            ▼                                                             │
│   ┌─────────────────┐     ┌─────────────────┐                           │
│   │  TokenHelper    │     │  CacheService   │                           │
│   │  .Create()      │     │                 │                           │
│   └────────┬────────┘     └────────┬────────┘                           │
│            │                        │                                    │
│      4a. Generar JWT         4b. Guardar JWT en cache                   │
│            │                        │ (clave: auth:tokens:{jwt})         │
│            │                        │ (expira: 1-5 min)                  │
│            ▼                        ▼                                    │
│      5a. Generar refresh     5b. Guardar RefreshToken en cache          │
│            token (random)        (clave: auth:refresh_tokens:{token})  │
│                                   (expira: 15 días)                     │
│            │                        │                                    │
│            ◄────────────────────────┘                                    │
│                                                                         │
│      6. Return { token, refreshToken }                                  │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│                              RENEW                                       │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│   Cliente                                                                │
│      │                                                                   │
│      │ 1. POST /api/auth/renew                                         │
│      │    { refreshToken }                                              │
│      ▼                                                                   │
│   ┌─────────────────┐                                                   │
│   │ AuthController  │                                                   │
│   └────────┬────────┘                                                   │
│            │                                                             │
│            ▼                                                             │
│   ┌─────────────────┐                                                   │
│   │   AuthService   │                                                   │
│   │    .Renew()     │                                                   │
│   └────────┬────────┘                                                   │
│            │                                                             │
│      2. Buscar refresh token en cache                                   │
│         clave: auth:refresh_tokens:{refreshToken}                      │
│            │                                                             │
│      ┌─────┴─────┐                                                      │
│      │            │                                                      │
│   Si existe    No existe                                               │
│      │            │                                                      │
│      ▼            ▼                                                      │
│   3. Obtener    4. Error                                                │
│   collaborator  NotFoundException                                       │
│   por ID                                                             │
│      │                                                               │
│      ▼                                                               │
│   5. Generar nuevo JWT (TokenHelper.Create)                           │
│                                                                         │
│   6. Generar nuevo refresh token                                       │
│                                                                         │
│   7. Eliminar refresh token antiguo del cache                          │
│      (CacheService.Delete)                                             │
│                                                                         │
│   8. Return { nuevoToken, nuevoRefreshToken }                          │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### 5.3 Código del AuthService

**Ubicación**: `TalentInsights.Application/Services/AuthService.cs`

```csharp
namespace TalentInsights.Application.Services
{
    public class AuthService(IUnitOfWork uow, IConfiguration configuration, ICacheService cacheService) : IAuthService
    {
        public async Task<GenericResponse<LoginAuthResponse>> Login(LoginAuthRequest model)
        {
            // 1. Buscar usuario por email
            var collaborator = await uow.collaboratorRepository.Get(model.Email)
                ?? throw new BadRequestException(ResponseConstants.AUTH_USER_OR_PASSWORD_NOT_FOUND);

            // 2. Validar contraseña
            var validatePassword = Hasher.ComparePassword(model.Password, collaborator.Password);
            if (!validatePassword)
            {
                throw new BadRequestException(ResponseConstants.AUTH_USER_OR_PASSWORD_NOT_FOUND);
            }

            // 3. Obtener roles del colaborador
            var roles = collaborator.CollaboratorRoleCollaborators
                .Select(x => x.Role.Name)
                .ToList();

            // 4. Generar tokens
            var token = TokenHelper.Create(collaborator.Id, roles, configuration, cacheService);
            var refreshToken = TokenHelper.CreateRefresh(collaborator.Id, configuration, cacheService);

            // 5. Responder
            return ResponseHelper.Create(new LoginAuthResponse
            {
                Token = token,
                RefreshToken = refreshToken
            });
        }

        public async Task<GenericResponse<LoginAuthResponse>> Renew(RenewAuthRequest model)
        {
            // 1. Buscar refresh token en cache
            var findRefreshToken = cacheService.Get<RefreshToken>(
                CacheHelper.AuthRefreshTokenKey(model.RefreshToken)
            ) ?? throw new NotFoundException(ResponseConstants.AUTH_REFRESH_TOKEN_NOT_FOUND);

            // 2. Obtener colaborador
            var collaborator = await uow.collaboratorRepository.Get(findRefreshToken.CollaboratorId)
                ?? throw new NotFoundException(ResponseConstants.COLLABORATOR_NOT_EXISTS);

            // 3. Generar nuevos tokens
            var token = TokenHelper.Create(findRefreshToken.CollaboratorId, roles, configuration, cacheService);
            var refreshToken = TokenHelper.CreateRefresh(findRefreshToken.CollaboratorId, configuration, cacheService);

            // 4. Eliminar refresh token antiguo
            cacheService.Delete(CacheHelper.AuthRefreshTokenKey(model.RefreshToken));

            // 5. Responder
            return ResponseHelper.Create(new LoginAuthResponse
            {
                Token = token,
                RefreshToken = refreshToken
            });
        }
    }
}
```

### 5.4 Modelo de Respuesta

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

---

## 6. Configuración Necesaria para Replicar

### 6.1 Paquetes NuGet requeridos

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.0" />
<PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="9.0.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.0.0" />
```

### 6.2 appsettings.json

```json
{
  "Jwt": {
    "PrivateKey": "TuClaveSecretaDeAlMenos32Caracteres123456",
    "Audience": "TuApp API",
    "Issuer": "TuApp",
    "ExpirationInMinutesMin": "1",
    "ExpirationInMinutesMax": "5"
  },
  "Auth": {
    "RefreshToken": {
      "ExpirationInDays": "15"
    }
  }
}
```

### 6.3 Configuración en Program.cs

```csharp
// Memory Cache
builder.Services.AddMemoryCache();

// Cache Service
builder.Services.AddScoped<ICacheService, CacheService>();

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

## 7. Resumen de Arquitectura

| Componente | Responsabilidad | Ubicación |
|------------|-----------------|------------|
| **Constants** | Valores centralizados (validación, configuración, respuestas, roles) | `TalentInsights.Shared/Constants/` |
| **Helpers** | Métodos utilitarios estáticos | `TalentInsights.Application/Helpers/` |
| **CacheService** | Abstracción de IMemoryCache con métodos CRUD | `TalentInsights.Application/Services/` |
| **TokenHelper** | Generación de JWT y refresh tokens + cache | `TalentInsights.Application/Helpers/` |
| **AuthService** | Lógica de negocio de autenticación | `TalentInsights.Application/Services/` |

### Beneficios de esta arquitectura:

1. **Separación de responsabilidades**: Cada clase tiene una única responsabilidad
2. **Inversión de dependencias**: Depende de abstracciones (interfaces), no de implementaciones
3. **Testabilidad**: Los helpers y servicios pueden unit-testearse fácilmente
4. **Mantenibilidad**: Los cambios en configuración o mensajes se hacen en un solo lugar
5. **Consistencia**: Los patrones de nomenclatura y estructura son uniformes

---

## 8. Notas de Seguridad

1. **JWT Private Key**: Debe tener al menos 32 caracteres
2. **Refresh Token**: Se genera aleatoriamente con 100 caracteres para evitar猜测
3. **Double Expiration**: El JWT en cache expira junto con el token mismo, permitiendo invalidación
4. **Sliding Expiration**: El cache renueva el tiempo cada vez que se accede
5. **Variables de entorno**: Priorizan sobre archivos de configuración para producción