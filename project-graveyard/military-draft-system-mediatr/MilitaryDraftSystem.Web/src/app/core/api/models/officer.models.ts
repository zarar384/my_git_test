import { OfficerStatus } from './enums';

/**
 * MilitaryDraftSystem.Application.Draft.Queries.GetRecruitmentOfficerStats.RecruitmentOfficerStatsDto
 * Returned by GET /draft/officers/{recruitmentOfficerId}/stats
 */
export interface RecruitmentOfficerStatsDto {
  id: string;
  fullName: string;
  department: string;
  draftedCitizensCount: number;
  moralePercent: number;
  guiltIncidentsCount: number;
  isRetired: boolean;
  status: OfficerStatus;
  hasEndedCareer: boolean;
}

/**
 * MilitaryDraftSystem.Application.Draft.Queries.ListRecruitmentOfficers.RecruitmentOfficerSummaryDto
 * Returned by GET /draft/officers
 */
export interface RecruitmentOfficerSummaryDto {
  id: string;
  fullName: string;
  department: string;
  playerId: string | null;
  status: OfficerStatus;
  isActive: boolean;
  hasEndedCareer: boolean;
}

/**
 * Request body for POST /draft/players/{playerId}/officers
 * Mirrors MilitaryDraftSystem.API.Controllers.StartOfficerCareerRequest
 */
export interface StartOfficerCareerRequest {
  fullName: string;
  department: string;
}
