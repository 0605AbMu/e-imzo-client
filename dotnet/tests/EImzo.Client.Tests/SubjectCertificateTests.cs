using EImzo.Client.Models.Common;
using Xunit;

namespace EImzo.Client.Tests;

public class SubjectCertificateTests
{
    [Fact]
    public void PhysicalPersonCertificate_ShouldMapPropertiesCorrectly()
    {
        var cert = new SubjectCertificateInfo
        {
            SerialNumber = "218711a92",
            X500Name = "CN=IVANOV IVAN IVANOVICH,UID=400000000,1.2.860.3.16.1.2=30000000000000",
            SubjectName = new Dictionary<string, string>
            {
                [SubjectCertificateInfo.OidPinfl] = "30000000000000",
                ["UID"] = "400000000",
                ["CN"] = "IVANOV IVAN IVANOVICH",
                ["Name"] = "IVAN",
                ["SURNAME"] = "IVANOV",
                ["C"] = "UZ"
            }
        };

        Assert.Equal("30000000000000", cert.Pinfl);
        Assert.Equal("400000000", cert.PhysicalPersonTin);
        Assert.Null(cert.LegalEntityTin);
        Assert.Equal("400000000", cert.Tin);
        Assert.Equal("IVANOV IVAN IVANOVICH", cert.CommonName);
        Assert.Equal("IVAN", cert.FirstName);
        Assert.Equal("IVANOV", cert.Surname);
        Assert.Equal("UZ", cert.Country);
        Assert.False(cert.IsLegalEntity);
        Assert.True(cert.IsPhysicalPerson);
    }

    [Fact]
    public void LegalEntityCertificate_ShouldMapPropertiesCorrectly()
    {
        var cert = new SubjectCertificateInfo
        {
            SerialNumber = "99887766",
            SubjectName = new Dictionary<string, string>
            {
                [SubjectCertificateInfo.OidPinfl] = "31111111111111",
                [SubjectCertificateInfo.OidLegalEntityTin] = "123456789",
                ["UID"] = "31111111111111",
                ["CN"] = "DIRECTOR NAME",
                ["O"] = "COMPANY LLC"
            }
        };

        Assert.Equal("31111111111111", cert.Pinfl);
        Assert.Equal("123456789", cert.LegalEntityTin);
        Assert.Equal("123456789", cert.Tin);
        Assert.Equal("COMPANY LLC", cert.Organization);
        Assert.True(cert.IsLegalEntity);
        Assert.False(cert.IsPhysicalPerson);
    }

    [Fact]
    public void NullSubjectName_ShouldReturnNullsSafely()
    {
        var cert = new SubjectCertificateInfo();

        Assert.Null(cert.Pinfl);
        Assert.Null(cert.LegalEntityTin);
        Assert.Null(cert.PhysicalPersonTin);
        Assert.Null(cert.Tin);
        Assert.Null(cert.CommonName);
        Assert.False(cert.IsLegalEntity);
        Assert.True(cert.IsPhysicalPerson);
    }
}
