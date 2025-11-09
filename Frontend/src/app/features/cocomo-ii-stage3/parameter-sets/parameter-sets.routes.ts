import { Routes } from '@angular/router';
import { authGuard } from '../../../core/guards/auth.guard';

export const PARAMETER_SETS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./parameter-sets.component')
      .then(m => m.ParameterSetsComponent),
    canActivate: [authGuard]
  },
  {
    path: 'create',
    loadComponent: () => import('./parameter-set-form/parameter-set-form.component')
      .then(m => m.ParameterSetFormComponent),
    canActivate: [authGuard]
  },
  {
    path: ':id',
    loadComponent: () => import('./parameter-set-detail/parameter-set-detail.component')
      .then(m => m.ParameterSetDetailComponent),
    canActivate: [authGuard]
  },
  {
    path: ':id/edit',
    loadComponent: () => import('./parameter-set-form/parameter-set-form.component')
      .then(m => m.ParameterSetFormComponent),
    canActivate: [authGuard]
  }
];