using System.Text.Json.Serialization;
using EImzo.Client.Enums;
using EImzo.Client.Serialization;

namespace EImzo.Client.Models.Common;

/// <summary>
/// Information about the subject certificate extracted by E-IMZO-SERVER.
/// Includes helper properties for Uzbek OID identifiers (PINFL / JSHSHIR, TIN / STIR).
/// </summary>
public class SubjectCertificateInfo
{
    /// <summary>
    /// OID for Uzbek Individual Personal Identification Number (JSHSHIR / PINFL - 14 digits).
    /// </summary>
    public const string OidPinfl = "1.2.860.3.16.1.2";

    /// <summary>
    /// OID for Uzbek Legal Entity Tax Identification Number (STIR / INN - 9 digits).
    /// </summary>
    public const string OidLegalEntityTin = "1.2.860.3.16.1.1";

    /// <summary>
    /// Serial number of the digital signature certificate.
    /// </summary>
    [JsonPropertyName("serialNumber")]
    public string? SerialNumber { get; set; }

    /// <summary>
    /// Full X.500 distinguished name string.
    /// </summary>
    [JsonPropertyName("X500Name")]
    public string? X500Name { get; set; }

    /// <summary>
    /// Dictionary of subject attributes and OIDs.
    /// </summary>
    [JsonPropertyName("subjectName")]
    public Dictionary<string, string>? SubjectName { get; set; }

    /// <summary>
    /// Certificate validity start date and time.
    /// </summary>
    [JsonPropertyName("validFrom")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? ValidFrom { get; set; }

    /// <summary>
    /// Certificate expiration date and time.
    /// </summary>
    [JsonPropertyName("validTo")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? ValidTo { get; set; }

    /// <summary>
    /// Public key parameters returned by /backend/auth containing algorithm name and paramSetOID.
    /// </summary>
    [JsonPropertyName("publicKeyParameter")]
    public CertificatePublicKey? PublicKeyParameter { get; set; }

    /// <summary>
    /// Public key details if present directly on certificate info.
    /// </summary>
    [JsonPropertyName("publicKey")]
    public CertificatePublicKey? PublicKey { get; set; }

    /// <summary>
    /// Direct parameter set OID if present on certificate info.
    /// </summary>
    [JsonPropertyName("paramSetOID")]
    public string? ParamSetOid { get; set; }

    /// <summary>
    /// Type of digital signature key or hardware token (PFX, IdCard, Baik, Uzguard) determined by paramSetOID.
    /// </summary>
    [JsonIgnore]
    public EImzoKeyType KeyType => EImzoParamSetOids.ResolveKeyType(
        PublicKeyParameter?.ParamSetOid ?? PublicKey?.ParamSetOid ?? ParamSetOid);

    /// <summary>
    /// 14-digit Individual Personal Identification Number (JSHSHIR / PINFL), retrieved via OID 1.2.860.3.16.1.2.
    /// </summary>
    [JsonIgnore]
    public string? Pinfl => GetSubjectAttribute(OidPinfl);

    /// <summary>
    /// Legal Entity Tax Identification Number (STIR / INN - 9 digits), retrieved via OID 1.2.860.3.16.1.1.
    /// Returns null if the certificate belongs to a physical person.
    /// </summary>
    [JsonIgnore]
    public string? LegalEntityTin => GetSubjectAttribute(OidLegalEntityTin);

    /// <summary>
    /// Physical Person Tax Identification Number (INN) retrieved via "UID".
    /// </summary>
    [JsonIgnore]
    public string? PhysicalPersonTin => GetSubjectAttribute("UID");

    /// <summary>
    /// Tax Identification Number (STIR / INN). Returns LegalEntityTin if present; otherwise PhysicalPersonTin.
    /// </summary>
    [JsonIgnore]
    public string? Tin => LegalEntityTin ?? PhysicalPersonTin;

    /// <summary>
    /// Common Name (CN) / Full Name of the certificate owner.
    /// </summary>
    [JsonIgnore]
    public string? CommonName => GetSubjectAttribute("CN");

    /// <summary>
    /// First name / Given name of the certificate owner.
    /// </summary>
    [JsonIgnore]
    public string? FirstName => GetSubjectAttribute("Name");

    /// <summary>
    /// Surname / Last name of the certificate owner.
    /// </summary>
    [JsonIgnore]
    public string? Surname => GetSubjectAttribute("SURNAME");

    /// <summary>
    /// Organization name (O), if present.
    /// </summary>
    [JsonIgnore]
    public string? Organization => GetSubjectAttribute("O");

    /// <summary>
    /// Country code (C), typically "UZ".
    /// </summary>
    [JsonIgnore]
    public string? Country => GetSubjectAttribute("C");

    /// <summary>
    /// Indicates whether the certificate represents a legal entity (organization/business).
    /// </summary>
    [JsonIgnore]
    public bool IsLegalEntity => !string.IsNullOrEmpty(LegalEntityTin);

    /// <summary>
    /// Indicates whether the certificate represents an individual (physical person).
    /// </summary>
    [JsonIgnore]
    public bool IsPhysicalPerson => !IsLegalEntity;

    /// <summary>
    /// Helper to get an attribute value from SubjectName dictionary safely.
    /// </summary>
    /// <param name="key">OID or attribute key (e.g. "CN", "1.2.860.3.16.1.2").</param>
    /// <returns>Attribute value or null if not found.</returns>
    public string? GetSubjectAttribute(string key)
    {
        if (SubjectName == null || string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        return SubjectName.TryGetValue(key, out var value) ? value : null;
    }
}
