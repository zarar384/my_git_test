import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { OfficersStore } from '../data/officers.store';
import { OfficerStatus, RecruitmentOfficerSummaryDto } from '../../../core/api/models';
import { BadgeComponent, CardComponent, EmptyStateComponent, ErrorStateComponent, LoadingStateComponent } from '../../../shared/components';
import { officerStatusTone } from '../../../shared/utils/enum-display';
import { OFFICER_STATUS_VALUES } from '../../../core/api/models/enums';
import { environment } from '../../../../environments/environment';

type StatusFilter = OfficerStatus | 'All';

@Component({
  selector: 'app-officers-list',
  standalone: true,
  imports: [RouterLink, BadgeComponent, CardComponent, EmptyStateComponent, ErrorStateComponent, LoadingStateComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './officers-list.component.html',
  styleUrl: './officers-list.component.scss'
})
export class OfficersListComponent implements OnInit {
  protected readonly store = inject(OfficersStore);

  protected readonly statusFilter = signal<StatusFilter>('All');
  protected readonly statusOptions: StatusFilter[] = ['All', ...OFFICER_STATUS_VALUES];

  protected readonly filteredOfficers = computed(() => {
    const filter = this.statusFilter();
    const all = this.store.officers();
    return filter === 'All' ? all : all.filter((o) => o.status === filter);
  });

  protected readonly tone = officerStatusTone;

  ngOnInit(): void {
    if (!this.store.loaded()) {
      this.store.reload();
    }
  }

  protected setFilter(status: StatusFilter): void {
    this.statusFilter.set(status);
  }

  protected isMine(officer: RecruitmentOfficerSummaryDto): boolean {
    return officer.playerId === environment.defaultPlayerId;
  }

  protected trackById = (officer: RecruitmentOfficerSummaryDto) => officer.id;
}
