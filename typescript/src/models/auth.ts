import type { CertificatePublicKey, EImzoResponse, SubjectCertificateData } from './common.js';
import { SubjectCertificateInfo, wrapSubjectCertificate, resolveKeyType } from './common.js';
import { EImzoKeyType } from '../enums/key-type.js';

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

  /**
   * Public key parameters if returned at root response level.
   */
  publicKeyParameter?: CertificatePublicKey;

  /**
   * Direct parameter set OID if returned at root response level.
   */
  paramSetOID?: string;
}

export class AuthResponse implements AuthResponseData {
  public status: number;
  public message?: string;
  public subjectCertificateInfo?: SubjectCertificateInfo;
  public publicKeyParameter?: CertificatePublicKey;
  public paramSetOID?: string;

  constructor(data: AuthResponseData) {
    this.status = data.status;
    this.message = data.message;
    this.subjectCertificateInfo = wrapSubjectCertificate(data.subjectCertificateInfo) as
      | SubjectCertificateInfo
      | undefined;
    this.publicKeyParameter = data.publicKeyParameter;
    this.paramSetOID = data.paramSetOID;
  }

  /**
   * Type of digital signature key or hardware token (Pfx, IdCard, Baik, Uzguard) determined by paramSetOID.
   */
  public get keyType(): EImzoKeyType {
    const rootOid = this.paramSetOID ?? this.publicKeyParameter?.paramSetOID;
    if (rootOid) {
      return resolveKeyType(rootOid);
    }
    return this.subjectCertificateInfo?.keyType ?? EImzoKeyType.Unknown;
  }
}

