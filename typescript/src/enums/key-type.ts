/**
 * Type of digital signature key or hardware token used in E-IMZO, determined by paramSetOID.
 */
export enum EImzoKeyType {
  /**
   * Unknown or unspecified key type.
   */
  Unknown = 'Unknown',

  /**
   * PFX key file.
   * OIDs: 1.2.860.3.15.1.1.2.1.1, 1.2.860.3.15.2.1.2.1.1
   */
  Pfx = 'Pfx',

  /**
   * E-IMZO-token (ID-card / idcard).
   * OIDs: 1.2.860.3.15.1.1.2.1.3, 1.2.860.3.15.2.1.2.1.3
   */
  IdCard = 'IdCard',

  /**
   * BAIK-token hardware key.
   * OID: 1.2.860.3.15.2.1.2.1.2
   */
  Baik = 'Baik',

  /**
   * UZGUARD-token hardware key.
   * OID: 1.2.860.3.15.2.1.2.1.4
   */
  Uzguard = 'Uzguard',
}
