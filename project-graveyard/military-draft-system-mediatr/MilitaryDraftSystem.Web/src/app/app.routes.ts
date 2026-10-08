import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent)
  },
  {
    path: 'officers',
    loadChildren: () => import('./features/officers/officers.routes').then((m) => m.OFFICERS_ROUTES)
  },
  {
    path: 'player',
    loadChildren: () => import('./features/player/player.routes').then((m) => m.PLAYER_ROUTES)
  },
  {
    path: 'cemetery',
    loadComponent: () =>
      import('./features/cemetery/cemetery.component').then((m) => m.CemeteryComponent)
  },
  {
    path: 'statistics',
    loadComponent: () =>
      import('./features/statistics/statistics.component').then((m) => m.StatisticsComponent)
  },
  {
    path: 'population',
    loadComponent: () =>
      import('./features/population/population.component').then((m) => m.PopulationComponent)
  },
  {
    path: 'draft',
    loadComponent: () => import('./features/draft/draft.component').then((m) => m.DraftComponent)
  },
  { path: '**', redirectTo: 'dashboard' }
];
