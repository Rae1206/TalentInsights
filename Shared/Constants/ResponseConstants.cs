namespace Shared.Constants;

/// <summary>
/// Mensajes de respuesta predefinidos para errores comunes en autenticación.
/// </summary>
public static class ResponseConstants
{
    public const string USER_NOT_EXISTS = "El usuario no existe";
    
    public static string RoleNotFound(string name) => $"El rol {name} no existe";
    public static string RoleNotFound(Guid id) => $"El rol con ID: {id} no existe";

    public const string POST_NOT_EXISTS = "La publicación no existe";

    // === Errores de autenticación ===
    public const string AUTH_TOKEN_NOT_FOUND = "El token no es correcto, expiró o no se argumentó";
    public const string AUTH_USER_OR_PASSWORD_NOT_FOUND = "Usuario o contraseña incorrectos";
    public const string AUTH_REFRESH_TOKEN_NOT_FOUND = "El token para refrescar la sesión expiró, no existe o es incorrecto";

    // === Errores inesperados ===
    public static string ErrorUnexpected(string traceId)
    {
        return $"Ha ocurrido un error inesperado: Contacto con soporte, mencionando el siguiente código de error: {traceId}";
    }

    public static string ConfigurationPropertyNotFound(string property)
    {
        return $"Falta la propiedad '{property}' por establecer en la configuración del aplicativo.";
    }
}