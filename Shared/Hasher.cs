namespace Shared;

/// <summary>
/// Utilidad para hashing y verificación de contraseñas usando BCrypt
/// </summary>
public static class Hasher
{
    /// <summary>
    /// Genera un hash de la contraseña usando BCrypt
    /// </summary>
    /// <param name="password">Contraseña en texto plano</param>
    /// <returns>Hash BCrypt de la contraseña</returns>
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verifica si una contraseña coincide con un hash BCrypt
    /// </summary>
    /// <param name="password">Contraseña en texto plano</param>
    /// <param name="hashedPassword">Hash BCrypt almacenado</param>
    /// <returns>True si la contraseña es válida, false en caso contrario</returns>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
