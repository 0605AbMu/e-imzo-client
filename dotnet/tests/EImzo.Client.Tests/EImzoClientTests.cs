using System.Net;
using EImzo.Client.Enums;
using EImzo.Client.Exceptions;
using EImzo.Client.Extensions;
using EImzo.Client.Models.Mobile;
using Xunit;

namespace EImzo.Client.Tests;

public class EImzoClientTests
{
    private static EImzoClient CreateClient(MockHttpMessageHandler handler, Action<EImzoClientOptions>? configure = null)
    {
        var options = new EImzoClientOptions
        {
            BaseUrl = new Uri("http://127.0.0.1:8080/"),
            DefaultHost = "example.uz",
            DefaultRealIp = "1.2.3.4"
        };
        configure?.Invoke(options);

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = options.BaseUrl
        };
        return new EImzoClient(httpClient, options);
    }

    [Fact]
    public async Task PingAsync_ShouldParseResponseCorrectly()
    {
        const string json = """
        {
          "serverDateTime": "2022-10-06 16:47:29",
          "yourIP": "127.0.0.1",
          "vpnKeyInfo": {
            "serialNumber": "3",
            "X500Name": "CN=Client",
            "validFrom": "2022-09-24 12:17:24",
            "validTo": "2022-10-24 12:17:24"
          }
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler);

        var response = await client.PingAsync();

        Assert.NotNull(response);
        Assert.Equal("127.0.0.1", response.YourIp);
        Assert.NotNull(response.ServerDateTime);
        Assert.NotNull(response.VpnKeyInfo);
        Assert.Equal("3", response.VpnKeyInfo.SerialNumber);
        Assert.Equal(HttpMethod.Get, handler.LastRequest!.Method);
        Assert.Equal("ping", handler.LastRequest.RequestUri!.AbsolutePath.TrimStart('/'));
    }

    [Fact]
    public async Task GetInfoAsync_ShouldParseResponseCorrectly()
    {
        const string json = """
        {
          "version": "1.11.4",
          "serverTime": "2025.10.21 11:13:57",
          "vpnKeyInfo": {
            "serialNumber": "cd53f2d492dbb643",
            "validFrom": "2025.08.19 13:53:00",
            "validTo": "2027.09.30 13:53:00"
          },
          "trustedCertificates": [
            {
              "serialNumber": "c48c6d327cb85a03",
              "validFrom": "2025.08.19 11:51:46",
              "validTo": "2030.08.19 11:51:46"
            }
          ]
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler);

        var response = await client.GetInfoAsync();

        Assert.NotNull(response);
        Assert.Equal("1.11.4", response.Version);
        Assert.NotNull(response.VpnKeyInfo);
        Assert.Single(response.TrustedCertificates!);
        Assert.Equal(HttpMethod.Get, handler.LastRequest!.Method);
    }

    [Fact]
    public async Task GetChallengeAsync_ShouldReturnChallengeAndTtl()
    {
        const string json = """
        {
          "challenge": "9b573e40-cefd-42cc-a534-f6e78b27c2ae",
          "ttl": 120,
          "status": 1,
          "message": ""
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler);

        var response = await client.GetChallengeAsync();

        Assert.True(response.IsSuccess);
        Assert.Equal("9b573e40-cefd-42cc-a534-f6e78b27c2ae", response.Challenge);
        Assert.Equal(120, response.Ttl);
        Assert.Equal(HttpMethod.Get, handler.LastRequest!.Method);
        Assert.Equal("frontend/challenge", handler.LastRequest.RequestUri!.AbsolutePath.TrimStart('/'));
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldSendHeadersAndBase64Body()
    {
        const string json = """
        {
          "subjectCertificateInfo": {
            "serialNumber": "218711a92",
            "subjectName": {
              "1.2.860.3.16.1.2": "30000000000000",
              "UID": "400000000",
              "CN": "IVANOV IVAN"
            },
            "validFrom": "2022-09-24 17:29:21",
            "validTo": "2022-10-24 17:29:21"
          },
          "status": 1,
          "message": ""
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler);

        var response = await client.AuthenticateAsync("MIAGCSqGSIb3DQEHA...", realIp: "9.9.9.9", host: "custom.domain.uz");

        Assert.True(response.IsSuccess);
        Assert.NotNull(response.SubjectCertificateInfo);
        Assert.Equal("30000000000000", response.SubjectCertificateInfo.Pinfl);
        Assert.Equal("IVANOV IVAN", response.SubjectCertificateInfo.CommonName);

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("backend/auth", handler.LastRequest.RequestUri!.AbsolutePath.TrimStart('/'));
        Assert.Equal("MIAGCSqGSIb3DQEHA...", handler.LastRequestBody);
        Assert.Equal("custom.domain.uz", handler.LastRequest.Headers.Host);
        Assert.Equal("9.9.9.9", handler.LastRequest.Headers.GetValues("X-Real-IP").First());
    }

    [Fact]
    public async Task AttachTimestampAsync_ShouldReturnAttachedTimestamp()
    {
        const string json = """
        {
          "pkcs7b64": "MIAGCSqG...bAAAAAAAA",
          "timestampedSignerList": [
            {
              "serialNumber": "218711a92"
            }
          ],
          "status": 1,
          "message": ""
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler);

        var response = await client.AttachTimestampAsync("MIAG_IN");

        Assert.True(response.IsSuccess);
        Assert.Equal("MIAGCSqG...bAAAAAAAA", response.Pkcs7Base64);
        Assert.Single(response.TimestampedSignerList!);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("frontend/timestamp/pkcs7", handler.LastRequest.RequestUri!.AbsolutePath.TrimStart('/'));
    }

    [Fact]
    public async Task VerifyAttachedAsync_ShouldReturnPkcs7Info()
    {
        const string json = """
        {
          "pkcs7Info": {
            "documentBase64": "c29tZSBkb2N1bWVudA==",
            "signers": [
              {
                "signingTime": "2022-09-27 11:17:53",
                "verified": true,
                "certificateVerified": true,
                "certificateValidAtSigningTime": true
              }
            ]
          },
          "status": 1,
          "message": ""
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler);

        var response = await client.VerifyAttachedAsync("MIAG_ATTACHED");

        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Pkcs7Info);
        Assert.Equal("c29tZSBkb2N1bWVudA==", response.Pkcs7Info.DocumentBase64);
        Assert.True(response.Pkcs7Info.IsAllSignersValid);
        Assert.True(response.Pkcs7Info.Signers![0].Verified);
    }

    [Fact]
    public async Task VerifyDetachedAsync_ShouldSendPipedContent()
    {
        const string json = """
        {
          "pkcs7Info": {
            "signers": [
              {
                "verified": true,
                "certificateVerified": true
              }
            ]
          },
          "status": 1,
          "message": ""
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler);

        var response = await client.VerifyDetachedAsync("DOC_B64", "PKCS7_B64");

        Assert.True(response.IsSuccess);
        Assert.Equal("DOC_B64|PKCS7_B64", handler.LastRequestBody);
        Assert.Equal("backend/pkcs7/verify/detached", handler.LastRequest!.RequestUri!.AbsolutePath.TrimStart('/'));
    }

    [Fact]
    public async Task MakeAttachedAsync_ShouldSendPipedContentAndReturnAttached()
    {
        const string json = """
        {
          "pkcs7b64": "RESULT_ATTACHED",
          "status": 1,
          "message": ""
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler);

        var response = await client.MakeAttachedAsync("DOC_B64", "PKCS7_B64");

        Assert.True(response.IsSuccess);
        Assert.Equal("RESULT_ATTACHED", response.Pkcs7Base64);
        Assert.Equal("DOC_B64|PKCS7_B64", handler.LastRequestBody);
    }

    [Fact]
    public async Task JoinAttachedAsync_ShouldSendPipedContainers()
    {
        const string json = """
        {
          "pkcs7b64": "MERGED_CONTAINER",
          "status": 1,
          "message": ""
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler);

        var response = await client.JoinAttachedAsync("CONTAINER_1", "CONTAINER_2");

        Assert.True(response.IsSuccess);
        Assert.Equal("MERGED_CONTAINER", response.Pkcs7Base64);
        Assert.Equal("CONTAINER_1|CONTAINER_2", handler.LastRequestBody);
    }

    [Fact]
    public async Task MobileAuthAsync_ShouldParseChallangeCorrectly()
    {
        const string json = """
        {
          "status": 1,
          "siteId": "0000",
          "documentId": "2944F1F2",
          "challange": "F8D2181DC6C02EA819B88FF3EF49BE0C"
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler);

        var response = await client.MobileAuthAsync();

        Assert.True(response.IsSuccess);
        Assert.Equal("0000", response.SiteId);
        Assert.Equal("2944F1F2", response.DocumentId);
        Assert.Equal("F8D2181DC6C02EA819B88FF3EF49BE0C", response.Challenge);
    }

    [Fact]
    public async Task MobileSignAsync_ShouldReturnSiteIdAndDocumentId()
    {
        const string json = """
        {
          "status": 1,
          "siteId": "0000",
          "documentId": "850FF727"
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler);

        var response = await client.MobileSignAsync();

        Assert.True(response.IsSuccess);
        Assert.Equal("0000", response.SiteId);
        Assert.Equal("850FF727", response.DocumentId);
    }

    [Fact]
    public async Task GetMobileStatusAsync_PendingStatus_ShouldReturnPending()
    {
        const string json = """
        {
          "status": 2
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler, opt => opt.ThrowOnError = true);

        var response = await client.GetMobileStatusAsync("BBF3E8C3");

        Assert.Equal(2, response.Status);
        Assert.True(response.IsPending);
        Assert.False(response.IsCompleted);
        Assert.Equal(EImzoMobileStatusCode.PendingUpload, response.StatusCode);
        Assert.Equal("documentId=BBF3E8C3", handler.LastRequestBody);
    }

    [Fact]
    public async Task UploadMobilePkcs7Async_ShouldSendFormParameters()
    {
        const string json = """
        {
          "status": 1,
          "message": ""
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler);

        var request = new MobileUploadRequest("DOC123", "PKCS7_DATA", "SN_999");
        var response = await client.UploadMobilePkcs7Async(request);

        Assert.True(response.IsSuccess);
        Assert.Contains("document_id=DOC123", handler.LastRequestBody);
        Assert.Contains("pkcs7_b64=PKCS7_DATA", handler.LastRequestBody);
        Assert.Contains("serial_number=SN_999", handler.LastRequestBody);
    }

    [Fact]
    public async Task MobileAuthenticateAsync_ShouldSendGetAndReturnCert()
    {
        const string json = """
        {
           "status": 1,
           "subjectCertificateInfo": {
              "serialNumber": "7700000",
              "subjectName": {
                 "1.2.860.3.16.1.2": "30000000000000",
                 "CN": "IVANOV IVAN"
              }
           }
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler);

        var response = await client.MobileAuthenticateAsync("2944F1F2");

        Assert.True(response.IsSuccess);
        Assert.NotNull(response.SubjectCertificateInfo);
        Assert.Equal("30000000000000", response.SubjectCertificateInfo.Pinfl);
        Assert.Equal(HttpMethod.Get, handler.LastRequest!.Method);
        Assert.Equal("backend/mobile/authenticate/2944F1F2", handler.LastRequest.RequestUri!.AbsolutePath.TrimStart('/'));
    }

    [Fact]
    public async Task MobileVerifyAsync_ShouldSendFormAndReturnVerifyResponse()
    {
        const string json = """
        {
           "status": 1,
           "subjectCertificateInfo": {
              "serialNumber": "7700000"
           },
           "verificationInfo": {
              "policyIdentifiers": [
                 "1.2.860.3.2.2.1.2.1"
              ],
              "signingTime": "2022-12-30 12:36:12"
           },
           "pkcs7Attached": "MIAGC....."
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler);

        var response = await client.MobileVerifyAsync("DOC1", "BASE64DOC");

        Assert.True(response.IsSuccess);
        Assert.NotNull(response.VerificationInfo);
        Assert.Equal("MIAGC.....", response.Pkcs7Attached);
        Assert.Contains("documentId=DOC1", handler.LastRequestBody);
        Assert.Contains("document=BASE64DOC", handler.LastRequestBody);
    }

    [Fact]
    public async Task HttpError_ShouldThrowEImzoApiException()
    {
        var handler = MockHttpMessageHandler.CreateJson("{\"error\":\"Internal\"}", HttpStatusCode.InternalServerError);
        var client = CreateClient(handler);

        var ex = await Assert.ThrowsAsync<EImzoApiException>(() => client.PingAsync());
        Assert.Equal(HttpStatusCode.InternalServerError, ex.HttpStatusCode);
    }

    [Fact]
    public async Task ThrowOnError_Enabled_ShouldThrowWhenStatusNotOne()
    {
        const string json = """
        {
          "status": -1,
          "message": "VPN connection missing"
        }
        """;

        var handler = MockHttpMessageHandler.CreateJson(json);
        var client = CreateClient(handler, opt => opt.ThrowOnError = true);

        var ex = await Assert.ThrowsAsync<EImzoApiException>(() => client.AuthenticateAsync("SAMPLE_PKCS7"));
        Assert.Equal(-1, ex.StatusCode);
        Assert.Equal(EImzoStatusCode.VpnOrCertificateCheckFailed, ex.StatusEnum);
    }

    [Fact]
    public void StatusExtensions_ShouldReturnLocalizedDescriptions()
    {
        Assert.Equal("Muvaffaqiyatli", EImzoStatusCode.Success.GetDescription("uz"));
        Assert.Equal("Успешно", EImzoStatusCode.Success.GetDescription("ru"));
        Assert.Equal("Success", EImzoStatusCode.Success.GetDescription("en"));

        Assert.Equal("ЭЦП недействительна", EImzoStatusCode.SignatureInvalid.GetDescription("ru"));
        Assert.Equal("Elektron raqamli imzo (ERI / PKCS#7) haqiqiy emas", EImzoStatusCode.SignatureInvalid.GetDescription("uz"));
    }
}
