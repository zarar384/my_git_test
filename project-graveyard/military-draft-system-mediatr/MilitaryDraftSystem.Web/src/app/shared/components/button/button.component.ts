import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

export type ButtonVariant = 'primary' | 'secondary' | 'danger' | 'ghost';

/**
 * Shared button used across all features so actions look and behave
 * consistently. Supports a busy state so mutating actions can disable
 * themselves while in flight instead of allowing duplicate submissions.
 */
@Component({
  selector: 'app-button',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <button
      class="btn"
      [class]="'btn--' + variant()"
      [attr.type]="type()"
      [disabled]="disabled() || busy()"
      (click)="pressed.emit()"
    >
      @if (busy()) {
        <span class="btn__spinner" aria-hidden="true"></span>
      }
      <span class="btn__label"><ng-content /></span>
    </button>
  `,
  styleUrl: './button.component.scss'
})
export class ButtonComponent {
  readonly variant = input<ButtonVariant>('primary');
  readonly type = input<'button' | 'submit'>('button');
  readonly disabled = input(false);
  readonly busy = input(false);
  readonly pressed = output<void>();
}
