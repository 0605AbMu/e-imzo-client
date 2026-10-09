using System.Text.Json.Serialization;
using EImzo.Client.Models.Common;
using EImzo.Client.Serialization;

namespace EImzo.Client.Models.Health;

/// <summary>
/// Response returned by the /info endpoint.
/// </summary>
public class ServerInfoResponse : EImzoResponse
{
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("serverTime")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? ServerTime { get; set; }

    [JsonPropertyName("vpnKeyInfo")]
    public VpnKeyInfo? VpnKeyInfo { get; set; }

    [JsonPropertyName("trustedCertificates")]
    public List<TrustedCertificateInfo>? TrustedCertificates { get; set; }
}
