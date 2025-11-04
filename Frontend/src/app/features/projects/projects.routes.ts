import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';

export const PROJECTS_ROUTES: Routes = [
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./project-list/project-list.component').then(m => m.ProjectListComponent)
  }
];
