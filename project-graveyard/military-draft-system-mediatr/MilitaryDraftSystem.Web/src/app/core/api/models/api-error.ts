/**
 * Normalized representation of an API failure, used throughout the
 * application instead of raw HttpErrorResponse objects so components never
 * need to deal with `[object Object]` or stack traces.
 */
export interface ApiError {
  /** HTTP status code, or 0 for network/connectivity failures. */
  status: number;
  /** Short machine-oriented category used to pick a UI treatment. */
  kind: ApiErrorKind;
  /** Human-readable message safe to show directly to the user. */
  message: string;
  /** Raw validation errors keyed by field, if the backend returned any (FluentValidation via ValidationBehavior). */
  validationErrors?: Record<string, string[]>;
}

export type ApiErrorKind =
  | 'validation'
  | 'unauthorized'
  | 'not-found'
  | 'conflict'
  | 'server'
  | 'network'
  | 'unknown';
