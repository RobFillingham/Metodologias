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
        <!-- Main Actions Grid -->
        <div class="main-actions-grid">
          <!-- COCOMO II Card -->
          <div class="action-card primary">
            <h2>COCOMO II</h2>
            <p>Accede a todas las herramientas de estimación y gestión de proyectos</p>
            <button class="btn btn-primary" (click)="goToCocomoII()">Comenzar</button>
          </div>

          <!-- Project Management Card -->
          <div class="action-card secondary">
            <h2>Gestión de Proyectos</h2>
            <p>Crea y administra tus proyectos para todas las metodologías de estimación</p>
            <button class="btn btn-secondary" (click)="goToProjects()">Gestionar</button>
          </div>
        </div>

        <!-- Estimation Methods Grid -->
        <div class="methods-section">
          <h2>Métodos de Estimación</h2>
          <div class="methods-grid">
            <!-- KLOC Card -->
            <div class="method-card">
              <div class="method-icon">💻</div>
              <h3>KLOC</h3>
              <p>Estimación basada en Líneas de Código</p>
              <button class="btn btn-outline" (click)="goToKloc()">Ir a KLOC →</button>
            </div>

            <!-- Function Point Card -->
            <div class="method-card">
              <div class="method-icon">📊</div>
              <h3>Puntos de Función</h3>
              <p>Estimación basada en componentes funcionales</p>
              <button class="btn btn-outline" (click)="goToFunctionPoint()">Ir a Function Point →</button>
            </div>

            <!-- Coming Soon Cards -->
            <div class="method-card disabled">
              <div class="method-icon">📝</div>
              <h3>Puntos de Caso de Uso</h3>
              <p>Próximamente</p>
            </div>
          </div>
        </div>

      </main>
    </div>
  `,
  styles: [`
    /* Welcome Header */
    .welcome-header {
      background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
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
      background: #f0f7ff;
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

    .main-actions-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
      gap: 2rem;
      margin-bottom: 3rem;
    }

    .action-card {
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
      text-align: center;
      max-width: 400px;
      width: 100%;
      transition: all 0.3s ease;
      border: 1px solid #e1e5e9;
    }

    .action-card.primary {
      background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
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
      box-shadow: 0 16px 48px rgba(37, 99, 235, 0.3);
    }

    .action-card.secondary {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      position: relative;
      overflow: hidden;
    }

    .action-card.secondary::before {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><circle cx="20" cy="20" r="2" fill="rgba(255,255,255,0.1)"/><circle cx="80" cy="80" r="2" fill="rgba(255,255,255,0.1)"/><circle cx="60" cy="30" r="1" fill="rgba(255,255,255,0.1)"/></svg>');
      opacity: 0.3;
    }

    .action-card.secondary:hover {
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
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
      max-width: 600px;
      width: 100%;
      border: 1px solid #e1e5e9;
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

    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 8px;
      font-size: 1rem;
      font-weight: 500;
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
      padding: 0.75rem 1.5rem;
      font-size: 1rem;
      font-weight: 500;
    }

    .btn-primary:hover {
      background: rgba(255, 255, 255, 0.3);
      transform: translateY(-1px);
      box-shadow: 0 8px 25px rgba(255, 255, 255, 0.2);
    }

    .btn-secondary {
      background: rgba(255, 255, 255, 0.2);
      color: white;
      border: 2px solid rgba(255, 255, 255, 0.3);
      backdrop-filter: blur(10px);
      padding: 0.75rem 1.5rem;
      font-size: 1rem;
      font-weight: 500;
    }

    .btn-secondary:hover {
      background: rgba(255, 255, 255, 0.3);
      transform: translateY(-1px);
      box-shadow: 0 8px 25px rgba(255, 255, 255, 0.2);
    }

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
      transform: none;
    }

    /* Methods Section */
    .methods-section {
      max-width: 1000px;
      margin: 3rem auto 0;
    }

    .methods-section h2 {
      text-align: center;
      color: #1e293b;
      font-size: 2rem;
      margin-bottom: 2rem;
      font-weight: 700;
    }

    .methods-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
      gap: 1.5rem;
    }

    .method-card {
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
      text-align: center;
      border: 2px solid #e1e5e9;
      transition: all 0.3s ease;
    }

    .method-card:not(.disabled):hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
      border-color: #3b82f6;
    }

    .method-card.disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .method-icon {
      font-size: 3rem;
      margin-bottom: 1rem;
    }

    .method-card h3 {
      margin: 0 0 0.75rem;
      color: #1e293b;
      font-size: 1.5rem;
      font-weight: 600;
    }

    .method-card p {
      margin: 0 0 1.5rem;
      color: #64748b;
      font-size: 1rem;
    }

    .btn-outline {
      background: transparent;
      color: #3b82f6;
      border: 2px solid #3b82f6;
      padding: 0.625rem 1.5rem;
      font-size: 1rem;
      font-weight: 500;
      cursor: pointer;
      border-radius: 8px;
      transition: all 0.3s ease;
    }

    .btn-outline:hover {
      background: #3b82f6;
      color: white;
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

      .methods-grid {
        grid-template-columns: 1fr;
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

  goToProjects() {
    this.router.navigate(['/projects']);
  }

  goToKloc() {
    this.router.navigate(['/kloc']);
  }

  goToFunctionPoint() {
    this.router.navigate(['/function-point']);
  }
}