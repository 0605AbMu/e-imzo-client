# E-IMZO Client SDK (Monorepo)

[![Build & Test](https://github.com/qo0p/e-imzo-client/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/qo0p/e-imzo-client/actions/workflows/dotnet-ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![NuGet Version](https://img.shields.io/nuget/v/EImzo.Client.svg)](https://www.nuget.org/packages/EImzo.Client)

Multi-language API Client SDK Monorepo for Uzbekistan's **E-IMZO** and **E-IMZO-SERVER** (State Electronic Digital Signature & Mobile ID-CARD system).

Documentation reference: [qo0p/e-imzo-doc](https://github.com/qo0p/e-imzo-doc/blob/master/README.md).

---

## 📁 Repository Structure

This monorepo is architected to support enterprise-grade SDKs across multiple languages:

```
e-imzo-client/
├── dotnet/                # .NET SDK (C#) - Target frameworks: net8.0, net9.0, netstandard2.0, netstandard2.1
│   ├── src/
│   │   └── EImzo.Client/ # Core SDK package (NuGet ready)
│   ├── tests/
│   │   └── EImzo.Client.Tests/
│   └── samples/
│       └── EImzo.Client.Sample/
├── .github/
│   └── workflows/         # Automated CI/CD & NuGet publishing pipelines
└── README.md
```

Planned language SDKs in future iterations:
- [x] **.NET (C#)** (`dotnet/`)
- [ ] **Go** (`golang/`)
- [ ] **TypeScript / Node.js** (`typescript/`)
- [ ] **Python** (`python/`)

---

## 🚀 .NET SDK (`EImzo.Client`)

High-performance, strongly-typed, modern .NET Client SDK targeting `.NET 8`, `.NET 9`, `.NET Standard 2.0`, and `.NET Standard 2.1`.

### Key Features
- **Comprehensive API Coverage**:
  - **Health & Info**: `/ping`, `/info`
  - **Web Auth & PKCS#7**: `/frontend/challenge`, `/backend/auth`, `/frontend/timestamp/pkcs7`, `/backend/pkcs7/verify/attached`, `/backend/pkcs7/verify/detached`, `/frontend/pkcs7/make-attached`, `/frontend/pkcs7/join`
  - **Mobile ID-Card (E-IMZO Mobile)**: `/frontend/mobile/auth`, `/frontend/mobile/sign`, `/frontend/mobile/status`, `/frontend/mobile/upload`, `/backend/mobile/authenticate/{DocumentID}`, `/backend/mobile/verify`
- **Native Dependency Injection**: Seamless setup with `services.AddEImzoClient(...)` and `IHttpClientFactory`.
- **Modern .NET**: Nullable reference types enabled, cancellation token support, high-performance `System.Text.Json` serialization.
- **Rich Domain Helpers**: Auto-mapping of Uzbek OIDs:
  - PINFL (JSHSHIR - `1.2.860.3.16.1.2`)
  - Legal Entity TIN (STIR - `1.2.860.3.16.1.1`)
  - Physical Person TIN (`UID`)
- **Mobile QR & Deeplink Generator**: Built-in CRC32 calculation and standard `eimzo://sign?qc=...` generation.
- **Enterprise Ready**: Full XML documentation, unit tests, and NuGet packaging metadata.

For detailed usage and code examples, see [dotnet/README.md](dotnet/README.md) or [dotnet/src/EImzo.Client/README.md](dotnet/src/EImzo.Client/README.md).

---

## 🛠 Contributing & Development

### Prerequisites
- [.NET SDK 8.0 or 10.0+](https://dotnet.microsoft.com/download)

### Building and Testing .NET
```bash
cd dotnet
dotnet restore
dotnet build -c Release
dotnet test -c Release --verbosity normal
```

---

## 📄 License
This project is licensed under the [MIT License](LICENSE).
