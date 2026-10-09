import { EImzoValidationError } from '../errors/e-imzo-validation-error.js';

const POLYNOMIAL = 0xedb88320;

const CRC_TABLE = new Uint32Array(256);
for (let i = 0; i < 256; i++) {
  let entry = i;
  for (let j = 0; j < 8; j++) {
    if ((entry & 1) === 1) {
      entry = (entry >>> 1) ^ POLYNOMIAL;
    } else {
      entry >>>= 1;
    }
  }
  CRC_TABLE[i] = entry >>> 0;
}

/**
 * Computes 32-bit CRC checksum for a byte array matching IEEE 802.3 and java.util.zip.CRC32.
 */
export function computeCrc32(data: Uint8Array): number {
  if (!data || data.length === 0) {
    return 0;
  }

  let crc = 0xffffffff;
  for (let i = 0; i < data.length; i++) {
    const tableIndex = (crc ^ data[i]) & 0xff;
    crc = (crc >>> 8) ^ CRC_TABLE[tableIndex];
  }

  return (~crc) >>> 0;
}

/**
 * Converts a hexadecimal string to a Uint8Array.
 */
export function hexToBytes(hex: string): Uint8Array {
  if (!hex) {
    return new Uint8Array(0);
  }

  const cleanHex = hex.trim();
  if (cleanHex.length % 2 !== 0) {
    throw new EImzoValidationError('hexString', 'Hexadecimal string must have an even length.');
  }

  const bytes = new Uint8Array(cleanHex.length / 2);
  for (let i = 0; i < cleanHex.length; i += 2) {
    const byte = parseInt(cleanHex.substring(i, i + 2), 16);
    if (Number.isNaN(byte)) {
      throw new EImzoValidationError('hexString', `Invalid hex character at position ${i}.`);
    }
    bytes[i / 2] = byte;
  }

  return bytes;
}

/**
 * Converts a byte array to a hexadecimal string.
 */
export function bytesToHex(bytes: Uint8Array): string {
  let hex = '';
  for (let i = 0; i < bytes.length; i++) {
    hex += bytes[i].toString(16).padStart(2, '0');
  }
  return hex.toLowerCase();
}

/**
 * Computes the CRC32 checksum of decoded hexadecimal bytes and returns it as an 8-character hex string.
 * Matches CRC32(Hex.decode(hexString)).
 */
export function computeHexFromHexString(hexString: string): string {
  if (!hexString || hexString.trim().length === 0) {
    return '00000000';
  }

  const bytes = hexToBytes(hexString);
  const crc = computeCrc32(bytes);
  return crc.toString(16).padStart(8, '0').toLowerCase();
}
