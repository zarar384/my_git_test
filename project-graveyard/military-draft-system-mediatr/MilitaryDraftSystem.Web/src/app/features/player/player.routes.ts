import { Routes } from '@angular/router';

export const PLAYER_ROUTES: Routes = [
  {
    path: 'start-career',
    loadComponent: () =>
      import('./start-career/start-career.component').then((m) => m.StartCareerComponent)
  }
];
