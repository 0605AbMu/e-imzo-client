import type { EImzoResponse, SubjectCertificateData } from './common.js';
import { SubjectCertificateInfo, wrapSubjectCertificate } from './common.js';

/**
 * Response returned by GET /frontend/challenge.
 */
export interface ChallengeResponse extends EImzoResponse {
  /**
   * Temporary unique random challenge string to be signed.
   */
  challenge?: string;

  /**
   * Time-to-live for the challenge in seconds.
   */
  ttl?: number;
}

/**
 * Response returned by POST /backend/auth.
 */
export interface AuthResponseData extends EImzoResponse {
  /**
   * Information about the subject certificate of the authenticated user.
   */
  subjectCertificateInfo?: SubjectCertificateData;
}

export class AuthResponse implements AuthResponseData {
  public status: number;
  public message?: string;
  public subjectCertificateInfo?: SubjectCertificateInfo;

  constructor(data: AuthResponseData) {
    this.status = data.status;
    this.message = data.message;
    this.subjectCertificateInfo = wrapSubjectCertificate(data.subjectCertificateInfo) as
      | SubjectCertificateInfo
      | undefined;
  }
}
