namespace Twitter.Domain.Exceptions;

/// <summary>
/// Exception thrown when a request is invalid or fails business validation.
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException()
    {
    }

    public BadRequestException(string message)
        : base(message)
    {
    }

    public BadRequestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}