/**
 * Configuration options for E-IMZO client SDK.
 */
export interface EImzoClientOptions {
  /**
   * Base URL of the E-IMZO-SERVER (e.g. "http://127.0.0.1:8080/").
   */
  baseUrl: string;

  /**
   * Request timeout in milliseconds (default: 30000ms).
   */
  timeout?: number;

  /**
   * Default site domain sent in 'Host' header (e.g. "myportal.gov.uz").
   */
  defaultHost?: string;

  /**
   * Default client IP sent in 'X-Real-IP' header (e.g. "1.2.3.4").
   */
  defaultRealIp?: string;

  /**
   * Whether to automatically throw EImzoApiException when status != 1 (default: true).
   * For mobile status polling, status == 2 (PendingUpload) will NOT throw even when throwOnError is true.
   */
  throwOnError?: boolean;

  /**
   * Optional custom fetch implementation (defaults to globalThis.fetch).
   */
  fetch?: typeof fetch;
}

/**
 * Options for individual API requests that support Host/IP overrides.
 */
export interface RequestHeaderOptions {
  /**
   * Client IP address sent in 'X-Real-IP' header. Falls back to defaultRealIp if omitted.
   */
  realIp?: string;

  /**
   * Domain name sent in 'Host' header. Falls back to defaultHost if omitted.
   */
  host?: string;

  /**
   * Optional AbortSignal for cancelling this specific request.
   */
  signal?: AbortSignal;
}
