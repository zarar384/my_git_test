import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { PopulationApiService } from '../../../core/api/services/population-api.service';
import { ApiError, LifecycleStatisticsDto } from '../../../core/api/models';

@Injectable({ providedIn: 'root' })
export class StatisticsStore {
  private readonly api = inject(PopulationApiService);

  private readonly _statistics = signal<LifecycleStatisticsDto | null>(null);
  private readonly _loading = signal(false);
  private readonly _error = signal<ApiError | null>(null);
  private readonly _loaded = signal(false);

  readonly statistics = this._statistics.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly loaded = this._loaded.asReadonly();

  reload(): void {
    this._loading.set(true);
    this._error.set(null);

    this.api
      .getLifecycleStatistics()
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe({
        next: (statistics) => {
          this._statistics.set(statistics);
          this._loaded.set(true);
        },
        error: (error: ApiError) => this._error.set(error)
      });
  }
}
