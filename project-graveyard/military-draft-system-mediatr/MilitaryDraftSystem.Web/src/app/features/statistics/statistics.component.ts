import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { StatisticsStore } from './data/statistics.store';
import {
  BadgeComponent,
  CardComponent,
  EmptyStateComponent,
  ErrorStateComponent,
  LoadingStateComponent
} from '../../shared/components';
import { deathReasonTone, humanizeEnumValue, workerEndReasonTone } from '../../shared/utils/enum-display';

@Component({
  selector: 'app-statistics',
  standalone: true,
  imports: [BadgeComponent, CardComponent, EmptyStateComponent, ErrorStateComponent, LoadingStateComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './statistics.component.html',
  styleUrl: './statistics.component.scss'
})
export class StatisticsComponent implements OnInit {
  protected readonly store = inject(StatisticsStore);
  protected readonly deathTone = deathReasonTone;
  protected readonly workerTone = workerEndReasonTone;
  protected readonly humanize = humanizeEnumValue;

  ngOnInit(): void {
    if (!this.store.loaded()) {
      this.store.reload();
    }
  }
}
