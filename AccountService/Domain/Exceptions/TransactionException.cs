namespace AccountService.Domain.Exceptions;

/// <summary>
/// Exception cho Transaction
/// </summary>
public class TransactionException : DomainException
{
    public TransactionException(string message) : base(message)
    {
    }

    public TransactionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

