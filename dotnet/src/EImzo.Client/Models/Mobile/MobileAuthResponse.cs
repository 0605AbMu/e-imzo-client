using System.Text.Json.Serialization;
using EImzo.Client.Models.Common;

namespace EImzo.Client.Models.Mobile;

/// <summary>
/// Response returned by the /frontend/mobile/auth endpoint.
/// </summary>
public class MobileAuthResponse : EImzoResponse
{
    [JsonPropertyName("siteId")]
    public string? SiteId { get; set; }

    [JsonPropertyName("documentId")]
    public string? DocumentId { get; set; }

    /// <summary>
    /// Temporary random challenge value.
    /// Handled for both server key "challange" and conventional "challenge".
    /// </summary>
    [JsonPropertyName("challange")]
    public string? ChallengeRaw { get; set; }

    [JsonPropertyName("challenge")]
    public string? ChallengeStandard { get; set; }

    /// <summary>
    /// Gets the challenge string (reading either "challange" or "challenge").
    /// </summary>
    [JsonIgnore]
    public string? Challenge => ChallengeRaw ?? ChallengeStandard;
}
