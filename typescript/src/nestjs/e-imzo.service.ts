import { Inject, Injectable } from '@nestjs/common';
import { EImzoClient } from '../client/e-imzo-client.js';
import type { EImzoClientOptions } from '../client/client-options.js';
import { E_IMZO_MODULE_OPTIONS } from './constants.js';

/**
 * Injectable NestJS service wrapping the EImzoClient.
 * Inherits all API methods directly from EImzoClient.
 */
@Injectable()
export class EImzoService extends EImzoClient {
  constructor(@Inject(E_IMZO_MODULE_OPTIONS) options: EImzoClientOptions) {
    super(options);
  }
}
