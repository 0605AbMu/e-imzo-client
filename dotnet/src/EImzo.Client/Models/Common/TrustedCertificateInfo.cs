using System.Text.Json.Serialization;
using EImzo.Client.Serialization;

namespace EImzo.Client.Models.Common;

/// <summary>
/// Information about a trusted root/intermediate certificate installed on E-IMZO-SERVER.
/// </summary>
public class TrustedCertificateInfo
{
    [JsonPropertyName("serialNumber")]
    public string? SerialNumber { get; set; }

    [JsonPropertyName("validFrom")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? ValidFrom { get; set; }

    [JsonPropertyName("validTo")]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? ValidTo { get; set; }

    [JsonPropertyName("subjectInfo")]
    public Dictionary<string, string>? SubjectInfo { get; set; }
}
