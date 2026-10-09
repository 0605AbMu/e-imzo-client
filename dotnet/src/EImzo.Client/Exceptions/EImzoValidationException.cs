namespace EImzo.Client.Exceptions;

/// <summary>
/// Exception thrown when client-side parameter validation fails before making an API call.
/// </summary>
public class EImzoValidationException : EImzoException
{
    public string ParameterName { get; }

    public EImzoValidationException(string parameterName, string message)
        : base($"Validation failed for '{parameterName}': {message}")
    {
        ParameterName = parameterName;
    }
}
