using System.Text.Json.Serialization;
using EImzo.Client.Models.Common;
using EImzo.Client.Serialization;

namespace EImzo.Client.Models.Mobile;

/// <summary>
/// Additional verification info returned after verifying a mobile ID-CARD signature.
/// </summary>
public class MobileVerificationInfo
{
    [JsonPropertyName("policyIdentifiers")]
    public List<string>? PolicyIdentifiers { get; set; }

    [JsonPropertyName("signingTime")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? SigningTime { get; set; }

    [JsonPropertyName("timestampedTime")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? TimestampedTime { get; set; }
}

/// <summary>
/// Response returned by the /backend/mobile/verify endpoint.
/// </summary>
public class MobileVerifyResponse : EImzoResponse
{
    [JsonPropertyName("subjectCertificateInfo")]
    public SubjectCertificateInfo? SubjectCertificateInfo { get; set; }

    [JsonPropertyName("verificationInfo")]
    public MobileVerificationInfo? VerificationInfo { get; set; }

    [JsonPropertyName("pkcs7Attached")]
    public string? Pkcs7Attached { get; set; }
}
