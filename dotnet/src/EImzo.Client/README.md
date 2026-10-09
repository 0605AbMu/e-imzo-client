# EImzo.Client - E-IMZO .NET Client SDK

[![NuGet Version](https://img.shields.io/nuget/v/EImzo.Client.svg)](https://www.nuget.org/packages/EImzo.Client)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![.NET Targets](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%20Standard%202.0%20%7C%20Standard%202.1-purple.svg)](https://dotnet.microsoft.com/)

Official-grade, high-performance .NET Client SDK for Uzbekistan's **E-IMZO** and **E-IMZO-SERVER** REST-API and Mobile ID-CARD system.

Reference documentation: [qo0p/e-imzo-doc](https://github.com/qo0p/e-imzo-doc/blob/master/README.md).

---

## 📦 Installation

Install via NuGet Package Manager:
```bash
dotnet add package EImzo.Client
```

Or via the Visual Studio Package Manager:
```powershell
Install-Package EImzo.Client
```

---

## ⚡ Key Highlights
- **Multi-Target Support**: `.NET 8.0`, `.NET 9.0`, `.NET Standard 2.0`, and `.NET Standard 2.1` (cross-platform, compatible with ASP.NET Core, .NET Core, .NET Framework 4.6.1+, Blazor, and MAUI).
- **Native Dependency Injection**: Seamlessly integrates with `Microsoft.Extensions.DependencyInjection` and `IHttpClientFactory`.
- **Complete Endpoint Coverage**:
  - **Health & Info**: `/ping`, `/info`
  - **Web Auth & Verification**: `/frontend/challenge`, `/backend/auth`, `/backend/pkcs7/verify/attached`, `/backend/pkcs7/verify/detached`, `/frontend/timestamp/pkcs7`, `/frontend/pkcs7/make-attached`, `/frontend/pkcs7/join`
  - **E-IMZO Mobile ID-CARD**: `/frontend/mobile/auth`, `/frontend/mobile/sign`, `/frontend/mobile/status`, `/frontend/mobile/upload`, `/backend/mobile/authenticate/{DocumentID}`, `/backend/mobile/verify`
- **Smart Subject Certificate Mapping**:
  - `Pinfl` (JSHSHIR - 14 digits) from OID `1.2.860.3.16.1.2`
  - `LegalEntityTin` (STIR - 9 digits) from OID `1.2.860.3.16.1.1`
  - `PhysicalPersonTin` from `UID`
  - `CommonName` (CN), `FirstName`, `Surname`, `Organization`, `IsLegalEntity`, `IsPhysicalPerson`
- **Mobile QR & Deeplink Generator**: Built-in IEEE 802.3 CRC32 calculation and standard `eimzo://sign?qc=...` generation.
- **Multi-Language Error Descriptions**: Human-readable descriptions for all E-IMZO status codes in Uzbek (`uz`), Russian (`ru`), and English (`en`).

---

## 🚀 Quick Start

### 1. Registering with Dependency Injection (ASP.NET Core / Worker Service)

In `Program.cs`:

```csharp
using EImzo.Client;

var builder = WebApplication.CreateBuilder(args);

// Register E-IMZO Client
builder.Services.AddEImzoClient(options =>
{
    options.BaseUrl = new Uri(builder.Configuration["EImzo:BaseUrl"] ?? "http://127.0.0.1:8080/");
    options.DefaultHost = "myportal.uz";     // Sent in HTTP 'Host' header
    options.DefaultRealIp = "127.0.0.1";    // Sent in HTTP 'X-Real-IP' header
    options.Timeout = TimeSpan.FromSeconds(30);
    options.ThrowOnError = false;           // If true, auto-throws EImzoApiException when Status != 1
});
```

Inject `IEImzoClient` into your services or controllers:

```csharp
public class AuthService
{
    private readonly IEImzoClient _eimzoClient;

    public AuthService(IEImzoClient eimzoClient)
    {
        _eimzoClient = eimzoClient;
    }
}
```

### 2. Standalone Client (Without Dependency Injection)

```csharp
using EImzo.Client;

var options = new EImzoClientOptions
{
    BaseUrl = new Uri("http://127.0.0.1:8080/"),
    DefaultHost = "myportal.uz"
};

using var httpClient = new HttpClient();
IEImzoClient client = new EImzoClient(httpClient, options);
```

---

## 📖 Usage Examples

### A. Web Authentication Flow (Challenge & Auth)

#### Step 1: Generate Challenge for Frontend
```csharp
var challengeResponse = await client.GetChallengeAsync();

if (challengeResponse.IsSuccess)
{
    string challenge = challengeResponse.Challenge!; // Send to frontend for E-IMZO sign
    int ttlSeconds = challengeResponse.Ttl;          // e.g. 120s
}
```

#### Step 2: Authenticate User on Backend
Frontend returns PKCS#7 signature in Base64:
```csharp
var authResponse = await client.AuthenticateAsync(pkcs7Base64, realIp: userIp, host: "myportal.uz");

if (authResponse.IsSuccess)
{
    var cert = authResponse.SubjectCertificateInfo!;
    
    string pinfl = cert.Pinfl!;          // 14-digit JSHSHIR
    string tin = cert.Tin!;              // STIR (Company or Personal)
    string fullName = cert.CommonName!;  // Full name
    bool isCompany = cert.IsLegalEntity; // True if legal entity
    
    // Login user into your application session...
}
else
{
    // Inspect localized error explanation
    string errorUz = authResponse.StatusCode.GetDescription("uz");
    string errorEn = authResponse.StatusCode.GetDescription("en");
    Console.WriteLine($"Auth failed: {errorUz} (Code: {authResponse.Status})");
}
```

---

### B. PKCS#7 Document Verification

#### Attached Verification (Includes document + timestamp)
```csharp
var verifyResponse = await client.VerifyAttachedAsync(pkcs7AttachedBase64, realIp: userIp);

if (verifyResponse.IsSuccess)
{
    string signedDocBase64 = verifyResponse.Pkcs7Info!.DocumentBase64!;
    bool allValid = verifyResponse.Pkcs7Info.IsAllSignersValid;
    
    foreach (var signer in verifyResponse.Pkcs7Info.Signers!)
    {
        Console.WriteLine($"Signed by: {signer.UserCertificate?.SubjectName}");
        Console.WriteLine($"Signing Time: {signer.SigningTime}");
        Console.WriteLine($"Signature Valid: {signer.Verified}");
        Console.WriteLine($"Cert Valid: {signer.CertificateVerified}");
    }
}
```

#### Detached Verification
```csharp
var verifyResponse = await client.VerifyDetachedAsync(documentBase64, pkcs7DetachedBase64);
```

#### Attaching Timestamp Token
```csharp
var timestampResponse = await client.AttachTimestampAsync(pkcs7Base64);
string pkcs7WithTimestamp = timestampResponse.Pkcs7Base64!;
```

#### Multi-Signature Joining (Section 2.2.8)
```csharp
// Combine two independently signed Attached PKCS#7 containers
var joinResponse = await client.JoinAttachedAsync(containerA, containerB);
string mergedContainer = joinResponse.Pkcs7Base64!;
```

---

### C. E-IMZO Mobile ID-CARD Flow

#### 1. Initiate Mobile Authentication
```csharp
var mobileAuth = await client.MobileAuthAsync();

string siteId = mobileAuth.SiteId!;
string documentId = mobileAuth.DocumentId!;
string challenge = mobileAuth.Challenge!;

// Compute digest of challenge (e.g. O'zDSt 1106 / GOST hash in hex)
string hexHash = ComputeGostHashHex(challenge);

// Generate QR Code payload (siteId + documentId + hexHash + CRC32)
string qrPayload = EImzoMobileHelper.BuildQrCode(siteId, documentId, hexHash);

// Generate Deeplink for mobile app (eimzo://sign?qc=...)
string deepLink = EImzoMobileHelper.BuildDeepLink(qrPayload);
```

#### 2. Poll Status
```csharp
var statusResponse = await client.GetMobileStatusAsync(documentId);

if (statusResponse.IsCompleted)
{
    // Status == 1: Mobile user finished signing and uploaded PKCS#7
}
else if (statusResponse.IsPending)
{
    // Status == 2: Waiting for user action in mobile app
}
```

#### 3. Backend Verification
```csharp
// For mobile auth:
var authResult = await client.MobileAuthenticateAsync(documentId);
string pinfl = authResult.SubjectCertificateInfo?.Pinfl;

// For mobile document sign:
var signResult = await client.MobileVerifyAsync(documentId, documentBytes);
```

---

## 🛡 Status Code Reference

| Code | Enum | Description (UZ) | Description (RU) |
|---|---|---|---|
| `1` | `Success` | Muvaffaqiyatli | Успешно |
| `-1` | `VpnOrCertificateCheckFailed` | Sertifikat holatini tekshirib bo'lmadi (VPN uzilgan) | Не удалось проверить статус сертификата (VPN) |
| `-5` | `TimestampWindowExceeded` | Imzolash vaqti ruxsat etilgan oraliqdan tashqarida | Время подписи вне допустимого окна |
| `-10` | `SignatureInvalid` | ERI (PKCS#7) imzosi haqiqiy emas | ЭЦП недействительна |
| `-11` | `CertificateInvalid` | Sertifikat haqiqiy emas | Сертификат недействителен |
| `-12` | `CertificateInvalidAtSigningTime` | Sertifikat imzolash paytida haqiqiy bo'lmagan | Сертификат недействителен на дату подписи |
| `-20` | `ChallengeNotFoundOrExpired` | Challenge topilmadi yoki muddati tugagan | Не найден challenge или срок его истек |
| `-24` | `CertificatePolicyDisallowed` | Sertifikat siyosati ruxsat etilmagan | Политика сертификата не входит в разрешённый набор |
| `-25` | `CaCertificateDisallowed` | CA sertifikati ruxsat etilmagan | Сертификат УЦ не разрешён для проверки |

---

## 📄 License
This SDK is distributed under the [MIT License](LICENSE).
