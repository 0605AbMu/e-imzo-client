import { describe, it, expect, vi } from 'vitest';
import {
  EImzoClient,
  EImzoApiException,
  EImzoValidationError,
  EImzoStatusCode,
  EImzoMobileStatusCode,
} from '../src/index.js';

function createMockFetch(responseBody: any, status = 200) {
  let capturedUrl: string | undefined;
  let capturedInit: RequestInit | undefined;

  const mockFetch = vi.fn().mockImplementation(async (url: string, init?: RequestInit) => {
    capturedUrl = url;
    capturedInit = init;

    return {
      ok: status >= 200 && status < 300,
      status,
      statusText: status === 200 ? 'OK' : 'Error',
      text: async () => (typeof responseBody === 'string' ? responseBody : JSON.stringify(responseBody)),
      json: async () => (typeof responseBody === 'string' ? JSON.parse(responseBody) : responseBody),
    } as unknown as Response;
  });

  return {
    fetch: mockFetch,
    getLastUrl: () => capturedUrl,
    getLastInit: () => capturedInit,
  };
}

describe('EImzoClient Endpoint Tests', () => {
  const baseUrl = 'http://127.0.0.1:8080/';

  it('ping retrieves serverDateTime and vpnKeyInfo', async () => {
    const mock = createMockFetch({
      status: 1,
      serverDateTime: '2022-10-06 16:47:29',
      yourIP: '127.0.0.1',
      vpnKeyInfo: {
        serialNumber: '3',
        X500Name: 'CN=Client',
      },
    });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    const res = await client.ping();

    expect(res.status).toBe(1);
    expect(res.yourIP).toBe('127.0.0.1');
    expect(res.vpnKeyInfo?.serialNumber).toBe('3');
    expect(mock.getLastUrl()).toBe('http://127.0.0.1:8080/ping');
    expect(mock.getLastInit()?.method).toBe('GET');
  });

  it('getInfo retrieves server info and trusted certificates', async () => {
    const mock = createMockFetch({
      status: 1,
      version: '1.11.4',
      serverTime: '2025.10.21 11:13:57',
      trustedCertificates: [{ serialNumber: 'c48c6d327cb85a03' }],
    });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    const res = await client.getInfo();

    expect(res.version).toBe('1.11.4');
    expect(res.trustedCertificates?.length).toBe(1);
    expect(mock.getLastUrl()).toBe('http://127.0.0.1:8080/info');
  });

  it('getChallenge generates challenge and returns ttl', async () => {
    const mock = createMockFetch({
      status: 1,
      challenge: '9b573e40-cefd-42cc-a534-f6e78b27c2ae',
      ttl: 120,
    });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    const res = await client.getChallenge();

    expect(res.challenge).toBe('9b573e40-cefd-42cc-a534-f6e78b27c2ae');
    expect(res.ttl).toBe(120);
    expect(mock.getLastUrl()).toBe('http://127.0.0.1:8080/frontend/challenge');
  });

  it('attachTimestamp posts pkcs7 and parses timestamped signers', async () => {
    const mock = createMockFetch({
      status: 1,
      pkcs7b64: 'MIAGCSqGSIb3DQEHAqCAMIACAQEx...',
      timestampedSignerList: [
        {
          serialNumber: '12345',
          subjectName: {
            CN: 'TEST USER',
          },
        },
      ],
    });

    const client = new EImzoClient({
      baseUrl,
      defaultHost: 'test.uz',
      defaultRealIp: '1.2.3.4',
      fetch: mock.fetch,
    });

    const res = await client.attachTimestamp('FAKE_PKCS7');
    expect(res.pkcs7b64).toBeDefined();
    expect(res.timestampedSignerList?.[0]?.commonName).toBe('TEST USER');

    const headers = mock.getLastInit()?.headers as Headers;
    expect(headers.get('Host')).toBe('test.uz');
    expect(headers.get('X-Real-IP')).toBe('1.2.3.4');
    expect(mock.getLastInit()?.body).toBe('FAKE_PKCS7');
  });

  it('makeAttached formats documentBase64|pkcs7DetachedBase64 body', async () => {
    const mock = createMockFetch({
      status: 1,
      pkcs7b64: 'ATTACHED_PKCS7',
    });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    const res = await client.makeAttached('DOC_B64', 'DETACHED_B64');

    expect(res.pkcs7b64).toBe('ATTACHED_PKCS7');
    expect(mock.getLastInit()?.body).toBe('DOC_B64|DETACHED_B64');
  });

  it('joinAttached joins two attached documents', async () => {
    const mock = createMockFetch({
      status: 1,
      pkcs7b64: 'JOINED_PKCS7',
    });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    const res = await client.joinAttached('DOC_A', 'DOC_B');

    expect(res.pkcs7b64).toBe('JOINED_PKCS7');
    expect(mock.getLastInit()?.body).toBe('DOC_A|DOC_B');
  });

  it('authenticate parses certificate and Uzbek PINFL/TIN', async () => {
    const mock = createMockFetch({
      status: 1,
      subjectCertificateInfo: {
        serialNumber: '445566',
        subjectName: {
          '1.2.860.3.16.1.2': '30101900000001',
          '1.2.860.3.16.1.1': '987654321',
          CN: 'DIRECTOR ALI',
        },
      },
    });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    const res = await client.authenticate('SIGNED_PKCS7');

    expect(res.subjectCertificateInfo?.pinfl).toBe('30101900000001');
    expect(res.subjectCertificateInfo?.legalEntityTin).toBe('987654321');
    expect(res.subjectCertificateInfo?.isLegalEntity).toBe(true);
  });

  it('verifyAttached returns signers and verification status', async () => {
    const mock = createMockFetch({
      status: 1,
      pkcs7Info: {
        documentBase64: 'HELLO_WORLD_B64',
        signers: [
          {
            verified: true,
            certificate: {
              serialNumber: 'AAA',
              subjectName: { CN: 'SIGNER ONE' },
            },
          },
        ],
      },
    });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    const res = await client.verifyAttached('ATTACHED_DATA');

    expect(res.pkcs7Info?.isAllSignersValid).toBe(true);
    expect(res.pkcs7Info?.primarySigner?.verified).toBe(true);
    expect(res.pkcs7Info?.documentBase64).toBe('HELLO_WORLD_B64');
  });

  it('verifyDetached parses detached verification', async () => {
    const mock = createMockFetch({
      status: 1,
      pkcs7Info: {
        signers: [{ verified: true }],
      },
    });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    const res = await client.verifyDetached('DOC', 'SIG');

    expect(res.pkcs7Info?.isAllSignersValid).toBe(true);
  });

  it('mobileAuth handles both challenge and challange spellings', async () => {
    const mock = createMockFetch({
      status: 1,
      siteId: '0000',
      documentId: 'DOC123',
      challange: 'RANDOM_CHALLENGE_HEX',
    });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    const res = await client.mobileAuth();

    expect(res.siteId).toBe('0000');
    expect(res.documentId).toBe('DOC123');
    expect(res.challenge).toBe('RANDOM_CHALLENGE_HEX');
  });

  it('mobileSign returns siteId and documentId', async () => {
    const mock = createMockFetch({
      status: 1,
      siteId: '0000',
      documentId: 'DOC_SIGN_456',
    });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    const res = await client.mobileSign();

    expect(res.siteId).toBe('0000');
    expect(res.documentId).toBe('DOC_SIGN_456');
  });

  it('getMobileStatus does not throw on status = 2 (PendingUpload)', async () => {
    const mock = createMockFetch({
      status: 2,
      message: 'Pending mobile app action',
    });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    const res = await client.getMobileStatus('DOC123');

    expect(res.status).toBe(2);
    expect(res.isPending).toBe(true);
    expect(res.isCompleted).toBe(false);
    expect(res.statusCode).toBe(EImzoMobileStatusCode.PendingUpload);
  });

  it('getMobileStatus throws on negative status code when throwOnError is true', async () => {
    const mock = createMockFetch({
      status: -2,
      message: 'Document expired',
    });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    await expect(client.getMobileStatus('DOC123')).rejects.toThrow(EImzoApiException);
  });

  it('uploadMobilePkcs7 sends form parameters', async () => {
    const mock = createMockFetch({ status: 1 });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    const res = await client.uploadMobilePkcs7('DOC1', 'PKCS7_DATA', 'SN99');

    expect(res.status).toBe(1);
    expect(mock.getLastInit()?.body).toContain('document_id=DOC1');
    expect(mock.getLastInit()?.body).toContain('pkcs7_b64=PKCS7_DATA');
    expect(mock.getLastInit()?.body).toContain('serial_number=SN99');
  });

  it('mobileAuthenticate verifies mobile auth on backend', async () => {
    const mock = createMockFetch({
      status: 1,
      subjectCertificateInfo: {
        serialNumber: '112233',
        subjectName: { CN: 'MOBILE USER' },
      },
    });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    const res = await client.mobileAuthenticate('DOC123');

    expect(res.subjectCertificateInfo?.commonName).toBe('MOBILE USER');
    expect(mock.getLastUrl()).toBe('http://127.0.0.1:8080/backend/mobile/authenticate/DOC123');
  });

  it('mobileVerify accepts document string or Uint8Array bytes', async () => {
    const mock = createMockFetch({
      status: 1,
      subjectCertificateInfo: {
        subjectName: { CN: 'SIGNER' },
      },
      pkcs7Attached: 'ATTACHED_RESULT',
    });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    const bytes = new TextEncoder().encode('Hello Document');
    const res = await client.mobileVerify('DOC123', bytes);

    expect(res.pkcs7Attached).toBe('ATTACHED_RESULT');
    expect(mock.getLastInit()?.body).toContain('documentId=DOC123');
    expect(mock.getLastInit()?.body).toContain('document=SGVsbG8gRG9jdW1lbnQ%3D');
  });

  it('throws EImzoApiException when server returns non-success status', async () => {
    const mock = createMockFetch({
      status: -10,
      message: 'Digital signature invalid',
    });

    const client = new EImzoClient({ baseUrl, fetch: mock.fetch });
    await expect(client.authenticate('BAD_SIGNATURE')).rejects.toThrow(EImzoApiException);
  });

  it('does not throw when throwOnError is false', async () => {
    const mock = createMockFetch({
      status: -10,
      message: 'Digital signature invalid',
    });

    const client = new EImzoClient({ baseUrl, throwOnError: false, fetch: mock.fetch });
    const res = await client.authenticate('BAD_SIGNATURE');
    expect(res.status).toBe(-10);
  });

  it('throws EImzoValidationError on empty required parameters', async () => {
    const client = new EImzoClient({ baseUrl });
    await expect(client.authenticate('')).rejects.toThrow(EImzoValidationError);
    await expect(client.attachTimestamp('   ')).rejects.toThrow(EImzoValidationError);
    await expect(client.makeAttached('', 'SIG')).rejects.toThrow(EImzoValidationError);
  });
});
