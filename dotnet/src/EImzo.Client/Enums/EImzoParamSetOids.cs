namespace EImzo.Client.Enums;

/// <summary>
/// OID constants and resolver for E-IMZO key types determined by paramSetOID.
/// </summary>
public static class EImzoParamSetOids
{
    /// <summary>
    /// PFX key file parameter set OID variant 1.
    /// </summary>
    public const string Pfx1 = "1.2.860.3.15.1.1.2.1.1";

    /// <summary>
    /// PFX key file parameter set OID variant 2.
    /// </summary>
    public const string Pfx2 = "1.2.860.3.15.2.1.2.1.1";

    /// <summary>
    /// E-IMZO-token (ID-card) parameter set OID variant 1.
    /// </summary>
    public const string IdCard1 = "1.2.860.3.15.1.1.2.1.3";

    /// <summary>
    /// E-IMZO-token (ID-card) parameter set OID variant 2.
    /// </summary>
    public const string IdCard2 = "1.2.860.3.15.2.1.2.1.3";

    /// <summary>
    /// BAIK-token parameter set OID.
    /// </summary>
    public const string Baik = "1.2.860.3.15.2.1.2.1.2";

    /// <summary>
    /// UZGUARD-token parameter set OID.
    /// </summary>
    public const string Uzguard = "1.2.860.3.15.2.1.2.1.4";

    /// <summary>
    /// Resolves the <see cref="EImzoKeyType"/> corresponding to the specified paramSetOID string.
    /// </summary>
    /// <param name="paramSetOid">The paramSetOID string from E-IMZO response.</param>
    /// <returns>The identified <see cref="EImzoKeyType"/>, or <see cref="EImzoKeyType.Unknown"/> if unmapped or null.</returns>
    public static EImzoKeyType ResolveKeyType(string? paramSetOid)
    {
        if (string.IsNullOrWhiteSpace(paramSetOid))
        {
            return EImzoKeyType.Unknown;
        }

        switch (paramSetOid!.Trim())
        {
            case Pfx1:
            case Pfx2:
                return EImzoKeyType.Pfx;

            case IdCard1:
            case IdCard2:
                return EImzoKeyType.IdCard;

            case Baik:
                return EImzoKeyType.Baik;

            case Uzguard:
                return EImzoKeyType.Uzguard;

            default:
                return EImzoKeyType.Unknown;
        }
    }
}
