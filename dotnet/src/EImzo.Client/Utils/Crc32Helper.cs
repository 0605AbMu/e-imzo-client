using System.Globalization;

namespace EImzo.Client.Utils;

/// <summary>
/// High-performance IEEE 802.3 CRC32 calculator matching java.util.zip.CRC32.
/// Used for E-IMZO Mobile QR-code and Deeplink checksums.
/// </summary>
public static class Crc32Helper
{
    private const uint Polynomial = 0xEDB88320;
    private static readonly uint[] Table = InitializeTable();

    private static uint[] InitializeTable()
    {
        var table = new uint[256];
        for (uint i = 0; i < 256; i++)
        {
            var entry = i;
            for (var j = 0; j < 8; j++)
            {
                if ((entry & 1) == 1)
                {
                    entry = (entry >> 1) ^ Polynomial;
                }
                else
                {
                    entry >>= 1;
                }
            }
            table[i] = entry;
        }
        return table;
    }

    /// <summary>
    /// Computes the 32-bit CRC checksum for a byte array.
    /// </summary>
    /// <param name="data">The input bytes.</param>
    /// <returns>Unsigned 32-bit integer checksum.</returns>
    public static uint Compute(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            return 0;
        }

        return Compute(data, 0, data.Length);
    }

    /// <summary>
    /// Computes the 32-bit CRC checksum for a slice of bytes.
    /// </summary>
    public static uint Compute(byte[] data, int offset, int length)
    {
        if (data == null || length <= 0)
        {
            return 0;
        }

        var crc = 0xFFFFFFFF;
        for (var i = offset; i < offset + length; i++)
        {
            var tableIndex = (byte)((crc ^ data[i]) & 0xFF);
            crc = (crc >> 8) ^ Table[tableIndex];
        }

        return ~crc;
    }

    /// <summary>
    /// Computes the CRC32 checksum of decoded hexadecimal bytes and returns it as an 8-character hex string.
    /// Matches the calculation: CRC32(Hex.decode(hexString)).
    /// </summary>
    /// <param name="hexString">Hexadecimal string (e.g. siteId + documentId + hashString).</param>
    /// <returns>8-character zero-padded lowercase hex string.</returns>
    public static string ComputeHexFromHexString(string hexString)
    {
        if (string.IsNullOrEmpty(hexString))
        {
            return "00000000";
        }

        var bytes = HexToBytes(hexString);
        var crc = Compute(bytes);
        return crc.ToString("x8", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts a hexadecimal string to a byte array.
    /// </summary>
    public static byte[] HexToBytes(string hex)
    {
        if (string.IsNullOrEmpty(hex))
        {
            return [];
        }

        if (hex.Length % 2 != 0)
        {
            hex = "0" + hex;
        }

        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return bytes;
    }

    /// <summary>
    /// Converts a byte array to a hexadecimal string.
    /// </summary>
    public static string BytesToHex(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
        {
            return string.Empty;
        }

        var c = new char[bytes.Length * 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            var b = bytes[i];
            c[i * 2] = GetHexChar(b >> 4);
            c[i * 2 + 1] = GetHexChar(b & 0x0F);
        }
        return new string(c);
    }

    private static char GetHexChar(int value)
    {
        return (char)(value < 10 ? '0' + value : 'a' + (value - 10));
    }
}
