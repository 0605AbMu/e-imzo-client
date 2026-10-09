using System.Text.Json.Serialization;
using EImzo.Client.Enums;
using EImzo.Client.Models.Common;

namespace EImzo.Client.Models.Mobile;

/// <summary>
/// Response returned by the /frontend/mobile/sign endpoint.
/// </summary>
public class MobileSignResponse : EImzoResponse
{
    [JsonPropertyName("siteId")]
    public string? SiteId { get; set; }

    [JsonPropertyName("documentId")]
    public string? DocumentId { get; set; }
}

/// <summary>
/// Response returned by the /frontend/mobile/status polling endpoint.
/// </summary>
public class MobileStatusResponse : EImzoResponse
{
    /// <summary>
    /// Status code as strongly typed enum for mobile ID-CARD flow.
    /// </summary>
    [JsonIgnore]
    public new EImzoMobileStatusCode StatusCode => Enum.IsDefined(typeof(EImzoMobileStatusCode), Status)
        ? (EImzoMobileStatusCode)Status
        : EImzoMobileStatusCode.Unknown;

    /// <summary>
    /// Indicates whether mobile signing or authentication was completed and uploaded (Status == 1).
    /// </summary>
    [JsonIgnore]
    public bool IsCompleted => Status == (int)EImzoMobileStatusCode.Success;

    /// <summary>
    /// Indicates whether the mobile app is still waiting for user action or uploading (Status == 2).
    /// </summary>
    [JsonIgnore]
    public bool IsPending => Status == (int)EImzoMobileStatusCode.PendingUpload;
}
