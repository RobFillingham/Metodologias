import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, NavbarComponent],
  template: `
    <app-navbar></app-navbar>

    <div class="home-container">
      <!-- Hero Section -->
      <section class="hero">
        <div class="hero-content">
          <h1>Herramienta de Estimación de Proyectos de Software</h1>
          <p class="hero-description">
            Proyecto desarrollado bajo la metodología OpenUP/TSP que implementa
            las principales técnicas de estimación de software utilizadas en la industria.
          </p>

          <div class="project-info">
            <h2>Sobre el Proyecto</h2>
            <p>
              Esta herramienta forma parte de un proyecto académico que demuestra la aplicación
              práctica de procesos de desarrollo de software siguiendo las mejores prácticas
              de la industria. Implementa metodologías OpenUP (Open Unified Process) y TSP
              (Team Software Process) para el desarrollo colaborativo y gestión de calidad.
            </p>

            <h3>Técnicas de Estimación Implementadas</h3>
            <div class="estimation-methods">
              <div class="method-item">
                <span>KLOC (Miles de Líneas de Código)</span>
              </div>
              <div class="method-item">
                <span>Puntos de Función</span>
              </div>
              <div class="method-item">
                <span>Puntos de Caso de Uso</span>
              </div>
              <div class="method-item">
                <span>COCOMO II Stage 1, 2 y 3</span>
              </div>
            </div>
          </div>
        </div>
      </section>
    </div>
  `,
  styles: [`
    .home-container {
      min-height: 100vh;
      background: #f8fafc;
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
    }

    .hero {
      background: linear-gradient(135deg, #ffffff 0%, #e0f2fe 100%);
      padding: 4rem 2rem 2rem;
      text-align: center;
    }

    .hero-content {
      max-width: 800px;
      margin: 0 auto;
    }

    h1 {
      font-size: 2.5rem;
      color: #1e40af;
      margin-bottom: 1.5rem;
      font-weight: 700;
      line-height: 1.2;
    }

    .hero-description {
      font-size: 1.2rem;
      color: #475569;
      line-height: 1.6;
      margin-bottom: 3rem;
    }

    .project-info {
      text-align: left;
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
      margin-bottom: 3rem;
    }

    .project-info h2 {
      color: #1e40af;
      font-size: 1.8rem;
      margin-bottom: 1rem;
      font-weight: 600;
    }

    .project-info p {
      color: #64748b;
      line-height: 1.6;
      margin-bottom: 2rem;
    }

    .project-info h3 {
      color: #1e40af;
      font-size: 1.4rem;
      margin-bottom: 1.5rem;
      font-weight: 600;
    }

    .estimation-methods {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1rem;
    }

    .method-item {
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 0.75rem;
      background: #f1f5f9;
      border-radius: 8px;
      color: #475569;
      font-weight: 500;
      text-align: center;
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
      text-align: center;
    }

    .btn-primary {
      background: #2563eb;
      color: white;
    }

    .btn-primary:hover {
      background: #1d4ed8;
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(37, 99, 235, 0.3);
    }

    .btn-secondary {
      background: white;
      color: #2563eb;
      border: 2px solid #2563eb;
    }

    .btn-secondary:hover {
      background: #2563eb;
      color: white;
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(37, 99, 235, 0.3);
    }

    /* Responsive Design */
    @media (max-width: 768px) {
      .hero {
        padding: 3rem 1rem 1rem;
      }

      h1 {
        font-size: 2rem;
      }

      .hero-description {
        font-size: 1.1rem;
      }

      .project-info {
        padding: 1.5rem;
      }

      .estimation-methods {
        grid-template-columns: 1fr;
      }

      .method-item {
        justify-content: center;
      }

      .btn {
        width: 100%;
        max-width: 200px;
      }
    }

    @media (max-width: 480px) {
      h1 {
        font-size: 1.75rem;
      }

      .project-info h2 {
        font-size: 1.5rem;
      }

      .project-info h3 {
        font-size: 1.2rem;
      }
    }
  `]
})
export class HomeComponent {
  authService = inject(AuthService);
  private router = inject(Router);

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/home']);
  }
}
