import { EImzoApiException } from '../errors/e-imzo-api-error.js';
import { EImzoValidationError } from '../errors/e-imzo-validation-error.js';
import type {
  EImzoResponse,
  PingResponse,
  ServerInfoResponse,
  ChallengeResponse,
  MakeAttachedResponse,
  JoinAttachedResponse,
  MobileSignResponse,
  MobileUploadRequest,
  MobileAuthResponseData,
  MobileAuthenticateResponseData,
} from '../models/index.js';
import {
  AuthResponse,
  AttachTimestampResponse,
  AttachedVerifyResponse,
  DetachedVerifyResponse,
  MobileAuthResponse,
  MobileStatusResponse,
  MobileVerifyResponse,
  MobileAuthenticateResponse,
} from '../models/index.js';
import type { EImzoClientOptions, RequestHeaderOptions } from './client-options.js';

export class EImzoClient {
  private readonly baseUrl: string;
  private readonly timeout: number;
  private readonly defaultHost?: string;
  private readonly defaultRealIp?: string;
  private readonly throwOnError: boolean;
  private readonly fetchImpl: typeof fetch;

  constructor(options: EImzoClientOptions) {
    if (!options.baseUrl || options.baseUrl.trim().length === 0) {
      throw new EImzoValidationError('baseUrl', 'baseUrl must be provided.');
    }

    this.baseUrl = options.baseUrl.endsWith('/') ? options.baseUrl : `${options.baseUrl}/`;
    this.timeout = options.timeout ?? 30000;
    this.defaultHost = options.defaultHost;
    this.defaultRealIp = options.defaultRealIp;
    this.throwOnError = options.throwOnError ?? true;
    this.fetchImpl = options.fetch ?? globalThis.fetch.bind(globalThis);
  }

  // ==========================================
  // Health and Diagnostic Endpoints
  // ==========================================

  /**
   * Checks VPN connection and returns server time and VPN key info (GET /ping).
   */
  public async ping(options?: { signal?: AbortSignal }): Promise<PingResponse> {
    const endpoint = 'ping';
    const response = await this.get<PingResponse>(endpoint, options);
    this.checkStatus(response, endpoint);
    return response;
  }

  /**
   * Retrieves detailed E-IMZO server information, version, VPN key, and trusted certificates (GET /info).
   */
  public async getInfo(options?: { signal?: AbortSignal }): Promise<ServerInfoResponse> {
    const endpoint = 'info';
    const response = await this.get<ServerInfoResponse>(endpoint, options);
    this.checkStatus(response, endpoint);
    return response;
  }

  // ==========================================
  // Web E-IMZO Frontend Endpoints
  // ==========================================

  /**
   * Generates a temporary unique random Challenge string for user digital signature (GET /frontend/challenge).
   */
  public async getChallenge(options?: { signal?: AbortSignal }): Promise<ChallengeResponse> {
    const endpoint = 'frontend/challenge';
    const response = await this.get<ChallengeResponse>(endpoint, options);
    this.checkStatus(response, endpoint);
    return response;
  }

  /**
   * Attaches a trusted timestamp token to a PKCS#7 document (POST /frontend/timestamp/pkcs7).
   *
   * @param pkcs7Base64 Base64 encoded PKCS#7 document.
   * @param options Header options (realIp, host, signal).
   */
  public async attachTimestamp(
    pkcs7Base64: string,
    options?: RequestHeaderOptions
  ): Promise<AttachTimestampResponse> {
    this.validateRequired(pkcs7Base64, 'pkcs7Base64');

    const endpoint = 'frontend/timestamp/pkcs7';
    const raw = await this.postForm(endpoint, pkcs7Base64.trim(), options);
    const response = new AttachTimestampResponse(raw);
    this.checkStatus(response, endpoint);
    return response;
  }

  /**
   * Creates a PKCS#7 Attached document by combining the original document and detached PKCS#7 signature (POST /frontend/pkcs7/make-attached).
   *
   * @param documentBase64 Base64 encoded original document.
   * @param pkcs7DetachedBase64 Base64 encoded detached PKCS#7 signature.
   * @param options Header options (realIp, host, signal).
   */
  public async makeAttached(
    documentBase64: string,
    pkcs7DetachedBase64: string,
    options?: RequestHeaderOptions
  ): Promise<MakeAttachedResponse> {
    this.validateRequired(documentBase64, 'documentBase64');
    this.validateRequired(pkcs7DetachedBase64, 'pkcs7DetachedBase64');

    const endpoint = 'frontend/pkcs7/make-attached';
    const body = `${documentBase64.trim()}|${pkcs7DetachedBase64.trim()}`;
    const response = await this.postForm<MakeAttachedResponse>(endpoint, body, options);
    this.checkStatus(response, endpoint);
    return response;
  }

