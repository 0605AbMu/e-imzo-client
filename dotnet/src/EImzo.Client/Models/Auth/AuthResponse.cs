using System.Text.Json.Serialization;
using EImzo.Client.Models.Common;

namespace EImzo.Client.Models.Auth;

/// <summary>
/// Response returned by the /backend/auth endpoint.
/// </summary>
public class AuthResponse : EImzoResponse
{
    /// <summary>
    /// Information about the subject certificate of the authenticated user.
    /// </summary>
    [JsonPropertyName("subjectCertificateInfo")]
    public SubjectCertificateInfo? SubjectCertificateInfo { get; set; }
}
