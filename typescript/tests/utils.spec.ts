import { describe, it, expect } from 'vitest';
import {
  computeCrc32,
  hexToBytes,
  bytesToHex,
  computeHexFromHexString,
  buildQrCode,
  buildDeepLink,
  verifyQrCode,
  parseEImzoDate,
  SubjectCertificateInfo,
  UZBEK_OIDS,
  EImzoValidationError,
  EImzoKeyType,
  resolveKeyType,
} from '../src/index.js';

describe('CRC32 and Mobile Utilities', () => {
  it('computes correct standard CRC32 for "123456789"', () => {
    const ascii = new TextEncoder().encode('123456789');
    const crc = computeCrc32(ascii);
    // 0xCBF43926 in decimal is 3421780262
    expect(crc).toBe(0xcbf43926);
  });

  it('converts hex to bytes and back accurately', () => {
    const hex = '01020304';
    const bytes = hexToBytes(hex);
    expect(bytes.length).toBe(4);
    expect(bytes[0]).toBe(1);
    expect(bytes[3]).toBe(4);

    const back = bytesToHex(bytes);
    expect(back).toBe(hex);
  });

  it('throws on invalid or odd length hex strings', () => {
    expect(() => hexToBytes('123')).toThrow(EImzoValidationError);
    expect(() => hexToBytes('12zz')).toThrow(EImzoValidationError);
  });

  it('buildQrCode generates valid payload with CRC32', () => {
    const siteId = '0000';
    const docId = '2944F1F2';
    const hexHash = 'F8D2181DC6C02EA819B88FF3EF49BE0C';

    const qrCode = buildQrCode(siteId, docId, hexHash);
    expect(qrCode).toBeDefined();
    expect(qrCode.startsWith(siteId + docId + hexHash)).toBe(true);
    expect(qrCode.length).toBe(siteId.length + docId.length + hexHash.length + 8);
    expect(verifyQrCode(qrCode)).toBe(true);
  });

  it('buildDeepLink formats eimzo:// URL correctly', () => {
    const siteId = '0000';
    const docId = '2944F1F2';
    const hexHash = 'F8D2181DC6C02EA819B88FF3EF49BE0C';

    const deepLink = buildDeepLink(siteId, docId, hexHash);
    expect(deepLink.startsWith('eimzo://sign?qc=')).toBe(true);

    const qrCode = deepLink.replace('eimzo://sign?qc=', '');
    expect(verifyQrCode(qrCode)).toBe(true);
  });

  it('verifyQrCode rejects corrupted or too short payloads', () => {
    const siteId = '0000';
    const docId = '2944F1F2';
    const hexHash = 'F8D2181DC6C02EA819B88FF3EF49BE0C';

    const qrCode = buildQrCode(siteId, docId, hexHash);
    const corrupted = qrCode.substring(0, qrCode.length - 1) + 'X';

    expect(verifyQrCode(corrupted)).toBe(false);
    expect(verifyQrCode('short')).toBe(false);
    expect(verifyQrCode('')).toBe(false);
  });

  it('buildQrCode throws on missing inputs', () => {
    expect(() => buildQrCode('', 'doc', 'hash')).toThrow(EImzoValidationError);
    expect(() => buildQrCode('site', '', 'hash')).toThrow(EImzoValidationError);
    expect(() => buildQrCode('site', 'doc', '')).toThrow(EImzoValidationError);
  });
});

