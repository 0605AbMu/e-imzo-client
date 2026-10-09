namespace EImzo.Client.Enums;

/// <summary>
/// Type of digital signature key or hardware token used in E-IMZO, determined by paramSetOID.
/// </summary>
public enum EImzoKeyType
{
    /// <summary>
    /// Unknown or unspecified key type.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// PFX key file.
    /// OIDs: 1.2.860.3.15.1.1.2.1.1, 1.2.860.3.15.2.1.2.1.1
    /// </summary>
    Pfx = 1,

    /// <summary>
    /// E-IMZO-token (ID-card / idcard).
    /// OIDs: 1.2.860.3.15.1.1.2.1.3, 1.2.860.3.15.2.1.2.1.3
    /// </summary>
    IdCard = 2,

    /// <summary>
    /// BAIK-token hardware key.
    /// OID: 1.2.860.3.15.2.1.2.1.2
    /// </summary>
    Baik = 3,

    /// <summary>
    /// UZGUARD-token hardware key.
    /// OID: 1.2.860.3.15.2.1.2.1.4
    /// </summary>
    Uzguard = 4
}
