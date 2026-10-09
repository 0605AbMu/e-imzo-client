using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EImzo.Client.Exceptions;
using EImzo.Client.Models.Auth;
using EImzo.Client.Models.Common;
using EImzo.Client.Models.Health;
using EImzo.Client.Models.Mobile;
using EImzo.Client.Models.Pkcs7;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EImzo.Client;

/// <summary>
/// Default implementation of the unofficial <see cref="IEImzoClient"/> interface.
/// </summary>
public class EImzoClient : IEImzoClient
{
    private readonly HttpClient _httpClient;
    private readonly EImzoClientOptions _options;
    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [ActivatorUtilitiesConstructor]
    public EImzoClient(HttpClient httpClient, IOptions<EImzoClientOptions>? options = null)
        : this(httpClient, options?.Value ?? new EImzoClientOptions())
    {
    }

    public EImzoClient(HttpClient httpClient, EImzoClientOptions? options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? new EImzoClientOptions();

        if (_httpClient.BaseAddress == null && _options.BaseUrl != null)
        {
            _httpClient.BaseAddress = _options.BaseUrl;
        }

        if (_options.Timeout > TimeSpan.Zero)
        {
            _httpClient.Timeout = _options.Timeout;
        }
    }

    #region Health and Diagnostics

    public async Task<PingResponse> PingAsync(CancellationToken cancellationToken = default)
    {
        const string endpoint = "ping";
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        var response = await SendAndDeserializeAsync<PingResponse>(request, endpoint, cancellationToken).ConfigureAwait(false);
        HandleStatusCheck(response, endpoint);
        return response;
    }

    public async Task<ServerInfoResponse> GetInfoAsync(CancellationToken cancellationToken = default)
    {
        const string endpoint = "info";
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        var response = await SendAndDeserializeAsync<ServerInfoResponse>(request, endpoint, cancellationToken).ConfigureAwait(false);
        HandleStatusCheck(response, endpoint);
        return response;
    }

    #endregion

    #region Web E-IMZO Frontend

    public async Task<ChallengeResponse> GetChallengeAsync(CancellationToken cancellationToken = default)
    {
        const string endpoint = "frontend/challenge";
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        var response = await SendAndDeserializeAsync<ChallengeResponse>(request, endpoint, cancellationToken).ConfigureAwait(false);
        HandleStatusCheck(response, endpoint);
        return response;
    }

