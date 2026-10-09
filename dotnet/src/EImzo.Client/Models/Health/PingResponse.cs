using System.Text.Json.Serialization;
using EImzo.Client.Models.Common;
using EImzo.Client.Serialization;

namespace EImzo.Client.Models.Health;

/// <summary>
/// Response returned by the /ping endpoint.
/// </summary>
public class PingResponse : EImzoResponse
{
    [JsonPropertyName("serverDateTime")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? ServerDateTime { get; set; }

    [JsonPropertyName("yourIP")]
    public string? YourIp { get; set; }

    [JsonPropertyName("vpnKeyInfo")]
    public VpnKeyInfo? VpnKeyInfo { get; set; }
}
