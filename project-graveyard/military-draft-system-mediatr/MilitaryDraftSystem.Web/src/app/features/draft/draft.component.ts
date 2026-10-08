import { ChangeDetectionStrategy, Component } from '@angular/core';
import { BackendGapPlaceholderComponent } from '../../shared/components';

@Component({
  selector: 'app-draft',
  standalone: true,
  imports: [BackendGapPlaceholderComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <app-backend-gap-placeholder
      title="Manual draft"
      subtitle="Citizen selection is not available yet"
      message="POST /draft/citizens/{citizenId}/officers/{recruitmentOfficerId} exists on the backend, but there is no endpoint to discover an eligible citizenId to draft. This screen will let you search and draft a citizen once a citizen-listing endpoint exists."
    />
  `
})
export class DraftComponent {}