    public async Task<AttachTimestampResponse> AttachTimestampAsync(
        string pkcs7Base64,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default)
    {
        ValidateRequired(pkcs7Base64, nameof(pkcs7Base64));

        const string endpoint = "frontend/timestamp/pkcs7";
        using var request = CreateRequest(HttpMethod.Post, endpoint, realIp, host);
        request.Content = new StringContent(pkcs7Base64.Trim(), Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await SendAndDeserializeAsync<AttachTimestampResponse>(request, endpoint, cancellationToken).ConfigureAwait(false);
        HandleStatusCheck(response, endpoint);
        return response;
    }

    public async Task<MakeAttachedResponse> MakeAttachedAsync(
        string documentBase64,
        string pkcs7DetachedBase64,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default)
    {
        ValidateRequired(documentBase64, nameof(documentBase64));
        ValidateRequired(pkcs7DetachedBase64, nameof(pkcs7DetachedBase64));

        const string endpoint = "frontend/pkcs7/make-attached";
        using var request = CreateRequest(HttpMethod.Post, endpoint, realIp, host);
        var body = $"{documentBase64.Trim()}|{pkcs7DetachedBase64.Trim()}";
        request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await SendAndDeserializeAsync<MakeAttachedResponse>(request, endpoint, cancellationToken).ConfigureAwait(false);
        HandleStatusCheck(response, endpoint);
        return response;
    }

    public async Task<JoinAttachedResponse> JoinAttachedAsync(
        string pkcs7AttachedBase64A,
        string pkcs7AttachedBase64B,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default)
    {
        ValidateRequired(pkcs7AttachedBase64A, nameof(pkcs7AttachedBase64A));
        ValidateRequired(pkcs7AttachedBase64B, nameof(pkcs7AttachedBase64B));

        const string endpoint = "frontend/pkcs7/join";
        using var request = CreateRequest(HttpMethod.Post, endpoint, realIp, host);
        var body = $"{pkcs7AttachedBase64A.Trim()}|{pkcs7AttachedBase64B.Trim()}";
        request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await SendAndDeserializeAsync<JoinAttachedResponse>(request, endpoint, cancellationToken).ConfigureAwait(false);
        HandleStatusCheck(response, endpoint);
        return response;
    }

    #endregion

    #region Web E-IMZO Backend

    public async Task<AuthResponse> AuthenticateAsync(
        string pkcs7Base64,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default)
    {
        ValidateRequired(pkcs7Base64, nameof(pkcs7Base64));

        const string endpoint = "backend/auth";
        using var request = CreateRequest(HttpMethod.Post, endpoint, realIp, host);
        request.Content = new StringContent(pkcs7Base64.Trim(), Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await SendAndDeserializeAsync<AuthResponse>(request, endpoint, cancellationToken).ConfigureAwait(false);
        HandleStatusCheck(response, endpoint);
        return response;
    }

    public async Task<AttachedVerifyResponse> VerifyAttachedAsync(
        string pkcs7AttachedBase64,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default)
    {
        ValidateRequired(pkcs7AttachedBase64, nameof(pkcs7AttachedBase64));

        const string endpoint = "backend/pkcs7/verify/attached";
        using var request = CreateRequest(HttpMethod.Post, endpoint, realIp, host);
        request.Content = new StringContent(pkcs7AttachedBase64.Trim(), Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await SendAndDeserializeAsync<AttachedVerifyResponse>(request, endpoint, cancellationToken).ConfigureAwait(false);
        HandleStatusCheck(response, endpoint);
        return response;
    }

    public async Task<DetachedVerifyResponse> VerifyDetachedAsync(
        string documentBase64,
        string pkcs7DetachedBase64,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default)
    {
        ValidateRequired(documentBase64, nameof(documentBase64));
        ValidateRequired(pkcs7DetachedBase64, nameof(pkcs7DetachedBase64));

        const string endpoint = "backend/pkcs7/verify/detached";
        using var request = CreateRequest(HttpMethod.Post, endpoint, realIp, host);
        var body = $"{documentBase64.Trim()}|{pkcs7DetachedBase64.Trim()}";
        request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await SendAndDeserializeAsync<DetachedVerifyResponse>(request, endpoint, cancellationToken).ConfigureAwait(false);
        HandleStatusCheck(response, endpoint);
        return response;
    }

    #endregion

    #region Mobile ID-CARD Frontend

    public async Task<MobileAuthResponse> MobileAuthAsync(CancellationToken cancellationToken = default)
    {
        const string endpoint = "frontend/mobile/auth";
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        var response = await SendAndDeserializeAsync<MobileAuthResponse>(request, endpoint, cancellationToken).ConfigureAwait(false);
        HandleStatusCheck(response, endpoint);
        return response;
    }

    public async Task<MobileSignResponse> MobileSignAsync(CancellationToken cancellationToken = default)
    {
        const string endpoint = "frontend/mobile/sign";
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        var response = await SendAndDeserializeAsync<MobileSignResponse>(request, endpoint, cancellationToken).ConfigureAwait(false);
        HandleStatusCheck(response, endpoint);
        return response;
    }

    public async Task<MobileStatusResponse> GetMobileStatusAsync(string documentId, CancellationToken cancellationToken = default)
    {
        ValidateRequired(documentId, nameof(documentId));

        const string endpoint = "frontend/mobile/status";
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["documentId"] = documentId.Trim()
        });

        var response = await SendAndDeserializeAsync<MobileStatusResponse>(request, endpoint, cancellationToken).ConfigureAwait(false);
        // Do not auto-throw on Status == 2 (PendingUpload) since polling loop relies on Status == 2
        if (_options.ThrowOnError && !response.IsSuccess && !response.IsPending)
        {
            response.EnsureSuccess(endpoint);
        }

        return response;
    }

    public Task<EImzoResponse> UploadMobilePkcs7Async(
        string documentId,
        string pkcs7Base64,
        string serialNumber,
        CancellationToken cancellationToken = default)
    {
        return UploadMobilePkcs7Async(new MobileUploadRequest(documentId, pkcs7Base64, serialNumber), cancellationToken);
    }

    public async Task<EImzoResponse> UploadMobilePkcs7Async(MobileUploadRequest uploadRequest, CancellationToken cancellationToken = default)
    {
        if (uploadRequest == null)
        {
            throw new ArgumentNullException(nameof(uploadRequest));
        }

        ValidateRequired(uploadRequest.DocumentId, nameof(uploadRequest.DocumentId));
        ValidateRequired(uploadRequest.Pkcs7Base64, nameof(uploadRequest.Pkcs7Base64));
        ValidateRequired(uploadRequest.SerialNumber, nameof(uploadRequest.SerialNumber));

        const string endpoint = "frontend/mobile/upload";
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["document_id"] = uploadRequest.DocumentId.Trim(),
            ["pkcs7_b64"] = uploadRequest.Pkcs7Base64.Trim(),
            ["serial_number"] = uploadRequest.SerialNumber.Trim()
        });

