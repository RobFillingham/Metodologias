import { Routes } from '@angular/router';
import { authGuard } from '../../../core/guards/auth.guard';

export const ESTIMATIONS_ROUTES: Routes = [
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./estimation-list/estimation-list.component')
      .then(m => m.EstimationListComponent)
  },
  {
    path: ':estimationId',
    canActivate: [authGuard],
    loadComponent: () => import('./estimation-detail/estimation-detail.component')
      .then(m => m.EstimationDetailComponent)
  }
];
