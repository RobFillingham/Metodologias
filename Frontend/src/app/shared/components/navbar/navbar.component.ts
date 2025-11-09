import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <nav class="navbar">
      <div class="nav-container">
        <div class="nav-brand">
          <span class="brand-text">Herramienta de Estimación</span>
        </div>

        <div class="nav-center">
          <ng-container *ngIf="authService.isAuthenticated">
            <button (click)="goToDashboard()" class="btn btn-center">
              Dashboard
            </button>
          </ng-container>
        </div>

        <div class="nav-actions">
          <ng-container *ngIf="!authService.isAuthenticated">
            <a routerLink="/auth/login" class="btn btn-outline">Iniciar Sesión</a>
            <a routerLink="/auth/register" class="btn btn-primary">Registrarse</a>
          </ng-container>

          <ng-container *ngIf="authService.isAuthenticated">
            <span class="user-greeting">
              Hola, {{ authService.currentUserValue?.firstName }}
            </span>
            <button (click)="logout()" class="btn btn-outline">Cerrar Sesión</button>
          </ng-container>
        </div>
      </div>
    </nav>
  `,
  styles: [`
    .navbar {
      background: white;
      border-bottom: 1px solid #e1e5e9;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
      position: sticky;
      top: 0;
      z-index: 1000;
    }

    .nav-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 1rem;
      display: flex;
      justify-content: space-between;
      align-items: center;
      height: 60px;
    }

    .nav-brand {
      text-align: left;
      flex: 0 0 auto;
    }

    .brand-text {
      color: #2563eb;
      font-weight: bold;
      font-size: 1.25rem;
    }

    .nav-center {
      flex: 1;
      display: flex;
      justify-content: center;
      align-items: center;
    }

    .nav-actions {
      display: flex;
      align-items: center;
      gap: 1rem;
      flex: 0 0 auto;
    }

    .user-greeting {
      color: #666;
      font-weight: 500;
      margin-right: 0.5rem;
    }

    .btn {
      padding: 0.5rem 1rem;
      border-radius: 6px;
      font-size: 0.9rem;
      font-weight: 500;
      text-decoration: none;
      cursor: pointer;
      transition: all 0.3s ease;
      border: 2px solid transparent;
    }

    .btn-outline {
      background: transparent;
      color: #2563eb;
      border-color: #2563eb;
    }

    .btn-outline:hover {
      background: #2563eb;
      color: white;
    }

    .btn-primary {
      background: #2563eb;
      color: white;
      border-color: #2563eb;
    }

    .btn-primary:hover {
      background: #1d4ed8;
      border-color: #1d4ed8;
    }

    .btn-center {
      background: #2563eb;
      color: white;
      border-color: #2563eb;
    }

    .btn-center:hover {
      background: #1d4ed8;
      border-color: #1d4ed8;
    }

    @media (max-width: 768px) {
      .nav-container {
        flex-direction: column;
        height: auto;
        padding: 1rem;
      }

      .nav-brand {
        text-align: center;
        margin-bottom: 1rem;
      }

      .nav-actions {
        margin-top: 1rem;
      }
    }
  `]
})
export class NavbarComponent {
  authService = inject(AuthService);
  private router = inject(Router);

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/home']);
  }

  goToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }
}