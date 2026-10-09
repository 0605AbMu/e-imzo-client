using System.Text.Json.Serialization;
using EImzo.Client.Models.Common;

namespace EImzo.Client.Models.Pkcs7;

/// <summary>
/// Detailed verification information for a PKCS#7 container.
/// </summary>
public class Pkcs7Info
{
    /// <summary>
    /// The original signed document encoded in Base64 (only present for Attached signatures).
    /// </summary>
    [JsonPropertyName("documentBase64")]
    public string? DocumentBase64 { get; set; }

    /// <summary>
    /// List of signers who signed the document.
    /// </summary>
    [JsonPropertyName("signers")]
    public List<SignerInfo>? Signers { get; set; }

    /// <summary>
    /// Gets the primary signer (the first signer in the list).
    /// </summary>
    [JsonIgnore]
    public SignerInfo? PrimarySigner => Signers != null && Signers.Count > 0 ? Signers[0] : null;

    /// <summary>
    /// Returns true if all signers' signatures and certificates are verified.
    /// </summary>
    [JsonIgnore]
    public bool IsAllSignersValid => Signers != null && Signers.Count > 0 && Signers.TrueForAll(s => s.Verified && s.CertificateVerified);
}
