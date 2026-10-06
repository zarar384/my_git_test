import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { environment } from '../../../../environments/environment';

/**
 * Attaches the development API key to every outgoing request to this app's
 * own backend. This is NOT real authentication — see BACKEND-GAPS.md. The
 * key is a shared dev-local secret already committed to the backend's own
 * appsettings.json, so keeping it in the frontend's environment file does
 * not expose anything beyond what the backend repository already discloses.
 */
export const apiKeyInterceptor: HttpInterceptorFn = (req, next) => {
  if (!req.url.startsWith(environment.apiBaseUrl)) {
    return next(req);
  }

  const cloned = req.clone({
    setHeaders: {
      'X-Api-Key': environment.apiKey
    }
  });

  return next(cloned);
};
