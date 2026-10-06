import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

export type BadgeTone = 'neutral' | 'success' | 'warning' | 'danger' | 'info';

/**
 * Generic status badge. Feature components decide the tone by mapping their
 * own domain enum (OfficerStatus, DeathReason, etc.) to a BadgeTone — this
 * component carries no domain knowledge itself.
 */
@Component({
  selector: 'app-badge',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<span class="badge" [class]="'badge--' + tone()">{{ label() }}</span>`,
  styleUrl: './badge.component.scss'
})
export class BadgeComponent {
  readonly label = input.required<string>();
  readonly tone = input<BadgeTone>('neutral');

  protected readonly resolvedTone = computed(() => this.tone());
}
