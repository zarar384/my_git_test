import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { CardComponent } from '../../../shared/components';

/**
 * Honest "not available yet" screen. Used by Population and Draft features
 * instead of inventing a citizen list/picker the backend does not expose.
 * See BACKEND-GAPS.md gap #1 (and #2 for player identity).
 */
@Component({
  selector: 'app-backend-gap-placeholder',
  standalone: true,
  imports: [CardComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <app-card [title]="title()" [subtitle]="subtitle()">
      <p class="placeholder__message">{{ message() }}</p>
      <p class="placeholder__reference">See <code>BACKEND-GAPS.md</code> for details.</p>
    </app-card>
  `,
  styleUrl: './backend-gap-placeholder.component.scss'
})
export class BackendGapPlaceholderComponent {
  readonly title = input.required<string>();
  readonly subtitle = input<string | undefined>(undefined);
  readonly message = input.required<string>();
}
