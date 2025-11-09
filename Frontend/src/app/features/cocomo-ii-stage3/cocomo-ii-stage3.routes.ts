import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';

export const COCOMO_II_STAGE3_ROUTES: Routes = [
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./cocomo-ii-stage3-landing/cocomo-ii-stage3-landing.component')
      .then(m => m.CocomoIIStage3LandingComponent)
  },
  {
    path: 'parameter-sets',
    canActivate: [authGuard],
    loadChildren: () => import('./parameter-sets/parameter-sets.routes')
      .then(m => m.PARAMETER_SETS_ROUTES)
  },
  {
    path: 'languages',
    canActivate: [authGuard],
    loadChildren: () => import('./languages/languages.routes')
      .then(m => m.LANGUAGES_ROUTES)
  }
];