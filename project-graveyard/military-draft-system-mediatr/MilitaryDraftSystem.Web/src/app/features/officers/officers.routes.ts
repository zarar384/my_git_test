import { Routes } from '@angular/router';

export const OFFICERS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./officers-list/officers-list.component').then((m) => m.OfficersListComponent)
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./officer-detail/officer-detail.component').then((m) => m.OfficerDetailComponent)
  }
];
