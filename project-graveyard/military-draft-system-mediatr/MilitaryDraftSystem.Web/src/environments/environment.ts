/**
 * Development environment configuration.
 *
 * The API key below matches the backend's dev-local default
 * (MilitaryDraftSystem.API/appsettings.json -> Authentication:ApiKey).
 * It is NOT a real secret — it only works against a locally running
 * development instance of the API and must never be treated as proof
 * of identity. See BACKEND-GAPS.md for the authentication/player-identity
 * limitations this implies for the client.
 */
export const environment = {
  production: false,
  apiBaseUrl: '/api',
  apiKey: 'dev-local-api-key',
  /**
   * There is currently no backend endpoint to discover or authenticate as a
   * Player (see BACKEND-GAPS.md, gap #2). The only known Player is the single
   * seeded one (PlayerSeeder.DefaultPlayerId). This is a temporary stand-in,
   * isolated here so it can be replaced by real player identity later
   * without touching feature code.
   */
  defaultPlayerId: '11111111-1111-1111-1111-111111111111',
  /** Polling interval (ms) for screens that need to reflect world state without a realtime transport. */
  pollingIntervalMs: 15000
};
