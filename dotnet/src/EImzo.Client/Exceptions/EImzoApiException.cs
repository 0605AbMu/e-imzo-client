using System.Net;
using EImzo.Client.Enums;

namespace EImzo.Client.Exceptions;

/// <summary>
/// Exception thrown when an E-IMZO API returns an error status code or unexpected HTTP failure.
/// </summary>
public class EImzoApiException : EImzoException
{
    /// <summary>
    /// The numeric status code returned by E-IMZO-SERVER.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// The mapped status code as an enum, if recognized.
    /// </summary>
    public EImzoStatusCode StatusEnum => Enum.IsDefined(typeof(EImzoStatusCode), StatusCode)
        ? (EImzoStatusCode)StatusCode
        : EImzoStatusCode.Unknown;

    /// <summary>
    /// The HTTP status code of the response, if available.
    /// </summary>
    public HttpStatusCode? HttpStatusCode { get; }

    /// <summary>
    /// The API endpoint that produced this error.
    /// </summary>
    public string? Endpoint { get; }

    public EImzoApiException(int statusCode, string? message, string? endpoint = null, HttpStatusCode? httpStatusCode = null)
        : base(FormatMessage(statusCode, message, endpoint))
    {
        StatusCode = statusCode;
        Endpoint = endpoint;
        HttpStatusCode = httpStatusCode;
    }

    public EImzoApiException(string message, string? endpoint = null, HttpStatusCode? httpStatusCode = null, Exception? innerException = null)
        : base(message, innerException!)
    {
        StatusCode = -999;
        Endpoint = endpoint;
        HttpStatusCode = httpStatusCode;
    }

    private static string FormatMessage(int statusCode, string? message, string? endpoint)
    {
        var desc = string.IsNullOrWhiteSpace(message) ? "Operation failed." : message;
        return string.IsNullOrWhiteSpace(endpoint)
            ? $"E-IMZO error [Status: {statusCode}]: {desc}"
            : $"E-IMZO error [Status: {statusCode}] at '{endpoint}': {desc}";
    }
}
