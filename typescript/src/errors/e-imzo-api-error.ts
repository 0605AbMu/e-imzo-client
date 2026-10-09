import { getEImzoStatusMessage } from '../enums/status-code.js';
import { getEImzoMobileStatusMessage } from '../enums/mobile-status-code.js';

export interface EImzoApiExceptionOptions {
  status: number;
  message?: string;
  endpoint?: string;
  httpStatusCode?: number;
  rawBody?: string;
  cause?: unknown;
}

/**
 * Exception thrown when an E-IMZO-SERVER endpoint returns a non-success status code (status != 1)
 * or when an HTTP communication/deserialization error occurs.
 */
export class EImzoApiException extends Error {
  public readonly status: number;
  public readonly endpoint?: string;
  public readonly httpStatusCode?: number;
  public readonly rawBody?: string;

  constructor(options: EImzoApiExceptionOptions) {
    const isMobile = options.endpoint ? options.endpoint.includes('/mobile/') : false;
    const defaultMsg = isMobile
      ? getEImzoMobileStatusMessage(options.status)
      : getEImzoStatusMessage(options.status);

    const message = options.message
      ? `${options.message} (Status: ${options.status})`
      : `${defaultMsg} (Status: ${options.status}${options.endpoint ? `, Endpoint: ${options.endpoint}` : ''})`;

    super(message);
    this.name = 'EImzoApiException';
    this.status = options.status;
    this.endpoint = options.endpoint;
    this.httpStatusCode = options.httpStatusCode;
    this.rawBody = options.rawBody;

    if (options.cause) {
      this.cause = options.cause;
    }

    Object.setPrototypeOf(this, EImzoApiException.prototype);
  }
}
