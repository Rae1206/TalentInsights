namespace Application.Models.Requests;

/// <summary>
/// Clase base para todos los requests de la aplicación.
/// Permite agregar propiedades comunes como paginación o filtros globales.
/// </summary>
public abstract class BaseRequest
{
    // Propiedades comunes que todos los requests pueden heredar
    // Ejemplo futuro: public int? PageNumber { get; set; }
    // Ejemplo futuro: public int? PageSize { get; set; }
}
