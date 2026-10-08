import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { PopulationApiService } from '../../../core/api/services/population-api.service';
import { ApiError, CemeteryRecordDto } from '../../../core/api/models';

@Injectable({ providedIn: 'root' })
export class CemeteryStore {
  private readonly api = inject(PopulationApiService);

  private readonly _records = signal<CemeteryRecordDto[]>([]);
  private readonly _loading = signal(false);
  private readonly _error = signal<ApiError | null>(null);
  private readonly _loaded = signal(false);

  readonly records = this._records.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly loaded = this._loaded.asReadonly();

  reload(): void {
    this._loading.set(true);
    this._error.set(null);

    this.api
      .getCemeteryRecords()
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe({
        next: (records) => {
          this._records.set(records);
          this._loaded.set(true);
        },
        error: (error: ApiError) => this._error.set(error)
      });
  }
}
