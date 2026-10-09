import type { EImzoResponse, SignerInfo, SubjectCertificateData } from './common.js';
import { SubjectCertificateInfo, wrapSubjectCertificate } from './common.js';
import { EImzoKeyType } from '../enums/key-type.js';

export interface Pkcs7InfoData {
  documentBase64?: string;
  signers?: SignerInfo[];
}

export class Pkcs7Info implements Pkcs7InfoData {
  public documentBase64?: string;
  public signers?: SignerInfo[];

  constructor(data?: Pkcs7InfoData) {
    if (data) {
      this.documentBase64 = data.documentBase64;
      this.signers = data.signers?.map((signer) => ({
        ...signer,
        certificate: wrapSubjectCertificate(signer.certificate),
      }));
    }
  }

  /**
   * Primary signer (the first signer in the list).
   */
  public get primarySigner(): SignerInfo | undefined {
    return this.signers && this.signers.length > 0 ? this.signers[0] : undefined;
  }

  /**
   * Primary signer certificate.
   */
  public get primaryCertificate(): SubjectCertificateInfo | undefined {
    const cert = this.primarySigner?.certificate;
    if (!cert) return undefined;
    return Array.isArray(cert) ? (cert[0] as SubjectCertificateInfo) : (cert as SubjectCertificateInfo);
  }

  /**
   * Type of digital signature key or hardware token of the primary signer.
   */
  public get keyType(): EImzoKeyType {
    return this.primaryCertificate?.keyType ?? EImzoKeyType.Unknown;
  }

  /**
   * Returns true if all signers are verified.
   */
  public get isAllSignersValid(): boolean {
    return Boolean(
      this.signers &&
        this.signers.length > 0 &&
        this.signers.every((s) => s.verified === true)
    );
  }
}

/**
 * Response returned by POST /frontend/timestamp/pkcs7.
 */
export interface AttachTimestampResponseData extends EImzoResponse {
  pkcs7b64?: string;
  timestampedSignerList?: SubjectCertificateData[];
}

export class AttachTimestampResponse implements AttachTimestampResponseData {
  public status: number;
  public message?: string;
  public pkcs7b64?: string;
  public timestampedSignerList?: SubjectCertificateInfo[];

  constructor(data: AttachTimestampResponseData) {
    this.status = data.status;
    this.message = data.message;
    this.pkcs7b64 = data.pkcs7b64;
    this.timestampedSignerList = wrapSubjectCertificate(data.timestampedSignerList) as
      | SubjectCertificateInfo[]
      | undefined;
  }
}

/**
 * Response returned by POST /frontend/pkcs7/make-attached.
 */
export interface MakeAttachedResponse extends EImzoResponse {
  pkcs7b64?: string;
}

/**
 * Response returned by POST /frontend/pkcs7/join.
 */
export interface JoinAttachedResponse extends EImzoResponse {
  pkcs7b64?: string;
}

/**
 * Response returned by POST /backend/pkcs7/verify/attached.
 */
export interface AttachedVerifyResponseData extends EImzoResponse {
  pkcs7Info?: Pkcs7InfoData;
}

export class AttachedVerifyResponse implements AttachedVerifyResponseData {
  public status: number;
  public message?: string;
  public pkcs7Info?: Pkcs7Info;

  constructor(data: AttachedVerifyResponseData) {
    this.status = data.status;
    this.message = data.message;
    this.pkcs7Info = data.pkcs7Info ? new Pkcs7Info(data.pkcs7Info) : undefined;
  }
}

/**
 * Response returned by POST /backend/pkcs7/verify/detached.
 */
export interface DetachedVerifyResponseData extends EImzoResponse {
  pkcs7Info?: Pkcs7InfoData;
}

export class DetachedVerifyResponse implements DetachedVerifyResponseData {
  public status: number;
  public message?: string;
  public pkcs7Info?: Pkcs7Info;

  constructor(data: DetachedVerifyResponseData) {
    this.status = data.status;
    this.message = data.message;
    this.pkcs7Info = data.pkcs7Info ? new Pkcs7Info(data.pkcs7Info) : undefined;
  }
}
