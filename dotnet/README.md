# E-IMZO .NET Client SDK (Unofficial)

> **Disclaimer**: This is an **unofficial** community client library. It is not affiliated with or endorsed by official E-IMZO service operators.

This folder contains the complete .NET solution and projects for the unofficial E-IMZO Client SDK:

- **`src/EImzo.Client`**: The primary, production-ready NuGet package targeting `.NET 8`, `.NET 9`, `.NET Standard 2.0`, and `.NET Standard 2.1`.
- **`tests/EImzo.Client.Tests`**: Unit and integration test suite with 100% test coverage for all endpoints, models, OID extractions, and mobile utilities.
- **`samples/EImzo.Client.Sample`**: An interactive sample application demonstrating Dependency Injection, health checks, authentication, and mobile QR/Deeplink generation.

### 🔄 Server Compatibility
This .NET SDK is versioned using Semantic Versioning (`1.x.x`) and is fully tested with **[E-IMZO Server v2.2.1](https://github.com/0605AbMu/e-imzo-server)** (and all `2.x` REST API services).

For complete documentation, installation, and code samples, see [src/EImzo.Client/README.md](src/EImzo.Client/README.md).
