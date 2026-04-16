using Application.Models.Responses;

namespace Application.Helpers;

/// <summary>
/// Helper estático para crear respuestas genéricas.
/// </summary>
public static class ResponseHelper
{
    /// <summary>
    /// Crea una respuesta genérica con datos, mensaje y errores opcionales.
    /// </summary>
    /// <typeparam name="T">Tipo de los datos</typeparam>
    /// <param name="data">Datos a retornar</param>
    /// <param name="errors">Lista de errores (opcional)</param>
    /// <param name="message">Mensaje de la respuesta (opcional)</param>
    /// <returns>GenericResponse con los datos proporcionados</returns>
    public static GenericResponse<T> Create<T>(T data, List<string>? errors = null, string? message = null)
    {
        return new GenericResponse<T>
        {
            Data = data,
            Message = message ?? "Solicitud realizada correctamente",
            Errors = errors ?? []
        };
    }
}