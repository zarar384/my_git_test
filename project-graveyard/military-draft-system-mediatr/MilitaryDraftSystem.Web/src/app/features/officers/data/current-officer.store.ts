import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, finalize } from 'rxjs';
import { OfficersApiService } from '../../../core/api/services/officers-api.service';
import { ApiError, RecruitmentOfficerStatsDto } from '../../../core/api/models';
import { OfficersStore } from './officers.store';

export type OfficerLifecycleAction =
  | 'leave'
  | 'returnFromLeave'
  | 'resign'
  | 'retire'
  | 'fire';

/**
 * Store for a single officer's detailed stats and the lifecycle mutations
 * available on them. All eligibility/transition rules remain server-side —
 * this store only reflects the last known status and lets the UI disable an
 * action while a request for it is in flight.
 */
@Injectable({ providedIn: 'root' })
export class CurrentOfficerStore {
  private readonly api = inject(OfficersApiService);
  private readonly officersStore = inject(OfficersStore);

  private readonly _stats = signal<RecruitmentOfficerStatsDto | null>(null);
  private readonly _loading = signal(false);
  private readonly _error = signal<ApiError | null>(null);
  private readonly _pendingAction = signal<OfficerLifecycleAction | null>(null);
  private readonly _actionError = signal<ApiError | null>(null);

  readonly stats = this._stats.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly pendingAction = this._pendingAction.asReadonly();
  readonly actionError = this._actionError.asReadonly();
  readonly isBusy = computed(() => this._pendingAction() !== null);

  loadStats(recruitmentOfficerId: string): void {
    this._loading.set(true);
    this._error.set(null);

    this.api
      .getOfficerStats(recruitmentOfficerId)
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe({
        next: (stats) => this._stats.set(stats),
        error: (error: ApiError) => this._error.set(error)
      });
  }

  goOnLeave(recruitmentOfficerId: string): void {
    this.runAction('leave', this.api.goOnLeave(recruitmentOfficerId), recruitmentOfficerId);
  }

  returnFromLeave(recruitmentOfficerId: string): void {
    this.runAction(
      'returnFromLeave',
      this.api.returnFromLeave(recruitmentOfficerId),
      recruitmentOfficerId
    );
  }

  resign(recruitmentOfficerId: string): void {
    this.runAction('resign', this.api.resign(recruitmentOfficerId), recruitmentOfficerId);
  }

  retire(recruitmentOfficerId: string): void {
    this.runAction('retire', this.api.retire(recruitmentOfficerId), recruitmentOfficerId);
  }

  fire(recruitmentOfficerId: string): void {
    this.runAction('fire', this.api.fire(recruitmentOfficerId), recruitmentOfficerId);
  }

  private runAction(
    action: OfficerLifecycleAction,
    request: Observable<void>,
    recruitmentOfficerId: string
  ): void {
    this._pendingAction.set(action);
    this._actionError.set(null);

    request.pipe(finalize(() => this._pendingAction.set(null))).subscribe({
      next: () => {
        this.loadStats(recruitmentOfficerId);
        this.officersStore.reload();
      },
      error: (error: ApiError) => this._actionError.set(error)
    });
  }
}
