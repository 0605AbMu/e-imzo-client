using EImzo.Client.Models.Auth;
using EImzo.Client.Models.Common;
using EImzo.Client.Models.Health;
using EImzo.Client.Models.Mobile;
using EImzo.Client.Models.Pkcs7;

namespace EImzo.Client;

/// <summary>
/// Strongly-typed client interface for the Uzbekistan E-IMZO and E-IMZO-SERVER REST-API.
/// </summary>
public interface IEImzoClient
{
    #region Health and Diagnostic Endpoints

    /// <summary>
    /// Checks VPN connection and returns server time and VPN key info (GET /ping).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PingResponse> PingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves detailed E-IMZO server information, version, VPN key, and trusted certificates (GET /info).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<ServerInfoResponse> GetInfoAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Web E-IMZO Frontend Endpoints

    /// <summary>
    /// Generates a temporary unique random Challenge string for user digital signature (GET /frontend/challenge).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<ChallengeResponse> GetChallengeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Attaches a trusted timestamp token to a PKCS#7 document (POST /frontend/timestamp/pkcs7).
    /// </summary>
    /// <param name="pkcs7Base64">Base64 encoded PKCS#7 document.</param>
    /// <param name="realIp">Client IP address (sent in X-Real-IP). Falls back to DefaultRealIp if null.</param>
    /// <param name="host">Domain name of the requesting site (sent in Host header). Falls back to DefaultHost if null.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<AttachTimestampResponse> AttachTimestampAsync(
        string pkcs7Base64,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a PKCS#7 Attached document by combining the original document and a detached PKCS#7 signature (POST /frontend/pkcs7/make-attached).
    /// </summary>
    /// <param name="documentBase64">Base64 encoded original document.</param>
    /// <param name="pkcs7DetachedBase64">Base64 encoded detached PKCS#7 signature.</param>
    /// <param name="realIp">Client IP address (X-Real-IP).</param>
    /// <param name="host">Domain name (Host header).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<MakeAttachedResponse> MakeAttachedAsync(
        string documentBase64,
        string pkcs7DetachedBase64,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Merges two PKCS#7 Attached documents containing the same document into a single multi-signed container (POST /frontend/pkcs7/join).
    /// </summary>
    /// <param name="pkcs7AttachedBase64A">First Base64 encoded PKCS#7 Attached document.</param>
    /// <param name="pkcs7AttachedBase64B">Second Base64 encoded PKCS#7 Attached document.</param>
    /// <param name="realIp">Client IP address (X-Real-IP).</param>
    /// <param name="host">Domain name (Host header).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<JoinAttachedResponse> JoinAttachedAsync(
        string pkcs7AttachedBase64A,
        string pkcs7AttachedBase64B,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Web E-IMZO Backend Endpoints

    /// <summary>
    /// Authenticates a user by validating the PKCS#7 signed Challenge document (POST /backend/auth).
    /// </summary>
    /// <param name="pkcs7Base64">Base64 encoded PKCS#7 document containing the signed challenge.</param>
    /// <param name="realIp">Client IP address (sent in X-Real-IP). Falls back to DefaultRealIp if null.</param>
    /// <param name="host">Domain name of the site (sent in Host header). Falls back to DefaultHost if null.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<AuthResponse> AuthenticateAsync(
        string pkcs7Base64,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the digital signature and certificate of a PKCS#7 Attached document (POST /backend/pkcs7/verify/attached).
    /// </summary>
    /// <param name="pkcs7AttachedBase64">Base64 encoded PKCS#7 Attached document.</param>
    /// <param name="realIp">Client IP address (sent in X-Real-IP). Falls back to DefaultRealIp if null.</param>
    /// <param name="host">Domain name of the site (sent in Host header). Falls back to DefaultHost if null.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<AttachedVerifyResponse> VerifyAttachedAsync(
        string pkcs7AttachedBase64,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the digital signature and certificate of a PKCS#7 Detached document with its original document (POST /backend/pkcs7/verify/detached).
    /// </summary>
    /// <param name="documentBase64">Base64 encoded original document.</param>
    /// <param name="pkcs7DetachedBase64">Base64 encoded detached PKCS#7 signature.</param>
    /// <param name="realIp">Client IP address (sent in X-Real-IP). Falls back to DefaultRealIp if null.</param>
    /// <param name="host">Domain name of the site (sent in Host header). Falls back to DefaultHost if null.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<DetachedVerifyResponse> VerifyDetachedAsync(
        string documentBase64,
        string pkcs7DetachedBase64,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Mobile ID-CARD Frontend Endpoints

