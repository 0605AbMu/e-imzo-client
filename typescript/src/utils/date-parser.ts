/**
 * Parses flexible date string formats commonly returned by E-IMZO-SERVER,
 * such as "2022-10-06 16:47:29", "2025.10.21 11:13:57", or ISO-8601 strings.
 *
 * @param dateStr Raw date string from server.
 * @returns Date object or undefined if parsing failed.
 */
export function parseEImzoDate(dateStr?: string | null): Date | undefined {
  if (!dateStr || typeof dateStr !== 'string' || dateStr.trim().length === 0) {
    return undefined;
  }

  const trimmed = dateStr.trim();

  // Try direct Date constructor (works for ISO 8601, etc.)
  const directDate = new Date(trimmed);
  if (!Number.isNaN(directDate.getTime())) {
    return directDate;
  }

  // Handle "YYYY.MM.DD HH:mm:ss" or "YYYY/MM/DD HH:mm:ss"
  const normalized = trimmed.replace(/\./g, '-').replace(/\//g, '-').replace(' ', 'T');
  const normalizedDate = new Date(normalized);
  if (!Number.isNaN(normalizedDate.getTime())) {
    return normalizedDate;
  }

  return undefined;
}
