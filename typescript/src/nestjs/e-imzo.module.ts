import { DynamicModule, Module, Provider } from '@nestjs/common';
import { E_IMZO_CLIENT, E_IMZO_MODULE_OPTIONS, E_IMZO_SERVICE } from './constants.js';
import { EImzoService } from './e-imzo.service.js';
import type {
  EImzoModuleAsyncOptions,
  EImzoModuleOptions,
  EImzoOptionsFactory,
} from './interfaces.js';

@Module({})
export class EImzoModule {
  /**
   * Registers EImzoModule synchronously with static options.
   */
  public static register(options: EImzoModuleOptions & { isGlobal?: boolean }): DynamicModule {
    const { isGlobal = false, ...clientOptions } = options;

    const optionsProvider: Provider = {
      provide: E_IMZO_MODULE_OPTIONS,
      useValue: clientOptions,
    };

    const clientProvider: Provider = {
      provide: E_IMZO_CLIENT,
      useExisting: EImzoService,
    };

    const serviceAliasProvider: Provider = {
      provide: E_IMZO_SERVICE,
      useExisting: EImzoService,
    };

    return {
      module: EImzoModule,
      global: isGlobal,
      providers: [optionsProvider, EImzoService, clientProvider, serviceAliasProvider],
      exports: [EImzoService, E_IMZO_CLIENT, E_IMZO_SERVICE],
    };
  }

  /**
   * Registers EImzoModule asynchronously (e.g. reading from ConfigService).
   */
  public static registerAsync(options: EImzoModuleAsyncOptions): DynamicModule {
    const clientProvider: Provider = {
      provide: E_IMZO_CLIENT,
      useExisting: EImzoService,
    };

    const serviceAliasProvider: Provider = {
      provide: E_IMZO_SERVICE,
      useExisting: EImzoService,
    };

    return {
      module: EImzoModule,
      global: options.isGlobal ?? false,
      imports: options.imports ?? [],
      providers: [
        ...this.createAsyncProviders(options),
        EImzoService,
        clientProvider,
        serviceAliasProvider,
      ],
      exports: [EImzoService, E_IMZO_CLIENT, E_IMZO_SERVICE],
    };
  }

  private static createAsyncProviders(options: EImzoModuleAsyncOptions): Provider[] {
    if (options.useExisting || options.useFactory) {
      return [this.createAsyncOptionsProvider(options)];
    }

    if (options.useClass) {
      return [
        this.createAsyncOptionsProvider(options),
        {
          provide: options.useClass,
          useClass: options.useClass,
        },
      ];
    }

    throw new Error(
      'Invalid EImzoModule async configuration: must provide useFactory, useClass, or useExisting.'
    );
  }

  private static createAsyncOptionsProvider(options: EImzoModuleAsyncOptions): Provider {
    if (options.useFactory) {
      return {
        provide: E_IMZO_MODULE_OPTIONS,
        useFactory: options.useFactory,
        inject: options.inject ?? [],
      };
    }

    const injectType = options.useExisting ?? options.useClass;
    if (!injectType) {
      throw new Error('Invalid EImzoModule configuration: missing provider type.');
    }

    return {
      provide: E_IMZO_MODULE_OPTIONS,
      useFactory: async (optionsFactory: EImzoOptionsFactory) =>
        optionsFactory.createEImzoOptions(),
      inject: [injectType],
    };
  }
}