        var response = await SendAndDeserializeAsync<EImzoResponse>(request, endpoint, cancellationToken).ConfigureAwait(false);
        HandleStatusCheck(response, endpoint);
        return response;
    }

    #endregion

    #region Mobile ID-CARD Backend

    public async Task<MobileAuthenticateResponse> MobileAuthenticateAsync(
        string documentId,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default)
    {
        ValidateRequired(documentId, nameof(documentId));

        var endpoint = $"backend/mobile/authenticate/{Uri.EscapeDataString(documentId.Trim())}";
        using var request = CreateRequest(HttpMethod.Get, endpoint, realIp, host);

        var response = await SendAndDeserializeAsync<MobileAuthenticateResponse>(request, endpoint, cancellationToken).ConfigureAwait(false);
        HandleStatusCheck(response, endpoint);
        return response;
    }

    public Task<MobileVerifyResponse> MobileVerifyAsync(
        string documentId,
        string documentBase64,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default)
    {
        return MobileVerifyAsync(new MobileVerifyRequest(documentId, documentBase64), realIp, host, cancellationToken);
    }

    public Task<MobileVerifyResponse> MobileVerifyAsync(
        string documentId,
        byte[] documentBytes,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default)
    {
        if (documentBytes == null)
        {
            throw new ArgumentNullException(nameof(documentBytes));
        }

        return MobileVerifyAsync(new MobileVerifyRequest(documentId, documentBytes), realIp, host, cancellationToken);
    }

    public async Task<MobileVerifyResponse> MobileVerifyAsync(
        MobileVerifyRequest verifyRequest,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default)
    {
        if (verifyRequest == null)
        {
            throw new ArgumentNullException(nameof(verifyRequest));
        }

        ValidateRequired(verifyRequest.DocumentId, nameof(verifyRequest.DocumentId));
        ValidateRequired(verifyRequest.DocumentBase64, nameof(verifyRequest.DocumentBase64));

        const string endpoint = "backend/mobile/verify";
        using var request = CreateRequest(HttpMethod.Post, endpoint, realIp, host);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["documentId"] = verifyRequest.DocumentId.Trim(),
            ["document"] = verifyRequest.DocumentBase64.Trim()
        });

        var response = await SendAndDeserializeAsync<MobileVerifyResponse>(request, endpoint, cancellationToken).ConfigureAwait(false);
        HandleStatusCheck(response, endpoint);
        return response;
    }

    #endregion

    #region Helper Methods

    private HttpRequestMessage CreateRequest(HttpMethod method, string uri, string? realIp, string? host)
    {
        var request = new HttpRequestMessage(method, uri);

        var resolvedHost = host ?? _options.DefaultHost;
        if (!string.IsNullOrWhiteSpace(resolvedHost))
        {
            request.Headers.Host = resolvedHost;
        }

        var resolvedRealIp = realIp ?? _options.DefaultRealIp;
        if (!string.IsNullOrWhiteSpace(resolvedRealIp))
        {
            request.Headers.TryAddWithoutValidation("X-Real-IP", resolvedRealIp);
        }

        return request;
    }

    private async Task<T> SendAndDeserializeAsync<T>(
        HttpRequestMessage request,
        string endpoint,
        CancellationToken cancellationToken)
        where T : class, new()
    {
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new EImzoApiException($"HTTP communication error calling '{endpoint}': {ex.Message}", endpoint, null, ex);
        }

        using (response)
        {
            var contentString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new EImzoApiException(
                    (int)response.StatusCode,
                    $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}. Response body: {contentString}",
                    endpoint,
                    response.StatusCode);
            }

            if (string.IsNullOrWhiteSpace(contentString))
            {
                return new T();
            }

            try
            {
                var result = JsonSerializer.Deserialize<T>(contentString, DefaultJsonOptions);
                return result ?? new T();
            }
            catch (JsonException jsonEx)
            {
                throw new EImzoApiException(
                    $"Failed to deserialize response from '{endpoint}'. Raw content: {contentString}",
                    endpoint,
                    response.StatusCode,
                    jsonEx);
            }
        }
    }

    private void HandleStatusCheck(EImzoResponse? response, string endpoint)
    {
        if (_options.ThrowOnError && response != null && !response.IsSuccess)
        {
            response.EnsureSuccess(endpoint);
        }
    }

    private static void ValidateRequired(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new EImzoValidationException(paramName, $"{paramName} cannot be null, empty, or whitespace.");
        }
    }

    #endregion
}
