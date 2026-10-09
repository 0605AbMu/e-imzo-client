using System.Text.Json.Serialization;

namespace EImzo.Client.Models.Common;

/// <summary>
/// Public key details of a certificate.
/// </summary>
public class CertificatePublicKey
{
    [JsonPropertyName("keyAlgName")]
    public string? KeyAlgName { get; set; }

    [JsonPropertyName("publicKey")]
    public string? PublicKey { get; set; }
}

/// <summary>
/// Digital signature details of a certificate.
/// </summary>
public class CertificateSignature
{
    [JsonPropertyName("signAlgName")]
    public string? SignAlgName { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}
