using System.Text.Json.Serialization;
using EImzo.Client.Models.Common;

namespace EImzo.Client.Models.Pkcs7;

/// <summary>
/// Response returned by the /backend/pkcs7/verify/detached endpoint.
/// </summary>
public class DetachedVerifyResponse : EImzoResponse
{
    /// <summary>
    /// Verification details for the PKCS#7 Detached document.
    /// Note: The original documentBase64 is not included in detached verify response.
    /// </summary>
    [JsonPropertyName("pkcs7Info")]
    public Pkcs7Info? Pkcs7Info { get; set; }
}
