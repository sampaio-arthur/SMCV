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
 * Extract a user-facing error message from an axios error, matching the
 * backend's ExceptionHandlingMiddleware shape: `{ error, details }`.
 * Handles Blob response bodies (responseType: 'blob') by parsing them.
 * @param {unknown} err
 * @param {string} fallback
 * @returns {Promise<string>}
 */
export async function getErrorMessage(err, fallback = 'Ocorreu um erro inesperado.') {
  const data = err?.response?.data;
  if (!data) return fallback;

  if (typeof Blob !== 'undefined' && data instanceof Blob) {
    try {
      const text = await data.text();
      const parsed = JSON.parse(text);
      return parsed?.error || fallback;
    } catch {
      return fallback;
    }
  }

  if (typeof data === 'string') return data || fallback;
  return data.error || fallback;
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
