namespace EImzo.Client.Enums;

/// <summary>
/// Status codes returned by E-IMZO-SERVER endpoints.
/// </summary>
public enum EImzoStatusCode
{
    /// <summary>
    /// Operation succeeded.
    /// </summary>
    Success = 1,

    /// <summary>
    /// Failed to verify certificate status. Often due to missing or disconnected VPN.
    /// </summary>
    VpnOrCertificateCheckFailed = -1,

    /// <summary>
    /// Signing time is outside the allowed window (auth.service.valid.minutes.window).
    /// </summary>
    TimestampWindowExceeded = -5,

    /// <summary>
    /// Digital signature (EDS / PKCS#7) is invalid.
    /// </summary>
    SignatureInvalid = -10,

    /// <summary>
    /// Certificate is invalid or revoked.
    /// </summary>
    CertificateInvalid = -11,

    /// <summary>
    /// Certificate was not valid at the time of signing.
    /// </summary>
    CertificateInvalidAtSigningTime = -12,

    /// <summary>
    /// Challenge not found or has expired (or timestamp certificate status check failed).
    /// </summary>
    ChallengeNotFoundOrExpired = -20,

    /// <summary>
    /// Timestamp signature or hash is invalid.
    /// </summary>
    TimestampSignatureOrHashInvalid = -21,

    /// <summary>
    /// Timestamp certificate is invalid.
    /// </summary>
    TimestampCertificateInvalid = -22,

    /// <summary>
    /// Timestamp certificate was not valid at the time of signing.
    /// </summary>
    TimestampCertificateInvalidAtSigningTime = -23,

    /// <summary>
    /// Certificate policy is not in the allowed list.
    /// </summary>
    CertificatePolicyDisallowed = -24,

    /// <summary>
    /// Certification Authority (CA) certificate is not permitted for verification.
    /// </summary>
    CaCertificateDisallowed = -25,

    /// <summary>
    /// Unknown or unmapped error code.
    /// </summary>
    Unknown = -999
}
