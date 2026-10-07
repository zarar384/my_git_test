import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { CurrentOfficerStore, OfficerLifecycleAction } from '../data/current-officer.store';
import { OfficersStore } from '../data/officers.store';
import {
  BadgeComponent,
  ButtonComponent,
  CardComponent,
  ConfirmDialogComponent,
  ErrorStateComponent,
  LoadingStateComponent
} from '../../../shared/components';
import { officerStatusTone } from '../../../shared/utils/enum-display';

type DestructiveAction = Extract<OfficerLifecycleAction, 'resign' | 'retire' | 'fire'>;

const DESTRUCTIVE_ACTION_COPY: Record<DestructiveAction, { title: string; message: string }> = {
  resign: {
    title: 'Resign this officer?',
    message: 'This permanently ends their career. This cannot be undone.'
  },
  retire: {
    title: 'Retire this officer?',
    message: 'This permanently ends their career. This cannot be undone.'
  },
  fire: {
    title: 'Fire this officer?',
    message: 'This permanently ends their career. This cannot be undone.'
  }
};

@Component({
  selector: 'app-officer-detail',
  standalone: true,
  imports: [
    RouterLink,
    BadgeComponent,
    ButtonComponent,
    CardComponent,
    ConfirmDialogComponent,
    ErrorStateComponent,
    LoadingStateComponent
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './officer-detail.component.html',
  styleUrl: './officer-detail.component.scss'
})
export class OfficerDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);

  protected readonly officerId = toSignal(
    this.route.paramMap.pipe(map((params) => params.get('id') ?? '')),
    { initialValue: '' }
  );

  protected readonly store = inject(CurrentOfficerStore);
  protected readonly officersStore = inject(OfficersStore);

  protected readonly summary = computed(() =>
    this.officersStore.officers().find((o) => o.id === this.officerId()) ?? null
  );

  protected readonly tone = officerStatusTone;
  protected readonly pendingDestructiveAction = signal<DestructiveAction | null>(null);
  protected readonly dialogCopy = computed(() => {
    const action = this.pendingDestructiveAction();
    return action ? DESTRUCTIVE_ACTION_COPY[action] : null;
  });

  ngOnInit(): void {
    const id = this.officerId();
    if (id) {
      this.store.loadStats(id);
    }
    if (!this.officersStore.loaded()) {
      this.officersStore.reload();
    }
  }

  protected goOnLeave(): void {
    this.store.goOnLeave(this.officerId());
  }

  protected returnFromLeave(): void {
    this.store.returnFromLeave(this.officerId());
  }

  protected requestDestructiveAction(action: DestructiveAction): void {
    this.pendingDestructiveAction.set(action);
  }

  protected cancelDestructiveAction(): void {
    this.pendingDestructiveAction.set(null);
  }

  protected confirmDestructiveAction(): void {
    const action = this.pendingDestructiveAction();
    const id = this.officerId();
    this.pendingDestructiveAction.set(null);

    switch (action) {
      case 'resign':
        this.store.resign(id);
        break;
      case 'retire':
        this.store.retire(id);
        break;
      case 'fire':
        this.store.fire(id);
        break;
    }
  }
}
