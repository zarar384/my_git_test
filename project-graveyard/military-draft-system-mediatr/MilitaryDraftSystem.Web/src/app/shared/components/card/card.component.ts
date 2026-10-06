import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-card',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="card">
      @if (title()) {
        <header class="card__header">
          <h3 class="card__title">{{ title() }}</h3>
          @if (subtitle()) {
            <p class="card__subtitle">{{ subtitle() }}</p>
          }
        </header>
      }
      <div class="card__body">
        <ng-content />
      </div>
    </section>
  `,
  styleUrl: './card.component.scss'
})
export class CardComponent {
  readonly title = input<string | undefined>(undefined);
  readonly subtitle = input<string | undefined>(undefined);
}