  /**
   * Merges two PKCS#7 Attached documents containing the same document into a single multi-signed container (POST /frontend/pkcs7/join).
   *
   * @param pkcs7AttachedBase64A First Base64 encoded PKCS#7 Attached document.
   * @param pkcs7AttachedBase64B Second Base64 encoded PKCS#7 Attached document.
   * @param options Header options (realIp, host, signal).
   */
  public async joinAttached(
    pkcs7AttachedBase64A: string,
    pkcs7AttachedBase64B: string,
    options?: RequestHeaderOptions
  ): Promise<JoinAttachedResponse> {
    this.validateRequired(pkcs7AttachedBase64A, 'pkcs7AttachedBase64A');
    this.validateRequired(pkcs7AttachedBase64B, 'pkcs7AttachedBase64B');

    const endpoint = 'frontend/pkcs7/join';
    const body = `${pkcs7AttachedBase64A.trim()}|${pkcs7AttachedBase64B.trim()}`;
    const response = await this.postForm<JoinAttachedResponse>(endpoint, body, options);
    this.checkStatus(response, endpoint);
    return response;
  }

  // ==========================================
  // Web E-IMZO Backend Endpoints
  // ==========================================

  /**
   * Authenticates a user by validating the PKCS#7 signed Challenge document (POST /backend/auth).
   *
   * @param pkcs7Base64 Base64 encoded PKCS#7 document containing the signed challenge.
   * @param options Header options (realIp, host, signal).
   */
  public async authenticate(
    pkcs7Base64: string,
    options?: RequestHeaderOptions
  ): Promise<AuthResponse> {
    this.validateRequired(pkcs7Base64, 'pkcs7Base64');

    const endpoint = 'backend/auth';
    const raw = await this.postForm(endpoint, pkcs7Base64.trim(), options);
    const response = new AuthResponse(raw);
    this.checkStatus(response, endpoint);
    return response;
  }

  /**
   * Verifies the digital signature and certificate of a PKCS#7 Attached document (POST /backend/pkcs7/verify/attached).
   *
   * @param pkcs7AttachedBase64 Base64 encoded PKCS#7 Attached document.
   * @param options Header options (realIp, host, signal).
   */
  public async verifyAttached(
    pkcs7AttachedBase64: string,
    options?: RequestHeaderOptions
  ): Promise<AttachedVerifyResponse> {
    this.validateRequired(pkcs7AttachedBase64, 'pkcs7AttachedBase64');

    const endpoint = 'backend/pkcs7/verify/attached';
    const raw = await this.postForm(endpoint, pkcs7AttachedBase64.trim(), options);
    const response = new AttachedVerifyResponse(raw);
    this.checkStatus(response, endpoint);
    return response;
  }

  /**
   * Verifies the digital signature and certificate of a PKCS#7 Detached document with its original document (POST /backend/pkcs7/verify/detached).
   *
   * @param documentBase64 Base64 encoded original document.
   * @param pkcs7DetachedBase64 Base64 encoded detached PKCS#7 signature.
   * @param options Header options (realIp, host, signal).
   */
  public async verifyDetached(
    documentBase64: string,
    pkcs7DetachedBase64: string,
    options?: RequestHeaderOptions
  ): Promise<DetachedVerifyResponse> {
    this.validateRequired(documentBase64, 'documentBase64');
    this.validateRequired(pkcs7DetachedBase64, 'pkcs7DetachedBase64');

    const endpoint = 'backend/pkcs7/verify/detached';
    const body = `${documentBase64.trim()}|${pkcs7DetachedBase64.trim()}`;
    const raw = await this.postForm(endpoint, body, options);
    const response = new DetachedVerifyResponse(raw);
    this.checkStatus(response, endpoint);
    return response;
  }

  // ==========================================
  // Mobile ID-CARD Frontend Endpoints
  // ==========================================

  /**
   * Initializes mobile authentication, returning SiteID, DocumentID, and Challenge (POST /frontend/mobile/auth).
   */
  public async mobileAuth(options?: { signal?: AbortSignal }): Promise<MobileAuthResponse> {
    const endpoint = 'frontend/mobile/auth';
    const raw = await this.postWithoutBody<MobileAuthResponseData>(endpoint, options);
    const response = new MobileAuthResponse(raw);
    this.checkStatus(response, endpoint);
    return response;
  }

  /**
   * Initializes mobile document signing, returning SiteID and DocumentID (POST /frontend/mobile/sign).
   */
  public async mobileSign(options?: { signal?: AbortSignal }): Promise<MobileSignResponse> {
    const endpoint = 'frontend/mobile/sign';
    const response = await this.postWithoutBody<MobileSignResponse>(endpoint, options);
    this.checkStatus(response, endpoint);
    return response;
  }

