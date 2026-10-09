using EImzo.Client;
using EImzo.Client.Enums;
using EImzo.Client.Extensions;
using EImzo.Client.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=================================================");
Console.WriteLine("    E-IMZO .NET Client SDK (Unofficial) - Sample");
Console.WriteLine("=================================================");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // 1. Register E-IMZO client via Dependency Injection
        services.AddEImzoClient(options =>
        {
            options.BaseUrl = new Uri("http://127.0.0.1:8080/");
            options.DefaultHost = "example.uz";
            options.DefaultRealIp = "127.0.0.1";
            options.Timeout = TimeSpan.FromSeconds(15);
            options.ThrowOnError = false; // We will inspect status codes explicitly
        });
    })
    .Build();

var client = host.Services.GetRequiredService<IEImzoClient>();

Console.WriteLine("\n--- 1. Health & Server Info ---");
try
{
    var ping = await client.PingAsync();
    Console.WriteLine($"Ping Status: {ping.Status} ({ping.StatusCode.GetDescription("uz")})");
    Console.WriteLine($"Server DateTime: {ping.ServerDateTime}, IP: {ping.YourIp}");
    if (ping.VpnKeyInfo != null)
    {
        Console.WriteLine($"VPN Key SN: {ping.VpnKeyInfo.SerialNumber}, Valid to: {ping.VpnKeyInfo.ValidTo}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Ping not reachable (E-IMZO server is not running locally): {ex.Message}");
}

Console.WriteLine("\n--- 2. Web Authentication Flow Demonstration ---");
try
{
    var challengeResponse = await client.GetChallengeAsync();
    if (challengeResponse.IsSuccess)
    {
        Console.WriteLine($"Generated Challenge: {challengeResponse.Challenge} (TTL: {challengeResponse.Ttl}s)");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Challenge call error: {ex.Message}");
}

Console.WriteLine("\n--- 3. E-IMZO Mobile ID-CARD Deeplink & QR Demonstration ---");
try
{
    var dummySiteId = "0000";
    var dummyDocId = "B7735734";
    var dummyHash = "AB8C2ED52B12DCBAB3FBD8C11007E4C0C7BF6A2F5818C05DEB61F3EE39052BDC";

    // Build QR Code Payload (Body + 8-char CRC32)
    var qrCodePayload = EImzoMobileHelper.BuildQrCode(dummySiteId, dummyDocId, dummyHash);
    Console.WriteLine($"QR Code Payload: {qrCodePayload}");
    Console.WriteLine($"QR Code CRC32 Valid: {EImzoMobileHelper.VerifyQrCode(qrCodePayload)}");

    // Build Deeplink (eimzo://sign?qc=...)
    var deepLink = EImzoMobileHelper.BuildDeepLink(qrCodePayload);
    Console.WriteLine($"Mobile Deeplink: {deepLink}");
}
catch (Exception ex)
{
    Console.WriteLine($"Mobile demonstration error: {ex.Message}");
}

Console.WriteLine("\n--- 4. Multi-language Status Code Descriptions ---");
var codes = new[]
{
    EImzoStatusCode.Success,
    EImzoStatusCode.VpnOrCertificateCheckFailed,
    EImzoStatusCode.SignatureInvalid,
    EImzoStatusCode.TimestampWindowExceeded,
    EImzoStatusCode.ChallengeNotFoundOrExpired
};

foreach (var code in codes)
{
    Console.WriteLine($"[(int){(int)code,3} {code,-30}]");
    Console.WriteLine($"   UZ: {code.GetDescription("uz")}");
    Console.WriteLine($"   EN: {code.GetDescription("en")}");
    Console.WriteLine($"   RU: {code.GetDescription("ru")}");
}

Console.WriteLine("\nAll demonstrations completed successfully.");
