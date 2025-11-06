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

    <!-- Welcome Header -->
    <div class="welcome-header">
      <div class="welcome-content">
        <h1>¡Bienvenido a tu Centro de Estimaciones!</h1>
        <p>Plataforma completa para estimaciones de software usando COCOMO II</p>
      </div>
    </div>

    <div class="dashboard-container">
      <main class="dashboard-content">
        <!-- Main Action -->
        <div class="main-action">
          <div class="action-card primary">
            <div class="card-icon">🔬</div>
            <h2>COCOMO II</h2>
            <p>Accede a todas las herramientas de estimación y gestión de proyectos</p>
            <button class="btn btn-primary" (click)="goToCocomoII()">Comenzar</button>
          </div>
        </div>

      </main>
    </div>
  `,
  styles: [`
    /* Welcome Header */
    .welcome-header {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 3rem 2rem;
      text-align: center;
    }

    .welcome-content h1 {
      font-size: 2.5rem;
      margin: 0 0 1rem;
      font-weight: 700;
      text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
    }

    .welcome-content p {
      font-size: 1.2rem;
      margin: 0;
      opacity: 0.9;
    }

    /* Dashboard Container */
    .dashboard-container {
      background: #f8f9fa;
      min-height: calc(100vh - 200px);
      padding: 3rem 0;
    }

    .dashboard-content {
      max-width: 1000px;
      margin: 0 auto;
      padding: 0 2rem;
    }

    /* Main Action */
    .main-action {
      display: flex;
      justify-content: center;
      margin-bottom: 3rem;
    }

    .action-card {
      background: white;
      padding: 3rem;
      border-radius: 16px;
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
      text-align: center;
      max-width: 500px;
      width: 100%;
      transition: all 0.3s ease;
    }

    .action-card.primary {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      position: relative;
      overflow: hidden;
    }

    .action-card.primary::before {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><circle cx="20" cy="20" r="2" fill="rgba(255,255,255,0.1)"/><circle cx="80" cy="80" r="2" fill="rgba(255,255,255,0.1)"/><circle cx="60" cy="30" r="1" fill="rgba(255,255,255,0.1)"/></svg>');
      opacity: 0.3;
    }

    .action-card.primary:hover {
      transform: translateY(-4px);
      box-shadow: 0 16px 48px rgba(102, 126, 234, 0.3);
    }

    .card-icon {
      font-size: 4rem;
      margin-bottom: 1.5rem;
      display: block;
    }

    .action-card h2 {
      margin: 0 0 1rem;
      font-size: 2rem;
      font-weight: 600;
    }

    .action-card p {
      margin: 0 0 2rem;
      line-height: 1.6;
      font-size: 1.1rem;
      opacity: 0.9;
    }

    /* Info Section */
    .info-section {
      display: flex;
      justify-content: center;
    }

    .info-card {
      background: white;
      padding: 2.5rem;
      border-radius: 12px;
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
      max-width: 600px;
      width: 100%;
    }

    .info-card h3 {
      margin: 0 0 1.5rem;
      color: #333;
      font-size: 1.5rem;
      font-weight: 600;
    }

    .info-card p {
      color: #666;
      line-height: 1.7;
      margin: 0;
      font-size: 1rem;
    }

    /* Buttons */
    .btn {
      padding: 1rem 2.5rem;
      border: none;
      border-radius: 8px;
      font-size: 1.1rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.3s ease;
      text-decoration: none;
      display: inline-block;
      position: relative;
      z-index: 1;
    }

    .btn-primary {
      background: rgba(255, 255, 255, 0.2);
      color: white;
      border: 2px solid rgba(255, 255, 255, 0.3);
      backdrop-filter: blur(10px);
    }

    .btn-primary:hover {
      background: rgba(255, 255, 255, 0.3);
      transform: translateY(-2px);
      box-shadow: 0 8px 25px rgba(255, 255, 255, 0.2);
    }

    /* Responsive Design */
    @media (max-width: 768px) {
      .welcome-header {
        padding: 2rem 1rem;
      }

      .welcome-content h1 {
        font-size: 2rem;
      }

      .welcome-content p {
        font-size: 1rem;
      }

      .dashboard-content {
        padding: 0 1rem;
      }

      .action-card {
        padding: 2rem;
      }

      .card-icon {
        font-size: 3rem;
      }

      .action-card h2 {
        font-size: 1.5rem;
      }

      .info-card {
        padding: 2rem;
      }
    }

    @media (max-width: 480px) {
      .welcome-content h1 {
        font-size: 1.8rem;
      }

      .action-card {
        padding: 1.5rem;
      }

      .info-card {
        padding: 1.5rem;
      }
    }
  `]
})
export class DashboardComponent {
  authService = inject(AuthService);
  router = inject(Router);

  goToCocomoII() {
    this.router.navigate(['/cocomo2']);
  }
}