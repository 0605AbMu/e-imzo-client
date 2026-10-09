import { Inject } from '@nestjs/common';
import { E_IMZO_CLIENT, E_IMZO_MODULE_OPTIONS, E_IMZO_SERVICE } from './constants.js';

/**
 * Decorator to inject the EImzoClient instance.
 */
export const InjectEImzoClient = () => Inject(E_IMZO_CLIENT);

/**
 * Decorator to inject the EImzoService instance.
 */
export const InjectEImzoService = () => Inject(E_IMZO_SERVICE);

/**
 * Decorator to inject the E-IMZO options.
 */
export const InjectEImzoOptions = () => Inject(E_IMZO_MODULE_OPTIONS);
