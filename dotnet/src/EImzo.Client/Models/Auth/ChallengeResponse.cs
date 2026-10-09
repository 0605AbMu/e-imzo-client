using System.Text.Json.Serialization;
using EImzo.Client.Models.Common;

namespace EImzo.Client.Models.Auth;

/// <summary>
/// Response returned by the /frontend/challenge endpoint.
/// </summary>
public class ChallengeResponse : EImzoResponse
{
    /// <summary>
    /// Temporary unique random challenge string to be signed by the user's digital signature.
    /// </summary>
    [JsonPropertyName("challenge")]
    public string? Challenge { get; set; }

    /// <summary>
    /// Time-to-live for the challenge in seconds.
    /// </summary>
    [JsonPropertyName("ttl")]
    public int Ttl { get; set; }
}
