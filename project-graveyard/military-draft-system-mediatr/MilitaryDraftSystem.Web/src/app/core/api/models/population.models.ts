import { DeathReason, SubjectType, WorkerEndReason } from './enums';

/**
 * MilitaryDraftSystem.Application.Population.Queries.GetCemeteryRecords.CemeteryRecordDto
 * Returned by GET /population/cemetery
 */
export interface CemeteryRecordDto {
  id: string;
  subjectType: SubjectType;
  subjectId: string;
  fullName: string;
  reason: DeathReason;
  /** ISO 8601 datetime offset string, e.g. "2026-01-01T12:00:00+00:00" */
  diedAt: string;
  originalRecordDeleted: boolean;
}

/**
 * MilitaryDraftSystem.Application.Population.Queries.GetLifecycleStatistics.DeathStatisticDto (nested)
 */
export interface DeathStatisticDto {
  subjectType: SubjectType;
  reason: DeathReason;
  count: number;
}

/**
 * MilitaryDraftSystem.Application.Population.Queries.GetLifecycleStatistics.WorkerLifecycleStatisticDto (nested)
 */
export interface WorkerLifecycleStatisticDto {
  reason: WorkerEndReason;
  count: number;
}

/**
 * MilitaryDraftSystem.Application.Population.Queries.GetLifecycleStatistics.LifecycleStatisticsDto
 * Returned by GET /population/lifecycle-statistics
 */
export interface LifecycleStatisticsDto {
  deathStatistics: DeathStatisticDto[];
  workerLifecycleStatistics: WorkerLifecycleStatisticDto[];
}
