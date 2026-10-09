import { EImzoKeyType } from '../enums/key-type.js';

/**
 * OID constants for Uzbek digital signature certificate attributes.
 */
export const UZBEK_OIDS = {
  /**
   * OID for Uzbek Individual Personal Identification Number (JSHSHIR / PINFL - 14 digits).
   */
  PINFL: '1.2.860.3.16.1.2',

  /**
   * OID for Uzbek Legal Entity Tax Identification Number (STIR / INN - 9 digits).
   */
  LEGAL_ENTITY_TIN: '1.2.860.3.16.1.1',

  /**
   * Attribute name for Physical Person Tax Identification Number (UID).
   */
  PHYSICAL_PERSON_TIN: 'UID',
} as const;

/**
 * Parameter set OIDs identifying cryptographic parameter sets and key / token types.
 */
export const EIMZO_PARAM_SET_OIDS = {
  PFX: ['1.2.860.3.15.1.1.2.1.1', '1.2.860.3.15.2.1.2.1.1'],
  ID_CARD: ['1.2.860.3.15.1.1.2.1.3', '1.2.860.3.15.2.1.2.1.3'],
  BAIK: ['1.2.860.3.15.2.1.2.1.2'],
  UZGUARD: ['1.2.860.3.15.2.1.2.1.4'],
} as const;

/**
 * Resolves the key type (PFX, IdCard, Baik, Uzguard) based on paramSetOID.
 */
export function resolveKeyType(paramSetOid?: string): EImzoKeyType {
  if (!paramSetOid) {
    return EImzoKeyType.Unknown;
  }
  const trimmed = paramSetOid.trim();
  if ((EIMZO_PARAM_SET_OIDS.PFX as readonly string[]).includes(trimmed)) {
    return EImzoKeyType.Pfx;
  }
  if ((EIMZO_PARAM_SET_OIDS.ID_CARD as readonly string[]).includes(trimmed)) {
    return EImzoKeyType.IdCard;
  }
  if ((EIMZO_PARAM_SET_OIDS.BAIK as readonly string[]).includes(trimmed)) {
    return EImzoKeyType.Baik;
  }
  if ((EIMZO_PARAM_SET_OIDS.UZGUARD as readonly string[]).includes(trimmed)) {
    return EImzoKeyType.Uzguard;
  }
  return EImzoKeyType.Unknown;
}

/**
 * Base response from E-IMZO-SERVER endpoints.
 */
export interface EImzoResponse<T = unknown> {
  /**
   * Numeric status code (1 = Success, negative numbers indicate specific errors).
   */
  status: number;

  /**
   * Error or status message returned by server.
   */
  message?: string;

  /**
   * Optional payload data.
   */
  data?: T;
}

/**
 * VPN key information returned by ping and info endpoints.
 */
export interface VpnKeyInfo {
  serialNumber?: string;
  X500Name?: string;
  validFrom?: string;
  validTo?: string;
}

/**
 * Trusted certificate information returned by info endpoint.
 */
export interface TrustedCertificateInfo {
  serialNumber?: string;
  validFrom?: string;
  validTo?: string;
}

/**
 * Certificate public key parameters and algorithms.
 */
export interface CertificatePublicKey {
  x?: string;
  y?: string;
  keyAlgName?: string;
  publicKey?: string;
  paramSetOID?: string;
}

/**
 * Raw data interface for Subject Certificate.
 */
export interface SubjectCertificateData {
  serialNumber?: string;
  X500Name?: string;
  subjectName?: Record<string, string>;
  validFrom?: string;
  validTo?: string;
  publicKeyParameter?: CertificatePublicKey;
  publicKey?: CertificatePublicKey;
  paramSetOID?: string;
}

/**
 * Enhanced Subject Certificate Info with Uzbek OID helper properties.
 */
export class SubjectCertificateInfo implements SubjectCertificateData {
  public serialNumber?: string;
  public X500Name?: string;
  public subjectName?: Record<string, string>;
  public validFrom?: string;
  public validTo?: string;
  public publicKeyParameter?: CertificatePublicKey;
  public publicKey?: CertificatePublicKey;
  public paramSetOID?: string;

