import { Injectable, computed, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { OfficersApiService } from '../../../core/api/services/officers-api.service';
import { ApiError, RecruitmentOfficerSummaryDto } from '../../../core/api/models';
import { ACTIVE_OFFICER_STATUSES } from '../../../core/api/models/enums';
import { environment } from '../../../../environments/environment';

/**
 * Signal-based store for the officers list (GET /draft/officers). Server is
 * the source of truth; this store only caches the latest response and
 * exposes convenience selectors (current player's active officer, etc.)
 * derived purely from that response — no independent lifecycle logic.
 */
@Injectable({ providedIn: 'root' })
export class OfficersStore {
  private readonly api = inject(OfficersApiService);

  private readonly _officers = signal<RecruitmentOfficerSummaryDto[]>([]);
  private readonly _loading = signal(false);
  private readonly _error = signal<ApiError | null>(null);
  private readonly _loaded = signal(false);

  readonly officers = this._officers.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly loaded = this._loaded.asReadonly();

  /**
   * The officer belonging to the current (stand-in) player whose career has
   * not ended, if any. This is the "current career" referenced throughout
   * the UI.
   */
  readonly currentOfficer = computed(() =>
    this._officers().find(
      (o) => o.playerId === environment.defaultPlayerId && ACTIVE_OFFICER_STATUSES.includes(o.status)
    ) ?? null
  );

  readonly hasActiveOfficer = computed(() => this.currentOfficer() !== null);

  readonly myOfficers = computed(() =>
    this._officers().filter((o) => o.playerId === environment.defaultPlayerId)
  );

  readonly otherOfficers = computed(() =>
    this._officers().filter((o) => o.playerId !== environment.defaultPlayerId)
  );

  reload(): void {
    this._loading.set(true);
    this._error.set(null);

    this.api
      .listOfficers()
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe({
        next: (officers) => {
          this._officers.set(officers);
          this._loaded.set(true);
        },
        error: (error: ApiError) => this._error.set(error)
      });
  }
}
