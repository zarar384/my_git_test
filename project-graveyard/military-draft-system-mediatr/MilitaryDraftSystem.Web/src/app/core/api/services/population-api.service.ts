import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CemeteryRecordDto, LifecycleStatisticsDto } from '../models';

/**
 * Typed wrapper around PopulationController. Only exposes the two endpoints
 * that actually exist today (cemetery, lifecycle-statistics). There is
 * intentionally no citizens-listing method — see BACKEND-GAPS.md gap #1.
 */
@Injectable({ providedIn: 'root' })
export class PopulationApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/population`;

  getCemeteryRecords(): Observable<CemeteryRecordDto[]> {
    return this.http.get<CemeteryRecordDto[]>(`${this.base}/cemetery`);
  }

  getLifecycleStatistics(): Observable<LifecycleStatisticsDto> {
    return this.http.get<LifecycleStatisticsDto>(`${this.base}/lifecycle-statistics`);
  }
}
