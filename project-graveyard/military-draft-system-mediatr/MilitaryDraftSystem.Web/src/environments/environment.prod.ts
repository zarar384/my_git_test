/**
 * Production environment configuration.
 *
 * NOTE: the apiKey/defaultPlayerId placeholders below are inherited from the
 * same dev-local stand-ins as environment.ts (see that file and
 * BACKEND-GAPS.md gaps #2/#3) because the backend has no real player
 * identity or production secret yet. This file intentionally does not
 * import environment.ts — Angular's file-replacement swaps this file in
 * for environment.ts at prod build time, so importing it would be
 * self-referential.
 */
export const environment = {
  production: true,
  apiBaseUrl: '/api',
  apiKey: 'dev-local-api-key',
  defaultPlayerId: '11111111-1111-1111-1111-111111111111',
  pollingIntervalMs: 15000
};
