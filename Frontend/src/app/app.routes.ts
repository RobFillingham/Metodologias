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
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'projects',
    canActivate: [authGuard],
    loadChildren: () => import('./features/projects/projects.routes').then(m => m.PROJECTS_ROUTES)
  },
  {
    path: 'cocomo2',
    canActivate: [authGuard],
    children: [
      {
        path: '',
        loadComponent: () => import('./features/cocomo2/cocomo2-landing/cocomo2-landing.component').then(m => m.Cocomo2LandingComponent)
      },
      {
        path: 'parameter-sets',
        loadChildren: () => import('./features/cocomo2/parameter-sets/parameter-sets.routes').then(m => m.PARAMETER_SETS_ROUTES)
      },
      {
        path: 'languages',
        loadChildren: () => import('./features/cocomo2/languages/languages.routes').then(m => m.LANGUAGES_ROUTES)
      },
      {
        path: 'estimations/:projectId',
        loadChildren: () => import('./features/cocomo2/estimations/estimations.routes').then(m => m.ESTIMATIONS_ROUTES)
      }
    ]
  },
  {
    path: 'kloc',
    canActivate: [authGuard],
    loadComponent: () => import('./features/kloc/kloc-estimations/kloc-estimations.component').then(m => m.KlocEstimationsComponent)
  },
  {
    path: 'function-point',
    canActivate: [authGuard],
    loadComponent: () => import('./features/function-point/function-point-estimations/function-point-estimations.component').then(m => m.FunctionPointEstimationsComponent)
  },
  {
    path: 'use-case-point',
    canActivate: [authGuard],
    loadComponent: () => import('./features/use-case-point/use-case-point-estimations/use-case-point-estimations.component').then(m => m.UseCasePointEstimationsComponent)
  },
  {
    path: '**',
    redirectTo: 'home'
  }
];
