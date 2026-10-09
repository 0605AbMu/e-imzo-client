using System.Text.Json.Serialization;
using EImzo.Client.Enums;
using EImzo.Client.Serialization;

namespace EImzo.Client.Models.Common;

/// <summary>
/// Information about a signer who signed the PKCS#7 document.
/// </summary>
public class SignerInfo
{
    [JsonPropertyName("signerId")]
    public SignerIdInfo? SignerId { get; set; }

    [JsonPropertyName("signingTime")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? SigningTime { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

    [JsonPropertyName("digest")]
    public string? Digest { get; set; }

    [JsonPropertyName("verified")]
    public bool Verified { get; set; }

    [JsonPropertyName("certificateVerified")]
    public bool CertificateVerified { get; set; }

    [JsonPropertyName("certificateValidAtSigningTime")]
    public bool CertificateValidAtSigningTime { get; set; }

    [JsonPropertyName("revokedStatusInfo")]
    public string? RevokedStatusInfo { get; set; }

    [JsonPropertyName("exception")]
    public string? Exception { get; set; }

    [JsonPropertyName("OCSPResponse")]
    public string? OcspResponse { get; set; }

    [JsonPropertyName("statusUpdatedAt")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? StatusUpdatedAt { get; set; }

    [JsonPropertyName("statusNextUpdateAt")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? StatusNextUpdateAt { get; set; }

    [JsonPropertyName("certificate")]
    public List<CertificateDetailInfo>? Certificate { get; set; }

    [JsonPropertyName("trustedCertificate")]
    public CertificateDetailInfo? TrustedCertificate { get; set; }

    [JsonPropertyName("policyIdentifiers")]
    public List<string>? PolicyIdentifiers { get; set; }

    [JsonPropertyName("timeStampInfo")]
    public TimeStampDetailInfo? TimeStampInfo { get; set; }

    /// <summary>
    /// Gets the primary signer certificate (the user certificate, usually first in chain).
    /// </summary>
    [JsonIgnore]
    public CertificateDetailInfo? UserCertificate => Certificate != null && Certificate.Count > 0 ? Certificate[0] : null;

    /// <summary>
    /// Gets the key type of the primary signer certificate determined by its public key paramSetOID.
    /// </summary>
    [JsonIgnore]
    public EImzoKeyType KeyType => UserCertificate?.KeyType ?? EImzoKeyType.Unknown;
}

public class SignerIdInfo
{
    [JsonPropertyName("issuer")]
    public string? Issuer { get; set; }

    [JsonPropertyName("subjectSerialNumber")]
    public string? SubjectSerialNumber { get; set; }
}

public class CertificateDetailInfo
{
    [JsonPropertyName("serialNumber")]
    public string? SerialNumber { get; set; }

    [JsonPropertyName("subjectName")]
    public string? SubjectName { get; set; }

    [JsonPropertyName("issuerName")]
    public string? IssuerName { get; set; }

    [JsonPropertyName("validFrom")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? ValidFrom { get; set; }

    [JsonPropertyName("validTo")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? ValidTo { get; set; }

    [JsonPropertyName("subjectInfo")]
    public Dictionary<string, string>? SubjectInfo { get; set; }

    [JsonPropertyName("issuerInfo")]
    public Dictionary<string, string>? IssuerInfo { get; set; }

    [JsonPropertyName("publicKey")]
    public CertificatePublicKey? PublicKey { get; set; }

    [JsonPropertyName("signature")]
    public CertificateSignature? Signature { get; set; }

    /// <summary>
    /// Type of digital signature key or hardware token determined by public key paramSetOID.
    /// </summary>
    [JsonIgnore]
    public EImzoKeyType KeyType => PublicKey?.KeyType ?? EImzoKeyType.Unknown;
}

public class TimeStampDetailInfo
{
    [JsonPropertyName("verified")]
    public bool Verified { get; set; }

    [JsonPropertyName("digestVerified")]
    public bool DigestVerified { get; set; }

    [JsonPropertyName("certificateVerified")]
    public bool CertificateVerified { get; set; }

    [JsonPropertyName("certificateValidAtSigningTime")]
    public bool CertificateValidAtSigningTime { get; set; }

    [JsonPropertyName("time")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? Time { get; set; }

    [JsonPropertyName("tsa")]
    public string? Tsa { get; set; }

    [JsonPropertyName("tsaPolicy")]
    public string? TsaPolicy { get; set; }

    [JsonPropertyName("serialNumber")]
    public string? SerialNumber { get; set; }

    [JsonPropertyName("hashAlgorithm")]
    public string? HashAlgorithm { get; set; }

    [JsonPropertyName("messageImprintAlgOID")]
    public string? MessageImprintAlgOid { get; set; }

    [JsonPropertyName("messageImprintDigest")]
    public string? MessageImprintDigest { get; set; }

    [JsonPropertyName("OCSPResponse")]
    public string? OcspResponse { get; set; }

    [JsonPropertyName("statusUpdatedAt")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? StatusUpdatedAt { get; set; }

    [JsonPropertyName("statusNextUpdateAt")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? StatusNextUpdateAt { get; set; }

    [JsonPropertyName("signerId")]
    public SignerIdInfo? SignerId { get; set; }

    [JsonPropertyName("certificate")]
    public List<CertificateDetailInfo>? Certificate { get; set; }

    [JsonPropertyName("trustedCertificate")]
    public CertificateDetailInfo? TrustedCertificate { get; set; }
}
