export type FieldErrors = Record<string, string[]>;

export class ApiError extends Error {
  readonly status: number;
  readonly fieldErrors: FieldErrors;

  constructor(
    message: string,
    status: number,
    fieldErrors: FieldErrors = {},
  ) {
    super(message);

    this.name = 'ApiError';
    this.status = status;
    this.fieldErrors = fieldErrors;
  }
}

function isObject(
  value: unknown,
): value is Record<string, unknown> {
  return typeof value === 'object' && value !== null;
}

export async function readApiError(
  response: Response,
  fallbackMessage: string,
): Promise<ApiError> {
  let body: unknown;

  try {
    body = await response.json();
  } catch {
    return new ApiError(fallbackMessage, response.status);
  }

  if (!isObject(body)) {
    return new ApiError(fallbackMessage, response.status);
  }

  const fieldErrors: FieldErrors = {};

  if (isObject(body.errors)) {
    for (const [key, value] of Object.entries(body.errors)) {
      if (!Array.isArray(value)) {
        continue;
      }

      const messages = value.filter(
        (message): message is string =>
          typeof message === 'string',
      );

      // Title → title, $.dueDate → dueDate
      const fieldName = key.replace(/^\$\./, '');
      const normalizedKey =
        fieldName.charAt(0).toLowerCase() + fieldName.slice(1);

      fieldErrors[normalizedKey] = [
        ...(fieldErrors[normalizedKey] ?? []),
        ...messages,
      ];
    }
  }

  const message =
    typeof body.detail === 'string' && body.detail.trim()
      ? body.detail
      : typeof body.title === 'string' && body.title.trim()
        ? body.title
        : fallbackMessage;

  return new ApiError(message, response.status, fieldErrors);
}