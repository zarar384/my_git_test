import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { OfficersApiService } from '../../../core/api/services/officers-api.service';
import { ApiError, StartOfficerCareerRequest } from '../../../core/api/models';
import { OfficersStore } from '../../officers/data/officers.store';

/**
 * Store for the "start a new career" flow. Backend enforces the
 * one-active-officer-per-player rule (StartOfficerCareerCommandHandler); this
 * store does not duplicate that check, it only surfaces whatever error the
 * backend returns (e.g. a conflict if an active officer already exists).
 */
@Injectable({ providedIn: 'root' })
export class StartCareerStore {
  private readonly api = inject(OfficersApiService);
  private readonly officersStore = inject(OfficersStore);

  private readonly _submitting = signal(false);
  private readonly _error = signal<ApiError | null>(null);
  private readonly _createdOfficerId = signal<string | null>(null);

  readonly submitting = this._submitting.asReadonly();
  readonly error = this._error.asReadonly();
  readonly createdOfficerId = this._createdOfficerId.asReadonly();

  start(playerId: string, request: StartOfficerCareerRequest): void {
    this._submitting.set(true);
    this._error.set(null);
    this._createdOfficerId.set(null);

    this.api
      .startOfficerCareer(playerId, request)
      .pipe(finalize(() => this._submitting.set(false)))
      .subscribe({
        next: (officerId) => {
          this._createdOfficerId.set(officerId);
          this.officersStore.reload();
        },
        error: (error: ApiError) => this._error.set(error)
      });
  }

  reset(): void {
    this._error.set(null);
    this._createdOfficerId.set(null);
  }
}