  /**
   * Polls the status of a mobile signing or authentication operation by DocumentID (POST /frontend/mobile/status).
   * Note: Status == 2 (PendingUpload) will NOT throw an error even when throwOnError is true.
   *
   * @param documentId The DocumentID returned from mobileAuth or mobileSign.
   * @param options Request options.
   */
  public async getMobileStatus(
    documentId: string,
    options?: { signal?: AbortSignal }
  ): Promise<MobileStatusResponse> {
    this.validateRequired(documentId, 'documentId');

    const endpoint = 'frontend/mobile/status';
    const params = new URLSearchParams();
    params.set('documentId', documentId.trim());

    const raw = await this.postParams(endpoint, params, options);
    const response = new MobileStatusResponse(raw);

    // Only throw on negative status codes; status === 2 is normal polling status
    if (this.throwOnError && response.status < 0) {
      throw new EImzoApiException({
        status: response.status,
        message: response.message,
        endpoint,
      });
    }

    return response;
  }

  /**
   * Uploads a PKCS#7 Detached document from mobile client or mock server (POST /frontend/mobile/upload).
   */
  public async uploadMobilePkcs7(
    request: MobileUploadRequest,
    options?: { signal?: AbortSignal }
  ): Promise<EImzoResponse>;
  public async uploadMobilePkcs7(
    documentId: string,
    pkcs7Base64: string,
    serialNumber: string,
    options?: { signal?: AbortSignal }
  ): Promise<EImzoResponse>;
  public async uploadMobilePkcs7(
    arg1: MobileUploadRequest | string,
    arg2?: string | { signal?: AbortSignal },
    arg3?: string,
    arg4?: { signal?: AbortSignal }
  ): Promise<EImzoResponse> {
    let documentId: string;
    let pkcs7Base64: string;
    let serialNumber: string;
    let opts: { signal?: AbortSignal } | undefined;

    if (typeof arg1 === 'object') {
      documentId = arg1.documentId;
      pkcs7Base64 = arg1.pkcs7Base64;
      serialNumber = arg1.serialNumber;
      opts = arg2 as { signal?: AbortSignal } | undefined;
    } else {
      documentId = arg1;
      pkcs7Base64 = arg2 as string;
      serialNumber = arg3 as string;
      opts = arg4;
    }

    this.validateRequired(documentId, 'documentId');
    this.validateRequired(pkcs7Base64, 'pkcs7Base64');
    this.validateRequired(serialNumber, 'serialNumber');

    const endpoint = 'frontend/mobile/upload';
    const params = new URLSearchParams();
    params.set('document_id', documentId.trim());
    params.set('pkcs7_b64', pkcs7Base64.trim());
    params.set('serial_number', serialNumber.trim());

    const response = await this.postParams<EImzoResponse>(endpoint, params, opts);
    this.checkStatus(response, endpoint);
    return response;
  }

  // ==========================================
  // Mobile ID-CARD Backend Endpoints
  // ==========================================

  /**
   * Verifies the mobile authentication result on backend after status polling reports 1 (GET /backend/mobile/authenticate/{DocumentID}).
   *
   * @param documentId Temporary DocumentID.
   * @param options Header options (realIp, host, signal).
   */
  public async mobileAuthenticate(
    documentId: string,
    options?: RequestHeaderOptions
  ): Promise<MobileAuthenticateResponse> {
    this.validateRequired(documentId, 'documentId');

    const endpoint = `backend/mobile/authenticate/${encodeURIComponent(documentId.trim())}`;
    const raw = await this.get<MobileAuthenticateResponseData>(endpoint, options);
    const response = new MobileAuthenticateResponse(raw);
    this.checkStatus(response, endpoint);
    return response;
  }

  /**
   * Verifies the signed document on backend after mobile status polling reports 1 (POST /backend/mobile/verify).
   *
   * @param documentId Temporary DocumentID.
   * @param document Base64 string or Uint8Array/Buffer of document that was signed.
   * @param options Header options (realIp, host, signal).
   */
  public async mobileVerify(
    documentId: string,
    document: string | Uint8Array,
    options?: RequestHeaderOptions
  ): Promise<MobileVerifyResponse> {
    this.validateRequired(documentId, 'documentId');
    if (!document) {
      throw new EImzoValidationError('document', 'document cannot be null or empty.');
    }

    const documentBase64 =
      typeof document === 'string'
        ? document.trim()
        : Buffer.from(document).toString('base64');

    const endpoint = 'backend/mobile/verify';
    const params = new URLSearchParams();
    params.set('documentId', documentId.trim());
    params.set('document', documentBase64);

    const raw = await this.postParams(endpoint, params, options);
    const response = new MobileVerifyResponse(raw);
    this.checkStatus(response, endpoint);
    return response;
  }

