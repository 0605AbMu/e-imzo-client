using System.Text.Json.Serialization;
using EImzo.Client.Models.Common;

namespace EImzo.Client.Models.Pkcs7;

/// <summary>
/// Response returned by the /frontend/pkcs7/make-attached endpoint.
/// </summary>
public class MakeAttachedResponse : EImzoResponse
{
    /// <summary>
    /// Base64 encoded assembled PKCS#7 Attached document.
    /// </summary>
    [JsonPropertyName("pkcs7b64")]
    public string? Pkcs7Base64 { get; set; }
}

/// <summary>
/// Response returned by the /frontend/pkcs7/join endpoint.
/// </summary>
public class JoinAttachedResponse : EImzoResponse
{
    /// <summary>
    /// Base64 encoded merged PKCS#7 Attached document containing multiple signers.
    /// </summary>
    [JsonPropertyName("pkcs7b64")]
    public string? Pkcs7Base64 { get; set; }
}
