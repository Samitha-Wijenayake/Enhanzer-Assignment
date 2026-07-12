import { Routes } from '@angular/router';

import { authGuard, guestGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/login/login').then((m) => m.LoginComponent),
    title: 'Login • TestEnhanzer',
  },
  {
    path: 'purchase-bill',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/purchase-bill/purchase-bill').then((m) => m.PurchaseBillComponent),
    title: 'Purchase Bill • TestEnhanzer',
  },
  { path: '', pathMatch: 'full', redirectTo: 'purchase-bill' },
  { path: '**', redirectTo: 'purchase-bill' },
];
