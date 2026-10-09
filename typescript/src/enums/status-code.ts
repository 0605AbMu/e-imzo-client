/**
 * Status codes returned by E-IMZO-SERVER endpoints.
 */
export enum EImzoStatusCode {
  /**
   * Operation succeeded.
   */
  Success = 1,

  /**
   * Failed to verify certificate status. Often due to missing or disconnected VPN.
   */
  VpnOrCertificateCheckFailed = -1,

  /**
   * Signing time is outside the allowed window (auth.service.valid.minutes.window).
   */
  TimestampWindowExceeded = -5,

  /**
   * Digital signature (EDS / PKCS#7) is invalid.
   */
  SignatureInvalid = -10,

  /**
   * Certificate is invalid or revoked.
   */
  CertificateInvalid = -11,

  /**
   * Certificate was not valid at the time of signing.
   */
  CertificateInvalidAtSigningTime = -12,

  /**
   * Challenge not found or has expired (or timestamp certificate status check failed).
   */
  ChallengeNotFoundOrExpired = -20,

  /**
   * Timestamp signature or hash is invalid.
   */
  TimestampSignatureOrHashInvalid = -21,

  /**
   * Timestamp certificate is invalid.
   */
  TimestampCertificateInvalid = -22,

  /**
   * Timestamp certificate was not valid at the time of signing.
   */
  TimestampCertificateInvalidAtSigningTime = -23,

  /**
   * Certificate policy is not in the allowed list.
   */
  CertificatePolicyDisallowed = -24,

  /**
   * Certification Authority (CA) certificate is not permitted for verification.
   */
  CaCertificateDisallowed = -25,

  /**
   * Unknown or unmapped error code.
   */
  Unknown = -999,
}

/**
 * Returns a human-readable English description for an E-IMZO status code.
 */
export function getEImzoStatusMessage(status: number): string {
  switch (status) {
    case EImzoStatusCode.Success:
      return 'Operation completed successfully.';
    case EImzoStatusCode.VpnOrCertificateCheckFailed:
      return 'Failed to verify certificate status (VPN connection may be inactive or inaccessible).';
    case EImzoStatusCode.TimestampWindowExceeded:
      return 'Signing time is outside the allowed window.';
    case EImzoStatusCode.SignatureInvalid:
      return 'Digital signature (PKCS#7) is invalid.';
    case EImzoStatusCode.CertificateInvalid:
      return 'Certificate is invalid or revoked.';
    case EImzoStatusCode.CertificateInvalidAtSigningTime:
      return 'Certificate was not valid at the date and time of signing.';
    case EImzoStatusCode.ChallengeNotFoundOrExpired:
      return 'Challenge not found or has expired (or timestamp status check failed).';
    case EImzoStatusCode.TimestampSignatureOrHashInvalid:
      return 'Timestamp signature or hash is invalid.';
    case EImzoStatusCode.TimestampCertificateInvalid:
      return 'Timestamp certificate is invalid.';
    case EImzoStatusCode.TimestampCertificateInvalidAtSigningTime:
      return 'Timestamp certificate was not valid at the date and time of signing.';
    case EImzoStatusCode.CertificatePolicyDisallowed:
      return 'Certificate policy is not in the allowed list.';
    case EImzoStatusCode.CaCertificateDisallowed:
      return 'Certification Authority (CA) certificate is not permitted for verification.';
    default:
      return `Unknown E-IMZO error code: ${status}`;
  }
}
