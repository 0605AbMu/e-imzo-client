# E-IMZO Client SDK (Node.js & TypeScript)

[![NPM Version](https://img.shields.io/npm/v/e-imzo-server-client.svg)](https://www.npmjs.com/package/e-imzo-server-client)
[![Build & Test](https://github.com/0605AbMu/e-imzo-client/actions/workflows/typescript-ci.yml/badge.svg)](https://github.com/0605AbMu/e-imzo-client/actions/workflows/typescript-ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Compatible Server](https://img.shields.io/badge/E--IMZO%20Server-v2.2.x%20%7C%202.x-brightgreen.svg)](https://github.com/0605AbMu/e-imzo-server)

> [!IMPORTANT]
> **Disclaimer**: This is an **unofficial**, community-maintained open-source client SDK. It is neither affiliated with, endorsed by, nor maintained by the State Tax Committee of Uzbekistan (DSQ), SIC "Yangi Texnologiyalar", or official E-IMZO service operators.

High-performance, strongly-typed, modern unofficial TypeScript and Node.js Client SDK for Uzbekistan's **E-IMZO** and **E-IMZO-SERVER** REST API and Mobile ID-CARD system, featuring **first-class NestJS compatibility**.

Documentation reference: [qo0p/e-imzo-doc](https://github.com/qo0p/e-imzo-doc/blob/master/README.md).  
Compatible Server Docker: [`0605AbMu/e-imzo-server`](https://github.com/0605AbMu/e-imzo-server) (`ghcr.io/0605abmu/e-imzo-server:2.2.1`).

---

## 🌟 Features

- ⚡ **Zero runtime dependencies**: Built using standard global `fetch` (Node.js 18+) and native IEEE 802.3 CRC32 calculation.
- 🎯 **Full API Coverage**:
  - **Health & Diagnostics**: `/ping`, `/info`
  - **Web Auth & PKCS#7**: `/frontend/challenge`, `/backend/auth`, `/frontend/timestamp/pkcs7`, `/backend/pkcs7/verify/attached`, `/backend/pkcs7/verify/detached`, `/frontend/pkcs7/make-attached`, `/frontend/pkcs7/join`
  - **Mobile ID-CARD (E-IMZO Mobile)**: `/frontend/mobile/auth`, `/frontend/mobile/sign`, `/frontend/mobile/status`, `/frontend/mobile/upload`, `/backend/mobile/authenticate/{DocumentID}`, `/backend/mobile/verify`
- 🏢 **First-Class NestJS Module**: Full dynamic module support (`EImzoModule.register(...)`, `EImzoModule.registerAsync(...)`), inject via `EImzoService` or `@InjectEImzoClient()`.
- 🇺🇿 **Uzbek OID Helpers**: Auto-parsing of PINFL (JSHSHIR - `1.2.860.3.16.1.2`), Legal Entity TIN (STIR - `1.2.860.3.16.1.1`), Physical Person TIN (`UID`), organization name, and legal entity detection.
- 📱 **Mobile QR & Deeplink Generator**: Built-in IEEE 802.3 CRC32 checksum calculation, QR code generator, and `eimzo://sign?qc=...` deeplink generator and validator.
- 📦 **Dual ESM & CommonJS**: Full compatibility with both `import` and `require()`, complete with TypeScript declarations (`.d.ts` / `.d.mts`) and sourcemaps.

---

## 📦 Installation

```bash
# npm
npm install e-imzo-server-client

# pnpm
pnpm add e-imzo-server-client

# yarn
yarn add e-imzo-server-client
```

---

## 🚀 Quick Start (Node.js / TypeScript)

### 1. Initialize Client

```typescript
import { EImzoClient } from 'e-imzo-server-client';

const eimzo = new EImzoClient({
  baseUrl: 'http://127.0.0.1:8080/', // E-IMZO-SERVER URL
  defaultHost: 'myportal.gov.uz',    // Required domain registered on E-IMZO
  defaultRealIp: '1.2.3.4',          // Client IP (X-Real-IP)
  timeout: 30000,                    // Timeout in ms (default: 30000)
  throwOnError: true,                // Auto throw EImzoApiException on status != 1
});

// Check server status
const ping = await eimzo.ping();
console.log('Server time:', ping.serverDateTime, 'VPN Key:', ping.vpnKeyInfo?.serialNumber);
```

### 2. Web Authentication Flow

```typescript
// Step 1: Frontend requests a random challenge
const { challenge, ttl } = await eimzo.getChallenge();
console.log(`Sign this challenge: ${challenge} (Valid for ${ttl}s)`);

// Step 2: User signs the challenge via browser E-IMZO plugin, returning pkcs7Base64
const pkcs7Base64 = 'MIAGCSqGSIb3DQEHAqCAMIACAQEx...';

// Step 3: Backend verifies the signature and authenticates user
const auth = await eimzo.authenticate(pkcs7Base64);
const cert = auth.subjectCertificateInfo;

console.log('Full Name:', cert?.commonName);
console.log('PINFL / JSHSHIR:', cert?.pinfl);
console.log('TIN / STIR:', cert?.tin);
console.log('Is Legal Entity:', cert?.isLegalEntity);
```

### 3. PKCS#7 Document Verification

```typescript
// Attached verification (document is embedded inside PKCS#7)
const result = await eimzo.verifyAttached(pkcs7AttachedBase64);
if (result.pkcs7Info?.isAllSignersValid) {
  console.log('Document content Base64:', result.pkcs7Info.documentBase64);
  console.log('Primary signer PINFL:', result.pkcs7Info.primarySigner?.certificate?.pinfl);
}

// Detached verification (signature separated from document)
const detachedResult = await eimzo.verifyDetached(documentBase64, pkcs7DetachedBase64);
console.log('Verified:', detachedResult.pkcs7Info?.isAllSignersValid);
```

### 4. Mobile ID-CARD (E-IMZO Mobile)

```typescript
import { EImzoClient, buildDeepLink, buildQrCode } from 'e-imzo-server-client';

// 1. Initialize mobile auth or sign session
const { siteId, documentId, challenge } = await eimzo.mobileAuth();

// 2. Generate QR code & Deeplink for mobile app
const qrCode = buildQrCode(siteId!, documentId!, challenge!);
const deepLink = buildDeepLink(qrCode); // eimzo://sign?qc=...

console.log('Scan QR Code:', qrCode);
console.log('Open App Deeplink:', deepLink);

// 3. Poll status until user approves on smartphone
let status = await eimzo.getMobileStatus(documentId!);
while (status.isPending) {
  await new Promise((resolve) => setTimeout(resolve, 2000));
  status = await eimzo.getMobileStatus(documentId!);
}

if (status.isCompleted) {
  // 4. Verify authentication on backend
  const verified = await eimzo.mobileAuthenticate(documentId!);
  console.log('Authenticated User PINFL:', verified.subjectCertificateInfo?.pinfl);
}
```

---

## 🦅 NestJS Integration

`e-imzo-server-client` provides native subpath export `e-imzo-server-client/nestjs` with first-class dynamic module support.

### 1. Register Module Synchronously

```typescript
// app.module.ts
import { Module } from '@nestjs/common';
import { EImzoModule } from 'e-imzo-server-client/nestjs';

@Module({
  imports: [
    EImzoModule.register({
      isGlobal: true, // makes EImzoService available everywhere
      baseUrl: 'http://127.0.0.1:8080',
      defaultHost: 'myportal.gov.uz',
    }),
  ],
})
export class AppModule {}
```

### 2. Register Module Asynchronously (with `ConfigService`)

```typescript
// app.module.ts
import { Module } from '@nestjs/common';
import { ConfigModule, ConfigService } from '@nestjs/config';
import { EImzoModule } from 'e-imzo-server-client/nestjs';

@Module({
  imports: [
    ConfigModule.forRoot(),
    EImzoModule.registerAsync({
      isGlobal: true,
      imports: [ConfigModule],
      inject: [ConfigService],
      useFactory: (config: ConfigService) => ({
        baseUrl: config.getOrThrow<string>('EIMZO_SERVER_URL'),
        defaultHost: config.get<string>('EIMZO_DEFAULT_HOST'),
        defaultRealIp: config.get<string>('EIMZO_DEFAULT_REAL_IP'),
        timeout: config.get<number>('EIMZO_TIMEOUT', 30000),
      }),
    }),
  ],
})
export class AppModule {}
```

### 3. Inject in Services or Controllers

You can inject `EImzoService` directly, or use the `@InjectEImzoClient()` decorator:

```typescript
// auth.service.ts
import { Injectable } from '@nestjs/common';
import { EImzoService, InjectEImzoClient } from 'e-imzo-server-client/nestjs';
import { EImzoClient } from 'e-imzo-server-client';

@Injectable()
export class AuthService {
  constructor(
    // Option A: Inject EImzoService (inherits all EImzoClient methods)
    private readonly eimzoService: EImzoService,

    // Option B: Inject interface with decorator
    @InjectEImzoClient() private readonly client: EImzoClient,
  ) {}

  async authenticateUser(pkcs7Base64: string) {
    const auth = await this.eimzoService.authenticate(pkcs7Base64);
    const cert = auth.subjectCertificateInfo;

    return {
      pinfl: cert?.pinfl,
      stir: cert?.tin,
      fullName: cert?.commonName,
      isLegalEntity: cert?.isLegalEntity,
    };
  }
}
```

---

## 🇺🇿 Uzbek Certificate OID Reference

`SubjectCertificateInfo` automatically parses standard Uzbek X.509 OIDs:

| Property | OID / Key | Description | Example |
| :--- | :--- | :--- | :--- |
| `pinfl` | `1.2.860.3.16.1.2` | 14-digit JSHSHIR / PINFL | `30101900000001` |
| `legalEntityTin` | `1.2.860.3.16.1.1` | 9-digit Legal Entity STIR / INN | `123456789` |
| `physicalPersonTin` | `UID` | 9-digit Physical Person STIR / INN | `400000000` |
| `tin` | Fallback | Legal entity STIR if present, else physical person TIN | `123456789` |
| `commonName` | `CN` | Full Name of Certificate Holder | `ALIEV VALI KARIMOVICH` |
| `organization` | `O` | Company / Enterprise Name | `OOO "ENTERPRISE"` |
| `isLegalEntity` | Computed | `true` if `legalEntityTin` is present | `true` / `false` |
| `isPhysicalPerson` | Computed | `true` if individual certificate | `true` / `false` |

---

## ⚠️ Error Handling & Status Codes

When `throwOnError: true` (default), any response where `status !== 1` throws `EImzoApiException`.

```typescript
import { EImzoClient, EImzoApiException, EImzoStatusCode } from 'e-imzo-server-client';

try {
  await eimzo.authenticate(pkcs7);
} catch (error) {
  if (error instanceof EImzoApiException) {
    console.error('Error status code:', error.status);
    console.error('API Endpoint:', error.endpoint);
    console.error('Message:', error.message);

    if (error.status === EImzoStatusCode.ChallengeNotFoundOrExpired) {
      // Challenge has expired
    } else if (error.status === EImzoStatusCode.SignatureInvalid) {
      // Signature is invalid
    }
  }
}
```

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).