    /// <summary>
    /// Initializes mobile authentication, returning SiteID, DocumentID, and Challenge (POST /frontend/mobile/auth).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<MobileAuthResponse> MobileAuthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Initializes mobile document signing, returning SiteID and DocumentID (POST /frontend/mobile/sign).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<MobileSignResponse> MobileSignAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Polls the status of a mobile signing or authentication operation by DocumentID (POST /frontend/mobile/status).
    /// </summary>
    /// <param name="documentId">The DocumentID returned from MobileAuth or MobileSign.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<MobileStatusResponse> GetMobileStatusAsync(string documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a PKCS#7 Detached document from mobile client or mock server (POST /frontend/mobile/upload).
    /// </summary>
    /// <param name="request">Mobile upload parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<EImzoResponse> UploadMobilePkcs7Async(MobileUploadRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a PKCS#7 Detached document from mobile client or mock server (POST /frontend/mobile/upload).
    /// </summary>
    /// <param name="documentId">Temporary DocumentID.</param>
    /// <param name="pkcs7Base64">Base64 encoded PKCS#7 document.</param>
    /// <param name="serialNumber">Certificate serial number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<EImzoResponse> UploadMobilePkcs7Async(
        string documentId,
        string pkcs7Base64,
        string serialNumber,
        CancellationToken cancellationToken = default);

    #endregion

    #region Mobile ID-CARD Backend Endpoints

    /// <summary>
    /// Verifies the mobile authentication result on the backend after status polling reports 1 (GET /backend/mobile/authenticate/{DocumentID}).
    /// </summary>
    /// <param name="documentId">The temporary DocumentID.</param>
    /// <param name="realIp">Client IP address (sent in X-Real-IP). Falls back to DefaultRealIp if null.</param>
    /// <param name="host">Domain name (sent in Host header). Falls back to DefaultHost if null.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<MobileAuthenticateResponse> MobileAuthenticateAsync(
        string documentId,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the signed document on the backend after mobile status polling reports 1 (POST /backend/mobile/verify).
    /// </summary>
    /// <param name="request">Verification request containing DocumentId and Base64 encoded document.</param>
    /// <param name="realIp">Client IP address (sent in X-Real-IP). Falls back to DefaultRealIp if null.</param>
    /// <param name="host">Domain name (sent in Host header). Falls back to DefaultHost if null.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<MobileVerifyResponse> MobileVerifyAsync(
        MobileVerifyRequest request,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the signed document on the backend after mobile status polling reports 1 (POST /backend/mobile/verify).
    /// </summary>
    /// <param name="documentId">The temporary DocumentID.</param>
    /// <param name="documentBase64">Base64 encoded document that was signed.</param>
    /// <param name="realIp">Client IP address (sent in X-Real-IP). Falls back to DefaultRealIp if null.</param>
    /// <param name="host">Domain name (sent in Host header). Falls back to DefaultHost if null.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<MobileVerifyResponse> MobileVerifyAsync(
        string documentId,
        string documentBase64,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the signed document bytes on the backend after mobile status polling reports 1 (POST /backend/mobile/verify).
    /// </summary>
    /// <param name="documentId">The temporary DocumentID.</param>
    /// <param name="documentBytes">Raw bytes of the document that was signed.</param>
    /// <param name="realIp">Client IP address (sent in X-Real-IP). Falls back to DefaultRealIp if null.</param>
    /// <param name="host">Domain name (sent in Host header). Falls back to DefaultHost if null.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<MobileVerifyResponse> MobileVerifyAsync(
        string documentId,
        byte[] documentBytes,
        string? realIp = null,
        string? host = null,
        CancellationToken cancellationToken = default);

    #endregion
}
