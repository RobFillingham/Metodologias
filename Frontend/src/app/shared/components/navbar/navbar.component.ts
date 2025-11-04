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
          <a routerLink="/home" class="brand-link">
            <span class="brand-text">COCOMO II</span>
          </a>
        </div>

        <div class="nav-menu">
          <a routerLink="/home" class="nav-link">Home</a>

          <!-- Show these links only when authenticated -->
          <ng-container *ngIf="authService.isAuthenticated">
            <a routerLink="/dashboard" class="nav-link">Dashboard</a>
            <a routerLink="/projects" class="nav-link">Projects</a>
            <a href="#" class="nav-link" disabled>Estimations</a>
          </ng-container>
        </div>

        <div class="nav-actions">
          <ng-container *ngIf="!authService.isAuthenticated">
            <a routerLink="/auth/login" class="btn btn-outline">Login</a>
            <a routerLink="/auth/register" class="btn btn-primary">Register</a>
          </ng-container>

          <ng-container *ngIf="authService.isAuthenticated">
            <span class="user-greeting">
              Hello, {{ authService.currentUserValue?.firstName }}
            </span>
            <button (click)="logout()" class="btn btn-outline">Logout</button>
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

    .nav-brand .brand-link {
      text-decoration: none;
      color: #667eea;
      font-weight: bold;
      font-size: 1.25rem;
    }

    .nav-menu {
      display: flex;
      gap: 2rem;
      align-items: center;
    }

    .nav-link {
      color: #666;
      text-decoration: none;
      font-weight: 500;
      transition: color 0.3s ease;
    }

    .nav-link:hover:not(:disabled) {
      color: #667eea;
    }

    .nav-link:disabled {
      color: #ccc;
      cursor: not-allowed;
    }

    .nav-actions {
      display: flex;
      align-items: center;
      gap: 1rem;
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
      color: #667eea;
      border-color: #667eea;
    }

    .btn-outline:hover {
      background: #667eea;
      color: white;
    }

    .btn-primary {
      background: #667eea;
      color: white;
      border-color: #667eea;
    }

    .btn-primary:hover {
      background: #5a6fd8;
      border-color: #5a6fd8;
    }

    @media (max-width: 768px) {
      .nav-container {
        flex-direction: column;
        height: auto;
        padding: 1rem;
      }

      .nav-menu {
        margin: 1rem 0;
        gap: 1rem;
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
}