  constructor(data?: SubjectCertificateData) {
    if (data) {
      this.serialNumber = data.serialNumber;
      this.X500Name = data.X500Name;
      this.subjectName = data.subjectName;
      this.validFrom = data.validFrom;
      this.validTo = data.validTo;
      this.publicKeyParameter = data.publicKeyParameter;
      this.publicKey = data.publicKey;
      this.paramSetOID = data.paramSetOID;
    }
  }

  /**
   * Type of digital signature key or hardware token (Pfx, IdCard, Baik, Uzguard) determined by paramSetOID.
   */
  public get keyType(): EImzoKeyType {
    const oid = this.publicKeyParameter?.paramSetOID ?? this.publicKey?.paramSetOID ?? this.paramSetOID;
    return resolveKeyType(oid);
  }

  /**
   * Retrieves an attribute from subjectName by key (case-insensitive fallback).
   */
  public getSubjectAttribute(key: string): string | undefined {
    if (!this.subjectName) {
      return undefined;
    }

    if (key in this.subjectName) {
      return this.subjectName[key];
    }

    const lowerKey = key.toLowerCase();
    for (const [k, v] of Object.entries(this.subjectName)) {
      if (k.toLowerCase() === lowerKey) {
        return v;
      }
    }

    return undefined;
  }

  /**
   * 14-digit Individual Personal Identification Number (JSHSHIR / PINFL), retrieved via OID 1.2.860.3.16.1.2.
   */
  public get pinfl(): string | undefined {
    return this.getSubjectAttribute(UZBEK_OIDS.PINFL);
  }

  /**
   * Legal Entity Tax Identification Number (STIR / INN - 9 digits), retrieved via OID 1.2.860.3.16.1.1.
   * Returns undefined if the certificate belongs to an individual.
   */
  public get legalEntityTin(): string | undefined {
    return this.getSubjectAttribute(UZBEK_OIDS.LEGAL_ENTITY_TIN);
  }

  /**
   * Physical Person Tax Identification Number (INN) retrieved via "UID".
   */
  public get physicalPersonTin(): string | undefined {
    return this.getSubjectAttribute(UZBEK_OIDS.PHYSICAL_PERSON_TIN);
  }

  /**
   * Tax Identification Number (STIR / INN). Returns LegalEntityTin if present; otherwise PhysicalPersonTin.
   */
  public get tin(): string | undefined {
    return this.legalEntityTin ?? this.physicalPersonTin;
  }

  /**
   * Common Name (CN) / Full Name of certificate owner.
   */
  public get commonName(): string | undefined {
    return this.getSubjectAttribute('CN');
  }

  /**
   * First name / Given name of certificate owner.
   */
  public get firstName(): string | undefined {
    return this.getSubjectAttribute('Name');
  }

  /**
   * Surname / Last name of certificate owner.
   */
  public get surname(): string | undefined {
    return this.getSubjectAttribute('SURNAME');
  }

  /**
   * Organization name (O), if present.
   */
  public get organization(): string | undefined {
    return this.getSubjectAttribute('O');
  }

  /**
   * Country code (C), typically "UZ".
   */
  public get country(): string | undefined {
    return this.getSubjectAttribute('C');
  }

  /**
   * Indicates whether the certificate represents an organization (legal entity).
   */
  public get isLegalEntity(): boolean {
    return Boolean(this.legalEntityTin && this.legalEntityTin.trim().length > 0);
  }

  /**
   * Indicates whether the certificate represents an individual (physical person).
   */
  public get isPhysicalPerson(): boolean {
    return !this.isLegalEntity;
  }
}

/**
 * Signer information inside PKCS#7 verification responses.
 */
export interface SignerInfo {
  certificate?: SubjectCertificateData[] | SubjectCertificateData;
  verified?: boolean;
  signingTime?: string;
  policyIdentifier?: string;
}

/**
 * Converts a raw or parsed SubjectCertificateData or array into SubjectCertificateInfo instance(s).
 */
export function wrapSubjectCertificate(
  cert?: SubjectCertificateData | SubjectCertificateData[]
): SubjectCertificateInfo | SubjectCertificateInfo[] | undefined {
  if (!cert) return undefined;
  if (Array.isArray(cert)) {
    return cert.map((c) => (c instanceof SubjectCertificateInfo ? c : new SubjectCertificateInfo(c)));
  }
  return cert instanceof SubjectCertificateInfo ? cert : new SubjectCertificateInfo(cert);
}
