import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { ApiError } from '../../../core/api/models';
import { ButtonComponent } from '../button/button.component';

@Component({
  selector: 'app-error-state',
  standalone: true,
  imports: [ButtonComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="error-state" role="alert">
      <h3 class="error-state__title">{{ title() }}</h3>
      <p class="error-state__message">{{ error().message }}</p>
      @if (showRetry()) {
        <app-button variant="secondary" (pressed)="retry.emit()">Try again</app-button>
      }
    </div>
  `,
  styleUrl: './error-state.component.scss'
})
export class ErrorStateComponent {
  readonly title = input('Something went wrong');
  readonly error = input.required<ApiError>();
  readonly showRetry = input(true);
  readonly retry = output<void>();
}
