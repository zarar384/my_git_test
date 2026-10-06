/**
 * TypeScript enums mirroring the backend domain enums exactly.
 * All backend DTOs serialize enums as their string name (verified against
 * the C# DTO property types: `string Status`, `string Reason`, etc.), so no
 * numeric/string enum mismatch exists here — these are plain string unions.
 *
 * Source: MilitaryDraftSystem.Domain/Enums/*.cs
 */

/** MilitaryDraftSystem.Domain.Enums.OfficerStatus */
export type OfficerStatus =
  | 'Active'
  | 'OnLeave'
  | 'Resigned'
  | 'Retired'
  | 'Fired'
  | 'Deceased';

export const OFFICER_STATUS_VALUES: readonly OfficerStatus[] = [
  'Active',
  'OnLeave',
  'Resigned',
  'Retired',
  'Fired',
  'Deceased'
];

/** Officer statuses that mean the officer's career has not ended. */
export const ACTIVE_OFFICER_STATUSES: readonly OfficerStatus[] = ['Active', 'OnLeave'];

/** MilitaryDraftSystem.Domain.Enums.WorkerEndReason */
export type WorkerEndReason =
  | 'Leave'
  | 'Resignation'
  | 'Retirement'
  | 'MoraleCollapse'
  | 'Dismissal'
  | 'AccidentalDeath'
  | 'Suicide'
  | 'DiedOnDuty';

export const WORKER_END_REASON_VALUES: readonly WorkerEndReason[] = [
  'Leave',
  'Resignation',
  'Retirement',
  'MoraleCollapse',
  'Dismissal',
  'AccidentalDeath',
  'Suicide',
  'DiedOnDuty'
];

/** MilitaryDraftSystem.Domain.Enums.DeathReason */
export type DeathReason =
  | 'OldAge'
  | 'HeartAttack'
  | 'Cancer'
  | 'UnknownIllness'
  | 'TrafficAccident'
  | 'Drowned'
  | 'LightningStrike'
  | 'FellFromStairs'
  | 'FriendlyFireDuringTraining'
  | 'KilledInCombat'
  | 'Suicide';

export const DEATH_REASON_VALUES: readonly DeathReason[] = [
  'OldAge',
  'HeartAttack',
  'Cancer',
  'UnknownIllness',
  'TrafficAccident',
  'Drowned',
  'LightningStrike',
  'FellFromStairs',
  'FriendlyFireDuringTraining',
  'KilledInCombat',
  'Suicide'
];

/** MilitaryDraftSystem.Domain.Enums.SubjectType */
export type SubjectType = 'Citizen' | 'RecruitmentOfficer';

export const SUBJECT_TYPE_VALUES: readonly SubjectType[] = ['Citizen', 'RecruitmentOfficer'];