describe('SubjectCertificateInfo and Uzbek OID Mapping', () => {
  it('maps physical person certificate fields properly', () => {
    const cert = new SubjectCertificateInfo({
      serialNumber: '218711a92',
      X500Name: 'CN=ALIEV VALI,UID=400000000,1.2.860.3.16.1.2=30000000000000',
      subjectName: {
        [UZBEK_OIDS.PINFL]: '30000000000000',
        [UZBEK_OIDS.PHYSICAL_PERSON_TIN]: '400000000',
        CN: 'ALIEV VALI',
        Name: 'VALI',
        SURNAME: 'ALIEV',
        C: 'UZ',
      },
    });

    expect(cert.pinfl).toBe('30000000000000');
    expect(cert.physicalPersonTin).toBe('400000000');
    expect(cert.legalEntityTin).toBeUndefined();
    expect(cert.tin).toBe('400000000');
    expect(cert.commonName).toBe('ALIEV VALI');
    expect(cert.firstName).toBe('VALI');
    expect(cert.surname).toBe('ALIEV');
    expect(cert.country).toBe('UZ');
    expect(cert.isLegalEntity).toBe(false);
    expect(cert.isPhysicalPerson).toBe(true);
  });

  it('maps legal entity certificate fields properly', () => {
    const cert = new SubjectCertificateInfo({
      serialNumber: '99887766',
      subjectName: {
        [UZBEK_OIDS.PINFL]: '31111111111111',
        [UZBEK_OIDS.LEGAL_ENTITY_TIN]: '123456789',
        UID: '31111111111111',
        CN: 'DIRECTOR NAME',
        O: 'OOO "INNOVATION"',
      },
    });

    expect(cert.pinfl).toBe('31111111111111');
    expect(cert.legalEntityTin).toBe('123456789');
    expect(cert.tin).toBe('123456789');
    expect(cert.organization).toBe('OOO "INNOVATION"');
    expect(cert.isLegalEntity).toBe(true);
    expect(cert.isPhysicalPerson).toBe(false);
  });

  it('safely handles empty subjectName', () => {
    const cert = new SubjectCertificateInfo();
    expect(cert.pinfl).toBeUndefined();
    expect(cert.legalEntityTin).toBeUndefined();
    expect(cert.physicalPersonTin).toBeUndefined();
    expect(cert.tin).toBeUndefined();
    expect(cert.isLegalEntity).toBe(false);
    expect(cert.isPhysicalPerson).toBe(true);
    expect(cert.keyType).toBe(EImzoKeyType.Unknown);
  });

  it('determines keyType from publicKeyParameter paramSetOID correctly', () => {
    const cert = new SubjectCertificateInfo({
      serialNumber: '218712ed3',
      publicKeyParameter: {
        keyAlgName: 'OZMST-286-2024-2',
        paramSetOID: '1.2.860.3.15.2.1.2.1.1',
      },
    });

    expect(cert.keyType).toBe(EImzoKeyType.Pfx);
    expect(cert.publicKeyParameter?.paramSetOID).toBe('1.2.860.3.15.2.1.2.1.1');
  });

  it('resolves all known key types from paramSetOID', () => {
    expect(resolveKeyType('1.2.860.3.15.1.1.2.1.1')).toBe(EImzoKeyType.Pfx);
    expect(resolveKeyType('1.2.860.3.15.2.1.2.1.1')).toBe(EImzoKeyType.Pfx);
    expect(resolveKeyType('1.2.860.3.15.1.1.2.1.3')).toBe(EImzoKeyType.IdCard);
    expect(resolveKeyType('1.2.860.3.15.2.1.2.1.3')).toBe(EImzoKeyType.IdCard);
    expect(resolveKeyType('1.2.860.3.15.2.1.2.1.2')).toBe(EImzoKeyType.Baik);
    expect(resolveKeyType('1.2.860.3.15.2.1.2.1.4')).toBe(EImzoKeyType.Uzguard);
    expect(resolveKeyType('1.2.3.4.5')).toBe(EImzoKeyType.Unknown);
    expect(resolveKeyType('')).toBe(EImzoKeyType.Unknown);
    expect(resolveKeyType(undefined)).toBe(EImzoKeyType.Unknown);
  });
});

describe('Date Parsing Utility', () => {
  it('parses standard E-IMZO date formats', () => {
    const d1 = parseEImzoDate('2022-10-06 16:47:29');
    expect(d1).toBeDefined();
    expect(d1?.getFullYear()).toBe(2022);
    expect(d1?.getMonth()).toBe(9); // 0-indexed month: October

    const d2 = parseEImzoDate('2025.10.21 11:13:57');
    expect(d2).toBeDefined();
    expect(d2?.getFullYear()).toBe(2025);

    expect(parseEImzoDate('')).toBeUndefined();
    expect(parseEImzoDate(null)).toBeUndefined();
  });
});
