import 'reflect-metadata';
import { describe, it, expect } from 'vitest';
import { Test, TestingModule } from '@nestjs/testing';
import { Injectable } from '@nestjs/common';
import {
  EImzoModule,
  EImzoService,
  InjectEImzoClient,
  E_IMZO_CLIENT,
} from '../src/nestjs/index.js';
import { EImzoClient } from '../src/index.js';

@Injectable()
class ConsumerService {
  constructor(
    public readonly eimzoService: EImzoService,
    @InjectEImzoClient() public readonly eimzoClient: EImzoClient
  ) {}
}

describe('NestJS EImzoModule Integration', () => {
  it('registers module synchronously with register()', async () => {
    const moduleRef: TestingModule = await Test.createTestingModule({
      imports: [
        EImzoModule.register({
          baseUrl: 'http://127.0.0.1:8080',
          defaultHost: 'myportal.uz',
        }),
      ],
      providers: [ConsumerService],
    }).compile();

    const service = moduleRef.get<EImzoService>(EImzoService);
    const client = moduleRef.get<EImzoClient>(E_IMZO_CLIENT);
    const consumer = moduleRef.get<ConsumerService>(ConsumerService);

    expect(service).toBeDefined();
    expect(client).toBeDefined();
    expect(consumer).toBeDefined();
    expect(consumer.eimzoService).toBe(service);
    expect(consumer.eimzoClient).toBe(service);
  });

  it('registers module asynchronously with registerAsync() and useFactory', async () => {
    const moduleRef: TestingModule = await Test.createTestingModule({
      imports: [
        EImzoModule.registerAsync({
          useFactory: async () => ({
            baseUrl: 'http://127.0.0.1:8080',
            defaultRealIp: '1.2.3.4',
          }),
        }),
      ],
      providers: [ConsumerService],
    }).compile();

    const service = moduleRef.get<EImzoService>(EImzoService);
    const consumer = moduleRef.get<ConsumerService>(ConsumerService);

    expect(service).toBeDefined();
    expect(consumer.eimzoService).toBeDefined();
    expect(consumer.eimzoClient).toBeDefined();
  });
});
