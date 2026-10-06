import { BadgeTone } from '../components/badge/badge.component';
import { DeathReason, OfficerStatus, WorkerEndReason } from '../../core/api/models';

/** Maps OfficerStatus to a visual tone. Backend remains the source of truth for the value itself. */
export function officerStatusTone(status: OfficerStatus): BadgeTone {
  switch (status) {
    case 'Active':
      return 'success';
    case 'OnLeave':
      return 'info';
    case 'Resigned':
    case 'Retired':
      return 'neutral';
    case 'Fired':
    case 'Deceased':
      return 'danger';
  }
}

/** Maps WorkerEndReason to a visual tone for lifecycle/cemetery displays. */
export function workerEndReasonTone(reason: WorkerEndReason): BadgeTone {
  switch (reason) {
    case 'Retirement':
      return 'neutral';
    case 'Resignation':
    case 'Leave':
      return 'info';
    case 'MoraleCollapse':
    case 'Dismissal':
      return 'warning';
    case 'AccidentalDeath':
    case 'Suicide':
    case 'DiedOnDuty':
      return 'danger';
  }
}

/** Maps DeathReason to a visual tone for the cemetery/statistics screens. */
export function deathReasonTone(reason: DeathReason): BadgeTone {
  switch (reason) {
    case 'OldAge':
      return 'neutral';
    case 'KilledInCombat':
    case 'FriendlyFireDuringTraining':
    case 'Suicide':
      return 'danger';
    default:
      return 'warning';
  }
}

/** Splits a PascalCase enum member into readable words, e.g. "KilledInCombat" -> "Killed In Combat". */
export function humanizeEnumValue(value: string): string {
  return value.replace(/([a-z0-9])([A-Z])/g, '$1 $2');
}
