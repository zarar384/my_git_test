import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { OfficersStore } from '../officers/data/officers.store';
import { BadgeComponent, CardComponent } from '../../shared/components';
import { officerStatusTone } from '../../shared/utils/enum-display';

/**
 * Landing page. Only surfaces what the backend actually exposes: the
 * current player's active officer (if any) via DraftController, plus
 * navigation to the other real features. No invented world/citizen summary
 * is shown here — see BACKEND-GAPS.md for why.
 */
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink, BadgeComponent, CardComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  protected readonly officersStore = inject(OfficersStore);
  protected readonly tone = officerStatusTone;

  ngOnInit(): void {
    if (!this.officersStore.loaded()) {
      this.officersStore.reload();
    }
  }
}
