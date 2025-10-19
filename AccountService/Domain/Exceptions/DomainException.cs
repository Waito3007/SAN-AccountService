namespace AccountService.Domain.Exceptions;

/// <summary>
/// Custom Exception cho Domain Layer
/// </summary>
public class DomainException : Exception
{
    public DomainException() : base()
    {
    }

    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}