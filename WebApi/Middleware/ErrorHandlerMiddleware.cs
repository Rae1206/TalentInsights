using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.Exceptions;

namespace WebApi.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[{TraceId}] Error no controlado | {Method} {Path} | User: {UserId} | IP: {ClientIp} | ExceptionType: {ExceptionType}",
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path,
                context.User.Identity?.Name ?? "Anónimo",
                context.Connection.RemoteIpAddress?.ToString() ?? "Desconocida",
                ex.GetType().Name);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        var problem = exception switch
        {
            ResourceNotFoundException ex => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Recurso no encontrado",
                Detail = ex.Message
            },
            ValidationException ex => new ValidationProblemDetails(ex.Errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Error de validación",
                Detail = ex.Message
            },
            ConflictException ex => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflicto",
                Detail = ex.Message
            },
            AlreadyExistsException ex => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Recurso ya existente",
                Detail = ex.Message
            },
            ForbiddenException ex => new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Acceso denegado",
                Detail = ex.Message
            },
            KeyNotFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Recurso no encontrado",
                Detail = ErrorConstants.RESOURCE_NOT_FOUND      
            },
            ArgumentException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Argumento inválido",
                Detail = exception.Message
            },
            UnauthorizedAccessException => new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "No autorizado",
                Detail = ErrorConstants.UNAUTHORIZED
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Error interno del servidor",
                Detail = string.Format(ErrorConstants.UNEXPECTED_ERROR, traceId)
            }
        };

        problem.Extensions["traceId"] = traceId;
        problem.Extensions["timestamp"] = DateTime.UtcNow.ToString("o");

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;

        await context.Response.WriteAsJsonAsync(problem);
    }
}
