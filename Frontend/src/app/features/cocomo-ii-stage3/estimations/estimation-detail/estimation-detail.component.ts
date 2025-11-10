import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { NavbarComponent } from '../../../../shared/components/navbar/navbar.component';
import { FunctionPointEntryStage3Component } from '../../components/function-point-entry/function-point-entry.component';
import { ParameterEditorComponent } from '../../components/parameter-editor/parameter-editor.component';
import { EstimationService } from '../../../../core/services/cocomo-ii-stage3/estimation.service';
import { Estimation, ApiResponse } from '../../../../core/models/cocomo-ii-stage3/cocomo-ii-stage3.models';

@Component({
  selector: 'app-estimation-detail',
  standalone: true,
  imports: [CommonModule, NavbarComponent, FunctionPointEntryStage3Component, ParameterEditorComponent],
  template: `
    <app-navbar></app-navbar>

    <div class="estimation-detail-container">
      <!-- Loading State -->
      <div *ngIf="loading()" class="loading-container">
        <div class="spinner"></div>
        <p>Cargando estimación...</p>
      </div>

      <!-- Error State -->
      <div *ngIf="error() && !loading()" class="error-container">
        <div class="error-icon">⚠️</div>
        <h3>Error al Cargar Estimación</h3>
        <p>{{ error() }}</p>
        <button class="btn btn-primary" (click)="loadEstimation()">Reintentar</button>
      </div>

      <!-- Estimation Content -->
      <div *ngIf="estimation() && !loading()">
        <!-- Header -->
        <div class="estimation-header">
          <div>
            <button class="btn-back" (click)="goBack()">← Volver a Estimaciones</button>
            <h1>{{ estimation()!.estimationName }}</h1>
            <p class="meta">Creado {{ formatDate(estimation()!.createdAt) }}</p>
          </div>
        </div>

        <!-- Results Summary (if calculated) -->
        <div *ngIf="estimation()!.totalUfp" class="results-summary">
          <h2>📊 Resultados del Cálculo</h2>
          <div class="results-grid">
            <div class="result-card">
              <div class="result-label">Total UFP</div>
              <div class="result-value">{{ estimation()!.totalUfp || 0 }}</div>
            </div>
            <div class="result-card">
              <div class="result-label">SLOC</div>
              <div class="result-value">{{ formatNumber(estimation()!.sloc) }}</div>
            </div>
            <div class="result-card">
              <div class="result-label">Esfuerzo (PM)</div>
              <div class="result-value">{{ formatNumber(estimation()!.effortPm) }}</div>
            </div>
            <div class="result-card">
              <div class="result-label">Duración (Meses)</div>
              <div class="result-value">{{ formatNumber(estimation()!.tdevMonths) }}</div>
            </div>
            <div class="result-card">
              <div class="result-label">Tamaño de Equipo</div>
              <div class="result-value">{{ formatNumber(estimation()!.avgTeamSize) }}</div>
            </div>
          </div>
        </div>

        <!-- Parameter Editor -->
        <div class="content-section">
          <app-parameter-editor
            [estimation]="estimation()!"
            (onSaved)="onParametersUpdated()"
          ></app-parameter-editor>
        </div>

        <!-- Function Points Entry -->
        <app-function-point-entry-stage3
          [estimationId]="estimationId"
          (functionsUpdated)="onFunctionsUpdated()"
        ></app-function-point-entry-stage3>
      </div>
    </div>
  `,
  styles: [`
    .estimation-detail-container {
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 2rem;
    }

    .loading-container,
    .error-container {
      max-width: 800px;
      margin: 4rem auto;
      text-align: center;
      background: white;
      padding: 4rem 2rem;
      border-radius: 16px;
      box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
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

    .error-icon {
      font-size: 4rem;
      margin-bottom: 1rem;
    }

    .error-container h3 {
      color: #e53e3e;
      margin: 0 0 1rem;
    }

    .error-container p {
      color: #666;
      margin-bottom: 2rem;
    }

    .estimation-header {
      max-width: 1200px;
      margin: 0 auto 2rem;
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .btn-back {
      background: transparent;
      border: none;
      color: #667eea;
      font-size: 1rem;
      cursor: pointer;
      margin-bottom: 1rem;
      padding: 0.5rem 0;
      transition: all 0.2s ease;
    }

    .btn-back:hover {
      color: #5568d3;
      transform: translateX(-4px);
    }

    .estimation-header h1 {
      margin: 0;
      color: #333;
      font-size: 2rem;
    }

    .meta {
      margin: 0.5rem 0 0;
      color: #666;
      font-size: 0.95rem;
    }

    .results-summary {
      max-width: 1200px;
      margin: 0 auto 2rem;
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .results-summary h2 {
      margin: 0 0 1.5rem;
      color: #333;
    }

    .results-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1.5rem;
    }

    .result-card {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 1.5rem;
      border-radius: 12px;
      color: white;
      text-align: center;
    }

    .result-label {
      font-size: 0.9rem;
      opacity: 0.9;
      margin-bottom: 0.5rem;
    }

    .result-value {
      font-size: 2rem;
      font-weight: 700;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 8px;
      font-size: 1rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.3s ease;
    }

    .btn-primary {
      background: #667eea;
      color: white;
    }

    .btn-primary:hover {
      background: #5568d3;
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
    }

    .content-section {
      max-width: 1200px;
      margin: 0 auto 2rem;
    }

    @media (max-width: 768px) {
      .estimation-detail-container {
        padding: 1rem;
      }

      .results-grid {
        grid-template-columns: repeat(2, 1fr);
      }
    }
  `]
})
export class EstimationDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private estimationService = inject(EstimationService);

  estimationId!: number;
  projectId!: number;

  estimation = signal<Estimation | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.projectId = +params['projectId'];
      this.estimationId = +params['estimationId'];
      this.loadEstimation();
    });
  }

  loadEstimation() {
    this.loading.set(true);
    this.error.set(null);

    this.estimationService.getEstimation(this.projectId, this.estimationId).subscribe({
      next: (response: ApiResponse<Estimation>) => {
        if (response.success && response.data) {
          this.estimation.set(response.data);
        } else {
          this.error.set(response.message || 'Error al cargar la estimación');
        }
        this.loading.set(false);
      },
      error: (err: any) => {
        this.error.set(err.message || 'Ocurrió un error al cargar la estimación');
        this.loading.set(false);
      }
    });
  }

  onFunctionsUpdated() {
    // Reload estimation to get updated calculations
    this.loadEstimation();
  }

  onParametersUpdated() {
    // Reload estimation to get updated calculations after rating changes
    this.loadEstimation();
  }

  goBack() {
    this.router.navigate(['/cocomo-ii-stage3/estimations', this.projectId]);
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-MX', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  formatNumber(value?: number): string {
    if (!value) return '-';
    return value.toFixed(2);
  }
}
