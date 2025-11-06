import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';

export const LANGUAGES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./languages.component')
      .then(m => m.LanguagesComponent),
    canActivate: [authGuard]
  },
  {
    path: ':id',
    loadComponent: () => import('./language-detail/language-detail.component')
      .then(m => m.LanguageDetailComponent),
    canActivate: [authGuard]
  }
];