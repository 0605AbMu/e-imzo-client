using System.Text.Json.Serialization;
using EImzo.Client.Enums;
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

    /// <summary>
    /// Public key parameters if returned at root response level.
    /// </summary>
    [JsonPropertyName("publicKeyParameter")]
    public CertificatePublicKey? PublicKeyParameter { get; set; }

    /// <summary>
    /// Direct parameter set OID if returned at root response level.
    /// </summary>
    [JsonPropertyName("paramSetOID")]
    public string? ParamSetOid { get; set; }

    /// <summary>
    /// Type of digital signature key or hardware token (PFX, IdCard, Baik, Uzguard) determined by paramSetOID.
    /// </summary>
    [JsonIgnore]
    public EImzoKeyType KeyType => EImzoParamSetOids.ResolveKeyType(
        ParamSetOid ??
        PublicKeyParameter?.ParamSetOid ??
        SubjectCertificateInfo?.PublicKeyParameter?.ParamSetOid ??
        SubjectCertificateInfo?.ParamSetOid);
}
