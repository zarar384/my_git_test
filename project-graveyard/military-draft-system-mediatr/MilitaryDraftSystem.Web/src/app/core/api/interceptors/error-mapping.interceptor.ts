import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { ApiError, ApiErrorKind } from '../models/api-error';

/**
 * Maps every failed HTTP response from the backend into a normalized
 * ApiError so the rest of the app never has to interpret HttpErrorResponse,
 * raw ProblemDetails payloads, or network failures directly.
 */
export const errorMappingInterceptor: HttpInterceptorFn = (req, next) =>
  next(req).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse) {
        return throwError(() => mapHttpError(error));
      }

      return throwError(
        () =>
          ({
            status: 0,
            kind: 'unknown',
            message: 'An unexpected error occurred.'
          }) satisfies ApiError
      );
    })
  );

function mapHttpError(error: HttpErrorResponse): ApiError {
  if (error.status === 0) {
    return {
      status: 0,
      kind: 'network',
      message: 'Could not reach the server. Check your connection and try again.'
    };
  }

  const kind = kindFromStatus(error.status);
  const validationErrors = extractValidationErrors(error.error);

  return {
    status: error.status,
    kind,
    message: extractMessage(error, kind),
    validationErrors
  };
}

function kindFromStatus(status: number): ApiErrorKind {
  switch (status) {
    case 400:
      return 'validation';
    case 401:
    case 403:
      return 'unauthorized';
    case 404:
      return 'not-found';
    case 409:
      return 'conflict';
    default:
      return status >= 500 ? 'server' : 'unknown';
  }
}

function extractMessage(error: HttpErrorResponse, kind: ApiErrorKind): string {
  const body = error.error;

  if (typeof body === 'string' && body.trim().length > 0) {
    return body;
  }

  if (body && typeof body === 'object') {
    const problem = body as { title?: string; detail?: string; message?: string };
    if (problem.detail) {
      return problem.detail;
    }
    if (problem.title) {
      return problem.title;
    }
    if (problem.message) {
      return problem.message;
    }
  }

  switch (kind) {
    case 'unauthorized':
      return 'Your API key is missing or invalid.';
    case 'not-found':
      return 'The requested item could not be found. It may no longer exist.';
    case 'conflict':
      return 'This action could not be completed because the current state has changed.';
    case 'validation':
      return 'The request was rejected because some values were invalid.';
    case 'server':
      return 'The server encountered an error while processing the request.';
    default:
      return 'An unexpected error occurred.';
  }
}

function extractValidationErrors(body: unknown): Record<string, string[]> | undefined {
  if (!body || typeof body !== 'object') {
    return undefined;
  }

  const errors = (body as { errors?: Record<string, string[]> }).errors;
  return errors && typeof errors === 'object' ? errors : undefined;
}
