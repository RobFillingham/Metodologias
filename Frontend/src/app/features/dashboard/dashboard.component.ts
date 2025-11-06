import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  template: `
    <app-navbar></app-navbar>

    <div class="dashboard-container">
      <main class="dashboard-content">
        <div class="quick-actions">
          <div class="action-card">
            <h3>📁 Projects</h3>
            <p>Create and manage your estimation projects</p>
            <button class="btn btn-primary" (click)="goToProjects()">Manage Projects</button>
          </div>

          <div class="action-card">
            <h3>⚙️ Parameter Sets</h3>
            <p>Customize parameters for your organization</p>
            <button class="btn btn-primary" (click)="goToParameterSets()">Manage Parameter Sets</button>
          </div>

          <div class="action-card">
            <h3>� Lenguajes</h3>
            <p>View and manage programming languages</p>
            <button class="btn btn-primary" (click)="goToLanguages()">Manage Languages</button>
          </div>
        </div>
      </main>
    </div>
  `,
  styles: [`
    .dashboard-container {
      min-height: 100vh;
      background: #f8f9fa;
    }

    .dashboard-header {
      background: white;
      border-bottom: 1px solid #e1e5e9;
      padding: 1rem 2rem;
      display: flex;
      justify-content: space-between;
      align-items: center;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    .dashboard-header h1 {
      margin: 0;
      color: #333;
    }

    .user-info {
      display: flex;
      align-items: center;
      gap: 1rem;
    }

    .user-info span {
      color: #666;
    }

    .dashboard-content {
      padding: 2rem;
      max-width: 1200px;
      margin: 0 auto;
    }

    .welcome-section {
      background: white;
      padding: 2rem;
      border-radius: 12px;
      margin-bottom: 2rem;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .welcome-section h2 {
      margin-top: 0;
      color: #333;
    }

    .quick-actions {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 1.5rem;
      margin-bottom: 2rem;
    }

    .action-card {
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      text-align: center;
    }

    .action-card h3 {
      margin-top: 0;
      color: #333;
    }

    .action-card p {
      color: #666;
      margin-bottom: 1.5rem;
    }

    .info-section {
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .info-section h3 {
      margin-top: 0;
      color: #333;
    }

    .info-section p {
      color: #666;
      line-height: 1.6;
      margin-bottom: 1rem;
    }

    .info-section ul {
      color: #666;
      padding-left: 1.5rem;
    }

    .info-section li {
      margin-bottom: 0.5rem;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 8px;
      font-size: 1rem;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.3s ease;
    }

    .btn-primary {
      background: #667eea;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background: #5a6fd8;
      transform: translateY(-1px);
    }

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
      transform: none;
    }

    .btn-secondary {
      background: #6c757d;
      color: white;
    }

    .btn-secondary:hover {
      background: #5a6268;
    }

    @media (max-width: 768px) {
      .dashboard-header {
        flex-direction: column;
        gap: 1rem;
        text-align: center;
      }

      .quick-actions {
        grid-template-columns: 1fr;
      }

      .dashboard-content {
        padding: 1rem;
      }
    }
  `]
})
export class DashboardComponent {
  authService = inject(AuthService);
  router = inject(Router);

  goToProjects() {
    this.router.navigate(['/projects']);
  }

  goToParameterSets() {
    this.router.navigate(['/parameter-sets']);
  }

  goToLanguages() {
    this.router.navigate(['/languages']);
  }
}