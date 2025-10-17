import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  },
  {
    path: 'home',
    loadChildren: () => import('./features/home/home.routes').then(m => m.HOME_ROUTES)
  },
  // Example of protected route
  // {
  //   path: 'dashboard',
  //   canActivate: [authGuard],
  //   loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
  // },
  {
    path: '**',
    redirectTo: 'home'
  }
];
