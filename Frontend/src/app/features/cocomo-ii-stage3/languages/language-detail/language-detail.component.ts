import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { LanguageService } from '../../../../core/services/cocomo-ii-stage3/language.service';
import { Language, ApiResponse } from '../../../../core/models/cocomo-ii-stage3/cocomo-ii-stage3.models';
import { NavbarComponent } from '../../../../shared/components/navbar/navbar.component';

@Component({
  selector: 'app-language-detail',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  template: `
    <app-navbar></app-navbar>

    <div class="detail-container">
      <!-- Loading State -->
      <div *ngIf="loading()" class="loading-state">
        <div class="spinner"></div>
        <p>Cargando detalles del lenguaje...</p>
      </div>

      <!-- Error State -->
      <div *ngIf="error()" class="error-alert">
        <strong>⚠️ Error:</strong> {{ error() }}
        <button class="btn btn-sm btn-outline-primary ms-2" (click)="loadLanguage()">
          Reintentar
        </button>
      </div>

      <!-- Language Details -->
      <div *ngIf="!loading() && !error() && language()" class="language-detail">
        <!-- Header -->
        <div class="detail-header">
          <div class="header-content">
            <div class="language-icon">💻</div>
            <div>
              <h1>{{ language()!.name }}</h1>
              <p>Detalles del Lenguaje de Programación</p>
            </div>
          </div>
        </div>

        <!-- Main Stats -->
        <div class="detail-section">
          <h3>📊 Factor de Conversión SLOC</h3>
          <div class="main-stat-card">
            <div class="stat-large">
              <div class="stat-value-large">{{ language()!.slocPerUfp }}</div>
              <div class="stat-label-large">SLOC por UFP</div>
            </div>
            <div class="stat-description">
              <p>
                Este factor indica que por cada <strong>1 Punto de Función sin Ajustar</strong>,
                se requieren aproximadamente <strong>{{ language()!.slocPerUfp }} líneas de código {{ language()!.name }}</strong>
                para implementar la funcionalidad.
              </p>
            </div>
          </div>
        </div>

        <!-- Comparison -->
        <div class="detail-section">
          <h3>📈 Verbosidad Relativa</h3>
          <div class="verbosity-visual">
            <div class="verbosity-bar-large">
              <div 
                class="verbosity-fill-large" 
                [style.width.%]="getVerbosityPercentage(language()!.slocPerUfp)"
                [class.low]="language()!.slocPerUfp < 50"
                [class.medium]="language()!.slocPerUfp >= 50 && language()!.slocPerUfp < 100"
                [class.high]="language()!.slocPerUfp >= 100">
                <span class="percentage-label">{{ getVerbosityPercentage(language()!.slocPerUfp).toFixed(0) }}%</span>
              </div>
            </div>
            <p class="verbosity-description">
              {{ getVerbosityDescription(language()!.slocPerUfp) }}
            </p>
          </div>
        </div>

        <!-- Examples -->
        <div class="detail-section">
          <h3>💡 Ejemplos de Cálculos</h3>
          <p class="section-description">
            Aquí hay algunas conversiones de ejemplo de Puntos de Función a Líneas de Código Fuente para {{ language()!.name }}:
          </p>
          <div class="examples-grid">
            <div class="example-card">
              <div class="example-input">10 UFP</div>
              <div class="example-arrow">→</div>
              <div class="example-output">{{ (10 * language()!.slocPerUfp).toFixed(0) }} SLOC</div>
              <div class="example-label">Característica Pequeña</div>
            </div>
            <div class="example-card">
              <div class="example-input">50 UFP</div>
              <div class="example-arrow">→</div>
              <div class="example-output">{{ (50 * language()!.slocPerUfp).toFixed(0) }} SLOC</div>
              <div class="example-label">Módulo Mediano</div>
            </div>
            <div class="example-card">
              <div class="example-input">100 UFP</div>
              <div class="example-arrow">→</div>
              <div class="example-output">{{ (100 * language()!.slocPerUfp).toFixed(0) }} SLOC</div>
              <div class="example-label">Módulo Grande</div>
            </div>
            <div class="example-card">
              <div class="example-input">500 UFP</div>
              <div class="example-arrow">→</div>
              <div class="example-output">{{ (500 * language()!.slocPerUfp).toFixed(0) }} SLOC</div>
              <div class="example-label">Aplicación Completa</div>
            </div>
          </div>
        </div>

        <!-- Additional Info -->
        <div class="detail-section">
          <h3>ℹ️ Entendiendo SLOC por UFP</h3>
          <div class="info-cards">
            <div class="info-card">
              <div class="info-icon">📏</div>
              <h4>¿Qué es SLOC?</h4>
              <p>
                <strong>SLOC</strong> (Líneas de Código Fuente) es una métrica que cuenta el número de líneas
                en el código fuente de un programa, excluyendo comentarios y líneas en blanco.
              </p>
            </div>
            <div class="info-card">
              <div class="info-icon">🎯</div>
              <h4>¿Qué es UFP?</h4>
              <p>
                <strong>UFP</strong> (Puntos de Función sin Ajustar) mide el tamaño funcional del software
                basado en los requisitos del usuario, independientemente de la tecnología.
              </p>
            </div>
            <div class="info-card">
              <div class="info-icon">🔄</div>
              <h4>¿Por qué Convertir?</h4>
              <p>
                Convertir Puntos de Función a SLOC permite a COCOMO II estimar el esfuerzo y la duración
                basándose en tasas de productividad específicas del lenguaje.
              </p>
            </div>
          </div>
        </div>

        <!-- Back Button -->
        <div class="detail-actions">
          <button class="btn btn-secondary" (click)="goBack()">
            <i class="bi bi-arrow-left"></i>
            Volver a Lenguajes
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .detail-container {
      min-height: 100vh;
      background: #f8f9fa;
      padding: 2rem;
    }

    .loading-state {
      text-align: center;
      padding: 4rem 2rem;
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .spinner {
      width: 50px;
      height: 50px;
      margin: 0 auto 1rem;
      border: 4px solid #f3f3f3;
      border-top: 4px solid #667eea;
      border-radius: 50%;
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    .error-alert {
      background: #f8d7da;
      color: #721c24;
      padding: 1rem;
      border-radius: 8px;
      margin-bottom: 2rem;
      border: 1px solid #f5c6cb;
    }

    .language-detail {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      overflow: hidden;
    }

    .detail-header {
      padding: 2rem;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      border-bottom: 1px solid #e1e5e9;
    }

    .header-content {
      display: flex;
      align-items: center;
      gap: 1.5rem;
    }

    .language-icon {
      font-size: 4rem;
    }

    .header-content h1 {
      margin: 0 0 0.25rem 0;
      font-size: 2.5rem;
    }

    .header-content p {
      margin: 0;
      opacity: 0.9;
    }

    .detail-section {
      padding: 2rem;
      border-bottom: 1px solid #e1e5e9;
    }

    .detail-section:last-child {
      border-bottom: none;
    }

    .detail-section h3 {
      margin: 0 0 1.5rem 0;
      color: #333;
      font-size: 1.5rem;
    }

    .section-description {
      color: #666;
      margin-bottom: 1.5rem;
      line-height: 1.6;
    }

    .main-stat-card {
      background: #f8f9fa;
      border-radius: 12px;
      padding: 2rem;
      display: flex;
      align-items: center;
      gap: 2rem;
      border: 2px solid #667eea;
    }

    .stat-large {
      text-align: center;
      min-width: 200px;
    }

    .stat-value-large {
      font-size: 4rem;
      font-weight: bold;
      color: #667eea;
      line-height: 1;
    }

    .stat-label-large {
      color: #666;
      font-size: 1rem;
      margin-top: 0.5rem;
    }

    .stat-description {
      flex: 1;
    }

    .stat-description p {
      margin: 0;
      color: #555;
      line-height: 1.8;
      font-size: 1.1rem;
    }

    .verbosity-visual {
      text-align: center;
    }

    .verbosity-bar-large {
      width: 100%;
      height: 60px;
      background: #e9ecef;
      border-radius: 30px;
      overflow: hidden;
      position: relative;
      margin-bottom: 1rem;
    }

    .verbosity-fill-large {
      height: 100%;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: width 0.5s ease;
      position: relative;
    }

    .verbosity-fill-large.low {
      background: linear-gradient(90deg, #4caf50, #66bb6a);
    }

    .verbosity-fill-large.medium {
      background: linear-gradient(90deg, #ff9800, #ffa726);
    }

    .verbosity-fill-large.high {
      background: linear-gradient(90deg, #f44336, #ef5350);
    }

    .percentage-label {
      color: white;
      font-weight: bold;
      font-size: 1.2rem;
    }

    .verbosity-description {
      color: #666;
      font-size: 1.1rem;
      line-height: 1.6;
    }

    .examples-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
      gap: 1.5rem;
    }

    .example-card {
      background: #f8f9fa;
      padding: 1.5rem;
      border-radius: 8px;
      text-align: center;
      border: 1px solid #e1e5e9;
    }

    .example-input {
      font-size: 1.2rem;
      font-weight: 600;
      color: #667eea;
      margin-bottom: 0.5rem;
    }

    .example-arrow {
      font-size: 1.5rem;
      color: #999;
      margin: 0.5rem 0;
    }

    .example-output {
      font-size: 1.5rem;
      font-weight: bold;
      color: #333;
      margin-bottom: 0.5rem;
    }

    .example-label {
      color: #666;
      font-size: 0.9rem;
    }

    .info-cards {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1.5rem;
    }

    .info-card {
      background: #f8f9fa;
      padding: 1.5rem;
      border-radius: 8px;
      border: 1px solid #e1e5e9;
    }

    .info-icon {
      font-size: 2rem;
      margin-bottom: 0.5rem;
    }

    .info-card h4 {
      margin: 0 0 0.75rem 0;
      color: #333;
      font-size: 1.1rem;
    }

    .info-card p {
      margin: 0;
      color: #666;
      line-height: 1.6;
      font-size: 0.95rem;
    }

    .detail-actions {
      padding: 2rem;
      border-top: 1px solid #e1e5e9;
      text-align: center;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: 1px solid #dee2e6;
      border-radius: 6px;
      font-size: 1rem;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s;
      text-decoration: none;
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
    }

    .btn-secondary {
      background: #6c757d;
      color: white;
      border-color: #6c757d;
    }

    .btn-secondary:hover {
      background: #5a6268;
      border-color: #5a6268;
    }

    .btn-outline-primary {
      color: #667eea;
      border-color: #667eea;
      background: white;
    }

    .btn-outline-primary:hover {
      background: #667eea;
      color: white;
    }

    .btn-sm {
      padding: 0.375rem 0.75rem;
      font-size: 0.8rem;
    }

    @media (max-width: 768px) {
      .detail-container {
        padding: 1rem;
      }

      .header-content {
        flex-direction: column;
        text-align: center;
      }

      .main-stat-card {
        flex-direction: column;
      }

      .examples-grid {
        grid-template-columns: 1fr;
      }

      .info-cards {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class LanguageDetailComponent implements OnInit {
  private languageService = inject(LanguageService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  language = signal<Language | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);
  languageId = signal<number | null>(null);

  ngOnInit() {
    const id = +this.route.snapshot.params['id'];
    this.languageId.set(id);
    this.loadLanguage(id);
  }

  loadLanguage(id?: number) {
    const langId = id || this.languageId();
    if (!langId) return;

    this.loading.set(true);
    this.error.set(null);

    this.languageService.getLanguage(langId).subscribe({
      next: (response: ApiResponse<Language>) => {
        this.loading.set(false);
        if (response.success && response.data) {
          this.language.set(response.data);
        } else {
          this.error.set(response.errors?.[0] || 'Failed to load language');
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set('Failed to load language. Please try again.');
        console.error('Error loading language:', err);
      }
    });
  }

  getVerbosityPercentage(slocPerUfp: number): number {
    const maxSloc = 320;
    return Math.min((slocPerUfp / maxSloc) * 100, 100);
  }

  getVerbosityDescription(slocPerUfp: number): string {
    if (slocPerUfp < 50) {
      return `${this.language()!.name} es un lenguaje relativamente conciso, que requiere menos líneas de código para implementar la misma funcionalidad en comparación con lenguajes más verbosos. Esto puede conducir a un desarrollo más rápido y un mantenimiento más fácil.`;
    } else if (slocPerUfp < 100) {
      return `${this.language()!.name} tiene un nivel de verbosidad moderado. Equilibra la expresividad con la concisión del código, haciéndolo adecuado para una amplia gama de aplicaciones.`;
    } else {
      return `${this.language()!.name} es más verboso, requiriendo más líneas de código para implementar funcionalidad. Esto puede proporcionar más control explícito y legibilidad, pero puede aumentar el tiempo de desarrollo.`;
    }
  }

  goBack() {
    this.router.navigate(['/cocomo-ii-stage3/languages']);
  }
}