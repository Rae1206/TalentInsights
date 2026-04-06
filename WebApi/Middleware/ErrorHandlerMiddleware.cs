using System.Net;
using System.Text.Json;

namespace WebApi.Middleware;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ErrorHandlerMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            KeyNotFoundException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = "Resource not found",
                Details = exception.Message,
                Timestamp = DateTime.UtcNow
            },
            ArgumentException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Invalid argument",
                Details = exception.Message,
                Timestamp = DateTime.UtcNow
            },
            UnauthorizedAccessException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Message = "Unauthorized access",
                Details = _environment.IsDevelopment() ? exception.Message : null,
                Timestamp = DateTime.UtcNow
            },
            _ => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An internal server error occurred",
                Details = _environment.IsDevelopment() ? exception.Message : null,
                Timestamp = DateTime.UtcNow
            }
        };

        context.Response.StatusCode = response.StatusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsJsonAsync(response, options);
    }
}
