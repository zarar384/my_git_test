import { ChangeDetectionStrategy, Component, effect, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { StartCareerStore } from '../data/start-career.store';
import { environment } from '../../../../environments/environment';
import { ButtonComponent, CardComponent, ErrorStateComponent } from '../../../shared/components';
import { OfficersStore } from '../../officers/data/officers.store';

/**
 * Lets the single seeded dev player start a new recruitment-officer career.
 * Backend (StartOfficerCareerCommandHandler) enforces the one-active-officer
 * rule; this form only submits fullName/department and surfaces whatever
 * error the backend returns (e.g. a conflict).
 */
@Component({
  selector: 'app-start-career',
  standalone: true,
  imports: [ReactiveFormsModule, ButtonComponent, CardComponent, ErrorStateComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './start-career.component.html',
  styleUrl: './start-career.component.scss'
})
export class StartCareerComponent {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  protected readonly store = inject(StartCareerStore);
  protected readonly officersStore = inject(OfficersStore);

  protected readonly form = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(200)]],
    department: ['', [Validators.required, Validators.maxLength(200)]]
  });

  constructor() {
    effect(() => {
      const officerId = this.store.createdOfficerId();
      if (officerId) {
        this.router.navigate(['/officers', officerId]);
      }
    });
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { fullName, department } = this.form.getRawValue();
    this.store.start(environment.defaultPlayerId, { fullName, department });
  }
}
