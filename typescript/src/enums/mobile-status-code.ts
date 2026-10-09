/**
 * Status codes returned by E-IMZO ID-CARD Mobile REST-API endpoints (/frontend/mobile/* and /backend/mobile/*).
 */
export enum EImzoMobileStatusCode {
  /**
   * Bad or malformed response.
   */
  BadResponse = 0,

  /**
   * Operation succeeded.
   */
  Success = 1,

  /**
   * PKCS#7 has not yet been uploaded from the mobile application (status polling in progress).
   */
  PendingUpload = 2,

  /**
   * Redis error (unable to connect or cache failure).
   */
  RedisError = -1,

  /**
   * Document record by DocumentID was not found in Redis (may have expired).
   */
  DocumentNotFound = -2,

  /**
   * PKCS#7 structure is invalid, or saved PKCS#7 by DocumentID is absent.
   */
  InvalidPkcs7Structure = -4,

  /**
   * PKCS#7 digital signature is invalid.
   */
  SignatureInvalid = -5,

  /**
   * User certificate is invalid or revoked.
   */
  CertificateInvalid = -6,

  /**
   * User certificate was not valid at the date and time of signing.
   */
  CertificateInvalidAtSigningTime = -7,

  /**
   * An error occurred during certificate status verification (check server log).
   */
  CertificateStatusCheckError = -8,

  /**
   * The allowed time difference between smartphone signing time and server verification time was exceeded.
   */
  TimeWindowExceeded = -9,

  /**
   * Unexpected server error.
   */
  UnexpectedError = -10,

  /**
   * Digest mismatch: calculated digest for document does not match signerInfo digest.
   */
  DigestMismatchOrGeneralError = -99,

  /**
   * Unknown status code.
   */
  Unknown = -999,
}

/**
 * Returns a human-readable English description for an E-IMZO Mobile status code.
 */
export function getEImzoMobileStatusMessage(status: number): string {
  switch (status) {
    case EImzoMobileStatusCode.BadResponse:
      return 'Malformed response from server.';
    case EImzoMobileStatusCode.Success:
      return 'Mobile operation succeeded.';
    case EImzoMobileStatusCode.PendingUpload:
      return 'Waiting for signature upload from mobile application.';
    case EImzoMobileStatusCode.RedisError:
      return 'Redis cache connection or write error on server.';
    case EImzoMobileStatusCode.DocumentNotFound:
      return 'Document record was not found (session expired or invalid document ID).';
    case EImzoMobileStatusCode.InvalidPkcs7Structure:
      return 'PKCS#7 structure is invalid or missing.';
    case EImzoMobileStatusCode.SignatureInvalid:
      return 'Mobile digital signature is invalid.';
    case EImzoMobileStatusCode.CertificateInvalid:
      return 'Mobile certificate is invalid or revoked.';
    case EImzoMobileStatusCode.CertificateInvalidAtSigningTime:
      return 'Mobile certificate was not valid at signing time.';
    case EImzoMobileStatusCode.CertificateStatusCheckError:
      return 'Error occurred during certificate status check (check server logs).';
    case EImzoMobileStatusCode.TimeWindowExceeded:
      return 'Signing time difference between smartphone and server exceeded allowed window.';
    case EImzoMobileStatusCode.UnexpectedError:
      return 'Unexpected internal server error.';
    case EImzoMobileStatusCode.DigestMismatchOrGeneralError:
      return 'Digest mismatch: document content does not match signer digest.';
    default:
      return `Unknown mobile status code: ${status}`;
  }
}
