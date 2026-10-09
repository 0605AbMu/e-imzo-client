using System.Text;
using EImzo.Client.Exceptions;
using EImzo.Client.Utils;
using Xunit;

namespace EImzo.Client.Tests;

public class MobileHelperTests
{
    [Fact]
    public void Crc32_StandardCalculation_ShouldMatchKnownVector()
    {
        // Standard check: "123456789" ASCII CRC32 is 0xCBF43926
        var bytes = Encoding.ASCII.GetBytes("123456789");
        var crc = Crc32Helper.Compute(bytes);

        Assert.Equal(0xCBF43926u, crc);
    }

    [Fact]
    public void Crc32_HexConversion_ShouldMatchExpectedFormat()
    {
        var hex = "01020304";
        var bytes = Crc32Helper.HexToBytes(hex);
        Assert.Equal(4, bytes.Length);
        Assert.Equal(1, bytes[0]);
        Assert.Equal(4, bytes[3]);

        var backToHex = Crc32Helper.BytesToHex(bytes);
        Assert.Equal(hex, backToHex);
    }

    [Fact]
    public void BuildQrCode_ShouldAppendValidCrc32()
    {
        var siteId = "0000";
        var docId = "2944F1F2";
        var hexHash = "F8D2181DC6C02EA819B88FF3EF49BE0C";

        var qrCode = EImzoMobileHelper.BuildQrCode(siteId, docId, hexHash);

        Assert.NotNull(qrCode);
        Assert.StartsWith(siteId + docId + hexHash, qrCode);
        Assert.Equal(siteId.Length + docId.Length + hexHash.Length + 8, qrCode.Length);
        Assert.True(EImzoMobileHelper.VerifyQrCode(qrCode));
    }

    [Fact]
    public void BuildDeepLink_ShouldGenerateStandardEImzoScheme()
    {
        var siteId = "0000";
        var docId = "2944F1F2";
        var hexHash = "F8D2181DC6C02EA819B88FF3EF49BE0C";

        var deepLink = EImzoMobileHelper.BuildDeepLink(siteId, docId, hexHash);

        Assert.StartsWith("eimzo://sign?qc=", deepLink);
        var qrCode = deepLink.Substring("eimzo://sign?qc=".Length);
        Assert.True(EImzoMobileHelper.VerifyQrCode(qrCode));
    }

    [Fact]
    public void VerifyQrCode_CorruptedCrc_ShouldReturnFalse()
    {
        var siteId = "0000";
        var docId = "2944F1F2";
        var hexHash = "F8D2181DC6C02EA819B88FF3EF49BE0C";

        var qrCode = EImzoMobileHelper.BuildQrCode(siteId, docId, hexHash);
        var corrupted = qrCode.Substring(0, qrCode.Length - 1) + "x";

        Assert.False(EImzoMobileHelper.VerifyQrCode(corrupted));
    }

    [Theory]
    [InlineData("", "doc", "hash")]
    [InlineData("site", "", "hash")]
    [InlineData("site", "doc", "")]
    public void BuildQrCode_InvalidArguments_ShouldThrowValidationException(string siteId, string docId, string hexHash)
    {
        Assert.Throws<EImzoValidationException>(() => EImzoMobileHelper.BuildQrCode(siteId, docId, hexHash));
    }
}
