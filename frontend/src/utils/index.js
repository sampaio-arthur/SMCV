/**
 * Format a date to a locale-readable string.
 * @param {string | Date} date
 * @param {string} locale
 * @returns {string}
 */
export function formatDate(date, locale = 'pt-BR') {
  return new Intl.DateTimeFormat(locale, {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  }).format(new Date(date));
}

/**
 * Truncate a string to a maximum length, appending an ellipsis.
 * @param {string} str
 * @param {number} maxLength
 * @returns {string}
 */
export function truncate(str, maxLength = 50) {
  if (!str || str.length <= maxLength) return str;
  return str.slice(0, maxLength) + '…';
}

/**
 * Delay execution for a given number of milliseconds.
 * @param {number} ms
 * @returns {Promise<void>}
 */
export function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

/**
 * Format a byte count to a user-facing file size.
 * @param {number} bytes
 * @returns {string}
 */
export function formatFileSize(bytes) {
  const units = ['B', 'KB', 'MB', 'GB'];
  let size = bytes;
  let unitIndex = 0;

  while (size >= 1024 && unitIndex < units.length - 1) {
    size /= 1024;
    unitIndex += 1;
  }

  return `${Number(size.toFixed(1))} ${units[unitIndex]}`;
}

/**
 * Extract a user-facing error message from an axios error, matching the
 * backend's ExceptionHandlingMiddleware shape: `{ error, details }`.
 * Handles Blob response bodies (responseType: 'blob') by parsing them.
 * @param {unknown} err
 * @param {string} fallback
 * @returns {Promise<string>}
 */
export async function getErrorMessage(err, fallback = 'Ocorreu um erro inesperado.') {
  if (err?.response?.status === 413) {
    return 'Arquivo muito grande. Envie um arquivo menor e tente novamente.';
  }

  const data = err?.response?.data;
  if (!data) return fallback;

  if (typeof Blob !== 'undefined' && data instanceof Blob) {
    try {
      const text = await data.text();
      const parsed = JSON.parse(text);
      return parsed?.error || parsed?.message || fallback;
    } catch {
      return fallback;
    }
  }

  if (typeof data === 'string') return data || fallback;
  return data.error || data.message || data.title || fallback;
}

/**
 * Safely access nested object properties without throwing.
 * @param {object} obj
 * @param {string} path  e.g. 'user.address.city'
 * @param {*} fallback
 * @returns {*}
 */
export function get(obj, path, fallback = undefined) {
  return path.split('.').reduce((acc, key) => acc?.[key], obj) ?? fallback;
}
