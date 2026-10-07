import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  {
    path: 'officers',
    loadChildren: () => import('./features/officers/officers.routes').then((m) => m.OFFICERS_ROUTES)
  }
];
