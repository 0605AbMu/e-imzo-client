using System.Text.Json.Serialization;
using EImzo.Client.Models.Common;

namespace EImzo.Client.Models.Pkcs7;

/// <summary>
/// Response returned by the /backend/pkcs7/verify/attached endpoint.
/// </summary>
public class AttachedVerifyResponse : EImzoResponse
{
    /// <summary>
    /// Verification details for the PKCS#7 Attached document, including signers and extracted document.
    /// </summary>
    [JsonPropertyName("pkcs7Info")]
    public Pkcs7Info? Pkcs7Info { get; set; }
}
