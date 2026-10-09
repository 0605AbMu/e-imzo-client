namespace EImzo.Client.Enums;

/// <summary>
/// Status codes returned by E-IMZO ID-CARD Mobile REST-API endpoints (/frontend/mobile/* and /backend/mobile/*).
/// </summary>
public enum EImzoMobileStatusCode
{
    /// <summary>
    /// Bad or malformed response (should not normally occur).
    /// </summary>
    BadResponse = 0,

    /// <summary>
    /// Operation succeeded.
    /// </summary>
    Success = 1,

    /// <summary>
    /// PKCS#7 has not yet been uploaded from the mobile application (status polling in progress).
    /// </summary>
    PendingUpload = 2,

    /// <summary>
    /// Redis error (unable to connect or cache failure).
    /// </summary>
    RedisError = -1,

    /// <summary>
    /// Document record by DocumentID was not found in Redis (may have expired).
    /// </summary>
    DocumentNotFound = -2,

    /// <summary>
    /// PKCS#7 structure is invalid, or saved PKCS#7 by DocumentID is absent.
    /// </summary>
    InvalidPkcs7Structure = -4,

    /// <summary>
    /// PKCS#7 digital signature is invalid.
    /// </summary>
    SignatureInvalid = -5,

    /// <summary>
    /// User certificate is invalid or revoked.
    /// </summary>
    CertificateInvalid = -6,

    /// <summary>
    /// User certificate was not valid at the date and time of signing.
    /// </summary>
    CertificateInvalidAtSigningTime = -7,

    /// <summary>
    /// An error occurred during certificate status verification (check server log).
    /// </summary>
    CertificateStatusCheckError = -8,

    /// <summary>
    /// The allowed time difference between smartphone signing time and server verification time was exceeded.
    /// </summary>
    TimeWindowExceeded = -9,

    /// <summary>
    /// Unexpected server error.
    /// </summary>
    UnexpectedError = -10,

    /// <summary>
    /// Digest mismatch: calculated digest for document does not match signerInfo digest.
    /// </summary>
    DigestMismatchOrGeneralError = -99,

    /// <summary>
    /// Unknown status code.
    /// </summary>
    Unknown = -999
}
