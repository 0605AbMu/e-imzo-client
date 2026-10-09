using System.Text.Json.Serialization;
using EImzo.Client.Enums;
using EImzo.Client.Exceptions;

namespace EImzo.Client.Models.Common;

/// <summary>
/// Base model for responses returned by E-IMZO-SERVER endpoints.
/// </summary>
public class EImzoResponse
{
    /// <summary>
    /// Numeric status code returned by the server (1 = Success, negative values indicate errors).
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// Error or informational message returned by the server.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Indicates whether the operation succeeded (Status == 1).
    /// </summary>
    [JsonIgnore]
    public bool IsSuccess => Status == 1;

    /// <summary>
    /// The mapped status code as an enum, if recognized.
    /// </summary>
    [JsonIgnore]
    public EImzoStatusCode StatusCode => Enum.IsDefined(typeof(EImzoStatusCode), Status)
        ? (EImzoStatusCode)Status
        : EImzoStatusCode.Unknown;

    /// <summary>
    /// Ensures that the response was successful (Status == 1), throwing <see cref="EImzoApiException"/> otherwise.
    /// </summary>
    /// <param name="endpoint">Optional endpoint name for diagnostics.</param>
    /// <exception cref="EImzoApiException">Thrown when Status != 1.</exception>
    public virtual void EnsureSuccess(string? endpoint = null)
    {
        if (!IsSuccess)
        {
            throw new EImzoApiException(Status, Message, endpoint);
        }
    }
}

/// <summary>
/// Generic E-IMZO response model wrapper for arbitrary data payloads.
/// </summary>
/// <typeparam name="T">The payload data type.</typeparam>
public class EImzoResponse<T> : EImzoResponse
{
    /// <summary>
    /// Response payload data.
    /// </summary>
    public T? Data { get; set; }

    public EImzoResponse()
    {
    }

    public EImzoResponse(T? data)
    {
        Data = data;
        Status = 1;
    }
}
