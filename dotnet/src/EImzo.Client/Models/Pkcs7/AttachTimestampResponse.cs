using System.Text.Json.Serialization;
using EImzo.Client.Models.Common;

namespace EImzo.Client.Models.Pkcs7;

/// <summary>
/// Response returned by the /frontend/timestamp/pkcs7 endpoint.
/// </summary>
public class AttachTimestampResponse : EImzoResponse
{
    /// <summary>
    /// Base64 encoded PKCS#7 document with the timestamp token attached.
    /// </summary>
    [JsonPropertyName("pkcs7b64")]
    public string? Pkcs7Base64 { get; set; }

    /// <summary>
    /// List of signer certificates whose signatures received a timestamp token.
    /// </summary>
    [JsonPropertyName("timestampedSignerList")]
    public List<SubjectCertificateInfo>? TimestampedSignerList { get; set; }
}
