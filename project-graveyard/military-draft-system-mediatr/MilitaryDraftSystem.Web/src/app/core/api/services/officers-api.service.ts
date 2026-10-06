import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  RecruitmentOfficerStatsDto,
  RecruitmentOfficerSummaryDto,
  StartOfficerCareerRequest
} from '../models';

/**
 * Typed wrapper around DraftController. No business rules live here — every
 * method is a thin, literal mapping to one backend endpoint.
 */
@Injectable({ providedIn: 'root' })
export class OfficersApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/draft`;

  listOfficers(): Observable<RecruitmentOfficerSummaryDto[]> {
    return this.http.get<RecruitmentOfficerSummaryDto[]>(`${this.base}/officers`);
  }

  getOfficerStats(recruitmentOfficerId: string): Observable<RecruitmentOfficerStatsDto> {
    return this.http.get<RecruitmentOfficerStatsDto>(
      `${this.base}/officers/${recruitmentOfficerId}/stats`
    );
  }

  startOfficerCareer(playerId: string, request: StartOfficerCareerRequest): Observable<string> {
    return this.http.post<string>(`${this.base}/players/${playerId}/officers`, request);
  }

  goOnLeave(recruitmentOfficerId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/officers/${recruitmentOfficerId}/leave`, {});
  }

  returnFromLeave(recruitmentOfficerId: string): Observable<void> {
    return this.http.post<void>(
      `${this.base}/officers/${recruitmentOfficerId}/return-from-leave`,
      {}
    );
  }

  resign(recruitmentOfficerId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/officers/${recruitmentOfficerId}/resign`, {});
  }

  retire(recruitmentOfficerId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/officers/${recruitmentOfficerId}/retire`, {});
  }

  fire(recruitmentOfficerId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/officers/${recruitmentOfficerId}/fire`, {});
  }

  /**
   * Manual drafting endpoint. Exposed here for completeness and future use,
   * but there is currently no backend way to discover an eligible citizenId
   * — see BACKEND-GAPS.md gap #1. The Draft feature screen surfaces this
   * limitation explicitly rather than inventing a citizen picker.
   */
  draftCitizen(citizenId: string, recruitmentOfficerId: string): Observable<void> {
    return this.http.post<void>(
      `${this.base}/citizens/${citizenId}/officers/${recruitmentOfficerId}`,
      {}
    );
  }
}
