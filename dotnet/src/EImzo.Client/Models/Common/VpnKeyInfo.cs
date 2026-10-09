using System.Text.Json.Serialization;
using EImzo.Client.Serialization;

namespace EImzo.Client.Models.Common;

/// <summary>
/// Information about the E-IMZO-SERVER VPN key.
/// </summary>
public class VpnKeyInfo
{
    [JsonPropertyName("serialNumber")]
    public string? SerialNumber { get; set; }

    [JsonPropertyName("X500Name")]
    public string? X500Name { get; set; }

    [JsonPropertyName("validFrom")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? ValidFrom { get; set; }

    [JsonPropertyName("validTo")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? ValidTo { get; set; }

    [JsonPropertyName("subjectInfo")]
    public Dictionary<string, string>? SubjectInfo { get; set; }
}
