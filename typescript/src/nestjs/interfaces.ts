import type { ModuleMetadata, Type } from '@nestjs/common';
import type { EImzoClientOptions } from '../client/client-options.js';

export type EImzoModuleOptions = EImzoClientOptions;

export interface EImzoOptionsFactory {
  createEImzoOptions(): Promise<EImzoModuleOptions> | EImzoModuleOptions;
}

export interface EImzoModuleAsyncOptions extends Pick<ModuleMetadata, 'imports'> {
  isGlobal?: boolean;
  useExisting?: Type<EImzoOptionsFactory>;
  useClass?: Type<EImzoOptionsFactory>;
  useFactory?: (...args: any[]) => Promise<EImzoModuleOptions> | EImzoModuleOptions;
  inject?: any[];
}
