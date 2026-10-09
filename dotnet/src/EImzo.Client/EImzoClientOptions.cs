namespace EImzo.Client;

/// <summary>
/// Configuration options for the E-IMZO Client SDK.
/// </summary>
public class EImzoClientOptions
{
    /// <summary>
    /// Base address of the E-IMZO-SERVER REST-API. Default is "http://127.0.0.1:8080/".
    /// </summary>
    public Uri BaseUrl { get; set; } = new Uri("http://127.0.0.1:8080/");

    /// <summary>
    /// Default domain name sent in the HTTP 'Host' header for backend and timestamp endpoints.
    /// Example: "example.uz".
    /// </summary>
    public string? DefaultHost { get; set; }

    /// <summary>
    /// Default IP address sent in the HTTP 'X-Real-IP' header for backend endpoints.
    /// Can be overridden on a per-request basis.
    /// </summary>
    public string? DefaultRealIp { get; set; }

    /// <summary>
    /// HTTP request timeout. Default is 30 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// If true, automatically throws an <see cref="Exceptions.EImzoApiException"/>
    /// when the server returns a response with status != 1.
    /// Default is false, allowing consumers to inspect status codes gracefully or call EnsureSuccess().
    /// </summary>
    public bool ThrowOnError { get; set; } = false;
}
