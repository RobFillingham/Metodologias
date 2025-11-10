import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';

@Component({
  selector: 'app-cocomo-ii-stage3-landing',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  template: `
    <app-navbar></app-navbar>

    <div class="cocomo-ii-stage3-container">
      <main class="cocomo-ii-stage3-content">
        <button class="btn-back" (click)="goBack()" title="Regresar">
          ← Regresar
        </button>

        <div class="hero-section">
          <h1>🔬 COCOMO II Stage 3</h1>
          <p>Constructive Cost Model II Stage 3 - Herramientas de estimación de software</p>
        </div>

        <div class="tools-grid">
          <div class="tool-card">
            <div class="tool-icon">📁</div>
            <h3>Proyectos</h3>
            <p>Crea y gestiona tus proyectos de estimación COCOMO II Stage 3</p>
            <button class="btn btn-primary" (click)="goToProjects()">Gestionar Proyectos</button>
          </div>

          <div class="tool-card">
            <div class="tool-icon">⚙️</div>
            <h3>Conjuntos de Parámetros</h3>
            <p>Personaliza los parámetros de estimación para tu organización</p>
            <button class="btn btn-primary" (click)="goToParameterSets()">Gestionar Parámetros</button>
          </div>

          <div class="tool-card">
            <div class="tool-icon">💻</div>
            <h3>Lenguajes</h3>
            <p>Administra los lenguajes de programación y sus factores</p>
            <button class="btn btn-primary" (click)="goToLanguages()">Gestionar Lenguajes</button>
          </div>
        </div>
      </main>
    </div>
  `,
  styles: [`
    .cocomo-ii-stage3-container {
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 2rem 0;
    }

    .cocomo-ii-stage3-content {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 2rem;
    }

    .btn-back {
      background: transparent;
      border: none;
      color: white;
      font-weight: 600;
      cursor: pointer;
      padding: 0.5rem 0;
      margin-bottom: 1rem;
      transition: all 0.2s ease;
      font-size: 1rem;
    }

    .btn-back:hover {
      opacity: 0.8;
      transform: translateX(-2px);
    }

    .hero-section {
      text-align: center;
      margin-bottom: 4rem;
      color: white;
    }

    .hero-section h1 {
      font-size: 3rem;
      margin-bottom: 1rem;
      text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
    }

    .hero-section p {
      font-size: 1.2rem;
      opacity: 0.9;
      margin: 0;
    }

    .tools-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 2rem;
    }

    .tool-card {
      background: white;
      border-radius: 16px;
      padding: 2rem;
      text-align: center;
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
      transition: all 0.3s ease;
    }

    .tool-card:hover {
      transform: translateY(-8px);
      box-shadow: 0 20px 40px rgba(0, 0, 0, 0.3);
    }

    .tool-icon {
      font-size: 3rem;
      margin-bottom: 1rem;
    }

    .tool-card h3 {
      color: #333;
      margin: 0 0 1rem;
      font-size: 1.5rem;
    }

    .tool-card p {
      color: #666;
      margin: 0 0 2rem;
      line-height: 1.6;
    }

    .btn {
      padding: 0.75rem 2rem;
      border: none;
      border-radius: 8px;
      font-size: 1rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.3s ease;
      text-decoration: none;
      display: inline-block;
    }

    .btn-primary {
      background: #667eea;
      color: white;
    }

    .btn-primary:hover {
      background: #5a6fd8;
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
    }

    @media (max-width: 768px) {
      .cocomo-ii-stage3-container {
        padding: 1rem 0;
      }

      .cocomo-ii-stage3-content {
        padding: 0 1rem;
      }

      .hero-section h1 {
        font-size: 2rem;
      }

      .tools-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class CocomoIIStage3LandingComponent {
  router = inject(Router);

  goToProjects() {
    this.router.navigate(['/projects'], { queryParams: { variant: 'cocomo-ii-stage3' } });
  }

  goToParameterSets() {
    this.router.navigate(['/cocomo-ii-stage3/parameter-sets']);
  }

  goToLanguages() {
    this.router.navigate(['/cocomo-ii-stage3/languages']);
  }

  goBack() {
    this.router.navigate(['/dashboard']);
  }
}