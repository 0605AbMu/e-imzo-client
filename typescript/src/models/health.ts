import type { EImzoResponse, TrustedCertificateInfo, VpnKeyInfo } from './common.js';

/**
 * Response returned by GET /ping endpoint.
 */
export interface PingResponse extends EImzoResponse {
  serverDateTime?: string;
  yourIP?: string;
  vpnKeyInfo?: VpnKeyInfo;
}

/**
 * Response returned by GET /info endpoint.
 */
export interface ServerInfoResponse extends EImzoResponse {
  version?: string;
  serverTime?: string;
  vpnKeyInfo?: VpnKeyInfo;
  trustedCertificates?: TrustedCertificateInfo[];
}
