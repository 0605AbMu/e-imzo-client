namespace EImzo.Client.Exceptions;

/// <summary>
/// Base exception for all E-IMZO SDK errors.
/// </summary>
public class EImzoException : Exception
{
    public EImzoException(string message) : base(message)
    {
    }

    public EImzoException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
