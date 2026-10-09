using System.Text.Json.Serialization;
using EImzo.Client.Models.Common;

namespace EImzo.Client.Models.Mobile;

/// <summary>
/// Response returned by the /backend/mobile/authenticate/{DocumentID} endpoint.
/// </summary>
public class MobileAuthenticateResponse : EImzoResponse
{
    [JsonPropertyName("subjectCertificateInfo")]
    public SubjectCertificateInfo? SubjectCertificateInfo { get; set; }
}
