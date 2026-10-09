import { EImzoMobileStatusCode } from '../enums/mobile-status-code.js';
import type { EImzoResponse, SubjectCertificateData } from './common.js';
import { SubjectCertificateInfo, wrapSubjectCertificate } from './common.js';

/**
 * Raw data interface for MobileAuthResponse.
 */
export interface MobileAuthResponseData extends EImzoResponse {
  siteId?: string;
  documentId?: string;
  challange?: string;
  challenge?: string;
}

/**
 * Response returned by POST /frontend/mobile/auth.
 */
export class MobileAuthResponse implements MobileAuthResponseData {
  public status: number;
  public message?: string;
  public siteId?: string;
  public documentId?: string;
  public challange?: string;
  public challengeRaw?: string;

  constructor(data: MobileAuthResponseData) {
    this.status = data.status;
    this.message = data.message;
    this.siteId = data.siteId;
    this.documentId = data.documentId;
    this.challange = data.challange;
    this.challengeRaw = data.challenge;
  }

  /**
   * Unified getter returning the challenge string (reading either 'challange' or 'challenge').
   */
  public get challenge(): string | undefined {
    return this.challange ?? this.challengeRaw;
  }
}

/**
 * Response returned by POST /frontend/mobile/sign.
 */
export interface MobileSignResponse extends EImzoResponse {
  siteId?: string;
  documentId?: string;
}

/**
 * Raw data interface for MobileStatusResponse.
 */
export interface MobileStatusResponseData extends EImzoResponse {}

/**
 * Response returned by POST /frontend/mobile/status polling endpoint.
 */
export class MobileStatusResponse implements MobileStatusResponseData {
  public status: number;
  public message?: string;

  constructor(data: MobileStatusResponseData) {
    this.status = data.status;
    this.message = data.message;
  }

  /**
   * Status code as strongly typed enum for mobile ID-CARD flow.
   */
  public get statusCode(): EImzoMobileStatusCode {
    return this.status in EImzoMobileStatusCode
      ? (this.status as EImzoMobileStatusCode)
      : EImzoMobileStatusCode.Unknown;
  }

  /**
   * Indicates whether mobile signing or authentication was completed (Status == 1).
   */
  public get isCompleted(): boolean {
    return this.status === EImzoMobileStatusCode.Success;
  }

  /**
   * Indicates whether mobile app is still waiting for user action or upload (Status == 2).
   */
  public get isPending(): boolean {
    return this.status === EImzoMobileStatusCode.PendingUpload;
  }
}

/**
 * Parameters for POST /frontend/mobile/upload endpoint.
 */
export interface MobileUploadRequest {
  documentId: string;
  pkcs7Base64: string;
  serialNumber: string;
}

/**
 * Verification details returned by mobile verification.
 */
export interface MobileVerificationInfo {
  policyIdentifiers?: string[];
  signingTime?: string;
  timestampedTime?: string;
}

/**
 * Parameters for POST /backend/mobile/verify endpoint.
 */
export interface MobileVerifyRequest {
  documentId: string;
  documentBase64: string;
}

/**
 * Raw data interface for MobileVerifyResponse.
 */
export interface MobileVerifyResponseData extends EImzoResponse {
  subjectCertificateInfo?: SubjectCertificateData;
  verificationInfo?: MobileVerificationInfo;
  pkcs7Attached?: string;
}

export class MobileVerifyResponse implements MobileVerifyResponseData {
  public status: number;
  public message?: string;
  public subjectCertificateInfo?: SubjectCertificateInfo;
  public verificationInfo?: MobileVerificationInfo;
  public pkcs7Attached?: string;

  constructor(data: MobileVerifyResponseData) {
    this.status = data.status;
    this.message = data.message;
    this.subjectCertificateInfo = wrapSubjectCertificate(data.subjectCertificateInfo) as
      | SubjectCertificateInfo
      | undefined;
    this.verificationInfo = data.verificationInfo;
    this.pkcs7Attached = data.pkcs7Attached;
  }
}

/**
 * Raw data interface for MobileAuthenticateResponse.
 */
export interface MobileAuthenticateResponseData extends EImzoResponse {
  subjectCertificateInfo?: SubjectCertificateData;
}

export class MobileAuthenticateResponse implements MobileAuthenticateResponseData {
  public status: number;
  public message?: string;
  public subjectCertificateInfo?: SubjectCertificateInfo;

  constructor(data: MobileAuthenticateResponseData) {
    this.status = data.status;
    this.message = data.message;
    this.subjectCertificateInfo = wrapSubjectCertificate(data.subjectCertificateInfo) as
      | SubjectCertificateInfo
      | undefined;
  }
}
