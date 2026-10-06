import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-loading-state',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="loading-state" role="status" [attr.aria-label]="label()">
      <span class="loading-state__spinner" aria-hidden="true"></span>
      <span class="loading-state__label">{{ label() }}</span>
    </div>
  `,
  styleUrl: './loading-state.component.scss'
})
export class LoadingStateComponent {
  readonly label = input('Loading…');
}
