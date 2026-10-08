import { ChangeDetectionStrategy, Component } from '@angular/core';
import { BackendGapPlaceholderComponent } from '../../shared/components';

@Component({
  selector: 'app-population',
  standalone: true,
  imports: [BackendGapPlaceholderComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <app-backend-gap-placeholder
      title="Population"
      subtitle="Citizen records are not available yet"
      message="The backend has no endpoint to list, search, or paginate citizens (PopulationController only exposes cemetery records and lifecycle statistics). This screen will show a searchable citizen roster once that endpoint exists."
    />
  `
})
export class PopulationComponent {}