  // ==========================================
  // Internal HTTP Helpers
  // ==========================================

  private async get<T>(endpoint: string, options?: RequestHeaderOptions): Promise<T> {
    const url = new URL(endpoint, this.baseUrl).toString();
    const headers = this.buildHeaders(options);

    return this.executeRequest<T>(url, {
      method: 'GET',
      headers,
      signal: this.createTimeoutSignal(options?.signal),
    }, endpoint);
  }

  private async postWithoutBody<T>(endpoint: string, options?: { signal?: AbortSignal }): Promise<T> {
    const url = new URL(endpoint, this.baseUrl).toString();

    return this.executeRequest<T>(url, {
      method: 'POST',
      signal: this.createTimeoutSignal(options?.signal),
    }, endpoint);
  }

  private async postForm<T = any>(
    endpoint: string,
    body: string,
    options?: RequestHeaderOptions
  ): Promise<T> {
    const url = new URL(endpoint, this.baseUrl).toString();
    const headers = this.buildHeaders(options);
    headers.set('Content-Type', 'application/x-www-form-urlencoded');

    return this.executeRequest<T>(url, {
      method: 'POST',
      headers,
      body,
      signal: this.createTimeoutSignal(options?.signal),
    }, endpoint);
  }

  private async postParams<T = any>(
    endpoint: string,
    params: URLSearchParams,
    options?: RequestHeaderOptions
  ): Promise<T> {
    const url = new URL(endpoint, this.baseUrl).toString();
    const headers = this.buildHeaders(options);
    headers.set('Content-Type', 'application/x-www-form-urlencoded');

    return this.executeRequest<T>(url, {
      method: 'POST',
      headers,
      body: params.toString(),
      signal: this.createTimeoutSignal(options?.signal),
    }, endpoint);
  }

  private buildHeaders(options?: RequestHeaderOptions): Headers {
    const headers = new Headers();
    const host = options?.host ?? this.defaultHost;
    if (host && host.trim().length > 0) {
      headers.set('Host', host.trim());
    }

    const realIp = options?.realIp ?? this.defaultRealIp;
    if (realIp && realIp.trim().length > 0) {
      headers.set('X-Real-IP', realIp.trim());
    }

    return headers;
  }

  private createTimeoutSignal(userSignal?: AbortSignal): AbortSignal {
    const timeoutSignal = AbortSignal.timeout(this.timeout);
    if (!userSignal) {
      return timeoutSignal;
    }

    if (typeof AbortSignal.any === 'function') {
      return AbortSignal.any([timeoutSignal, userSignal]);
    }

    const controller = new AbortController();
    const onAbort = () => controller.abort();
    timeoutSignal.addEventListener('abort', onAbort, { once: true });
    userSignal.addEventListener('abort', onAbort, { once: true });
    return controller.signal;
  }

  private async executeRequest<T>(
    url: string,
    init: RequestInit,
    endpoint: string
  ): Promise<T> {
    let res: Response;
    try {
      res = await this.fetchImpl(url, init);
    } catch (err: unknown) {
      throw new EImzoApiException({
        status: -999,
        message: `HTTP communication error calling '${endpoint}': ${(err as Error)?.message ?? String(err)}`,
        endpoint,
        cause: err,
      });
    }

    const text = await res.text();

    if (!res.ok) {
      throw new EImzoApiException({
        status: res.status,
        httpStatusCode: res.status,
        message: `HTTP ${res.status} ${res.statusText}. Response body: ${text}`,
        endpoint,
        rawBody: text,
      });
    }

    if (!text || text.trim().length === 0) {
      return {} as T;
    }

    try {
      return JSON.parse(text) as T;
    } catch (parseErr) {
      throw new EImzoApiException({
        status: -999,
        httpStatusCode: res.status,
        message: `Failed to parse JSON response from '${endpoint}'. Raw body: ${text}`,
        endpoint,
        rawBody: text,
        cause: parseErr,
      });
    }
  }

  private checkStatus(response: EImzoResponse, endpoint: string): void {
    if (this.throwOnError && response && response.status !== 1) {
      throw new EImzoApiException({
        status: response.status,
        message: response.message,
        endpoint,
      });
    }
  }

  private validateRequired(value: string | undefined | null, fieldName: string): void {
    if (!value || value.trim().length === 0) {
      throw new EImzoValidationError(fieldName, `${fieldName} cannot be null, empty, or whitespace.`);
    }
  }
}
