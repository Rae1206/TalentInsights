namespace Application.Interfaces.Services;

/// <summary>
/// Interfaz del servicio de caché en memoria.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Crea una entrada en el caché con tiempo de expiración.
    /// </summary>
    /// <typeparam name="T">Tipo del valor</typeparam>
    /// <param name="key">Clave única</param>
    /// <param name="expiration">Tiempo de expiración</param>
    /// <param name="value">Valor a guardar</param>
    /// <returns>El valor guardado</returns>
    T Create<T>(string key, TimeSpan expiration, T value);

    /// <summary>
    /// Obtiene un valor del caché.
    /// </summary>
    /// <typeparam name="T">Tipo del valor esperado</typeparam>
    /// <param name="key">Clave única</param>
    /// <returns>El valor o null si no existe</returns>
    T? Get<T>(string key);

    /// <summary>
    /// Elimina una clave del caché.
    /// </summary>
    /// <param name="key">Clave a eliminar</param>
    /// <returns>True si existía y se eliminó, False si no existía</returns>
    bool Delete(string key);
}