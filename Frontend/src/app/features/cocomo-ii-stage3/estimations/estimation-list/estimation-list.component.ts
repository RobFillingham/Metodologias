import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { NavbarComponent } from '../../../../shared/components/navbar/navbar.component';
import { EstimationFormComponent } from '../estimation-form/estimation-form.component';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { EstimationService } from '../../../../core/services/cocomo-ii-stage3/estimation.service';
import { ProjectService } from '../../../../core/services/cocomo-ii-stage3/project.service';
import { Estimation, Project, ApiResponse } from '../../../../core/models/cocomo-ii-stage3/cocomo-ii-stage3.models';

@Component({
  selector: 'app-estimation-list',
  standalone: true,
  imports: [CommonModule, NavbarComponent, EstimationFormComponent, ConfirmDialogComponent],
  template: `
    <app-navbar></app-navbar>

    <div class="estimation-list-container">
      <!-- Header -->
      <div class="header">
        <div>
          <button class="btn-back" (click)="goBack()">← Volver a Proyectos</button>
          <h1>{{ project()?.projectName }} - Estimaciones</h1>
          <p class="meta" *ngIf="project()">{{ project()!.description }}</p>
        </div>
        <button class="btn btn-primary" (click)="createEstimation()">
          + Nueva Estimación
        </button>
      </div>

      <!-- Loading State -->
      <div *ngIf="loading()" class="loading-container">
        <div class="spinner"></div>
        <p>Cargando estimaciones...</p>
      </div>

      <!-- Error State -->
      <div *ngIf="error() && !loading()" class="error-container">
        <div class="error-icon">⚠️</div>
        <h3>Error al Cargar Estimaciones</h3>
        <p>{{ error() }}</p>
        <button class="btn btn-primary" (click)="loadEstimations()">Reintentar</button>
      </div>

      <!-- Empty State -->
      <div *ngIf="!loading() && !error() && estimations().length === 0" class="empty-container">
        <div class="empty-icon">📊</div>
        <h3>No Hay Estimaciones Aún</h3>
        <p>Crea tu primera estimación COCOMO II para este proyecto</p>
        <button class="btn btn-primary" (click)="createEstimation()">Crear Estimación</button>
      </div>

      <!-- Estimations Grid -->
      <div *ngIf="!loading() && !error() && estimations().length > 0" class="estimations-grid">
        <div *ngFor="let estimation of estimations()" class="estimation-card">
          <div class="card-header">
            <h3>{{ estimation.estimationName }}</h3>
            <span class="status-badge" [class.calculated]="estimation.totalUfp">
              {{ estimation.totalUfp ? '✓ Calculado' : '⏳ Pendiente' }}
            </span>
          </div>

          <div class="card-stats">
            <div class="stat" *ngIf="estimation.totalUfp">
              <div class="stat-label">Total UFP</div>
              <div class="stat-value">{{ estimation.totalUfp }}</div>
            </div>
            <div class="stat" *ngIf="estimation.effortPm">
              <div class="stat-label">Esfuerzo (PM)</div>
              <div class="stat-value">{{ formatNumber(estimation.effortPm) }}</div>
            </div>
            <div class="stat" *ngIf="estimation.tdevMonths">
              <div class="stat-label">Duración</div>
              <div class="stat-value">{{ formatNumber(estimation.tdevMonths) }}m</div>
            </div>
            <div class="stat" *ngIf="estimation.avgTeamSize">
              <div class="stat-label">Tamaño de Equipo</div>
              <div class="stat-value">{{ formatNumber(estimation.avgTeamSize) }}</div>
            </div>
          </div>

          <div class="card-footer">
            <span class="date">Creado {{ formatDate(estimation.createdAt) }}</span>
            <div class="card-actions">
              <button class="btn btn-icon btn-danger" (click)="confirmDelete(estimation)" title="Eliminar estimación">
                🗑️
              </button>
              <button class="btn btn-secondary" (click)="viewEstimation(estimation)">
                Ver Detalles →
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Estimation Modal -->
    <app-estimation-form
      *ngIf="showCreateModal()"
      [projectId]="projectId"
      (formSubmitted)="onEstimationCreated()"
      (formCancelled)="closeCreateModal()"
    ></app-estimation-form>

    <!-- Delete Confirmation Dialog -->
    <app-confirm-dialog
      [show]="showDeleteDialog()"
      title="Eliminar Estimación"
      message="¿Estás seguro de que quieres eliminar esta estimación? Esta acción no se puede deshacer."
      confirmText="Eliminar"
      cancelText="Cancelar"
      [loading]="deleting()"
      (confirm)="deleteEstimation()"
      (cancel)="cancelDelete()"
    ></app-confirm-dialog>
  `,
  styles: [`
    .estimation-list-container {
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 2rem;
    }

    .header {
      max-width: 1200px;
      margin: 0 auto 2rem;
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      gap: 2rem;
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

    .header h1 {
      margin: 0;
      color: #333;
      font-size: 2rem;
    }

    .meta {
      margin: 0.5rem 0 0;
      color: #666;
      font-size: 0.95rem;
    }

    .loading-container,
    .error-container,
    .empty-container {
      max-width: 800px;
      margin: 2rem auto;
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

    .empty-icon,
    .error-icon {
      font-size: 4rem;
      margin-bottom: 1rem;
    }

    .empty-container h3,
    .error-container h3 {
      margin: 0 0 1rem;
      color: #333;
    }

    .error-container h3 {
      color: #e53e3e;
    }

    .empty-container p,
    .error-container p {
      color: #666;
      margin-bottom: 2rem;
    }

    .estimations-grid {
      max-width: 1200px;
      margin: 0 auto;
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
      gap: 2rem;
    }

    .estimation-card {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      overflow: hidden;
      transition: all 0.3s ease;
    }

    .estimation-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
    }

    .card-header {
      padding: 1.5rem;
      border-bottom: 1px solid #eee;
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      gap: 1rem;
    }

    .card-header h3 {
      margin: 0;
      color: #333;
      font-size: 1.25rem;
      flex: 1;
    }

    .status-badge {
      padding: 0.25rem 0.75rem;
      border-radius: 12px;
      font-size: 0.85rem;
      font-weight: 600;
      background: #e2e8f0;
      color: #64748b;
      white-space: nowrap;
    }

    .status-badge.calculated {
      background: #d1fae5;
      color: #065f46;
    }

    .card-stats {
      padding: 1.5rem;
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 1rem;
    }

    .stat {
      text-align: center;
    }

    .stat-label {
      font-size: 0.85rem;
      color: #666;
      margin-bottom: 0.25rem;
    }

    .stat-value {
      font-size: 1.5rem;
      font-weight: 700;
      color: #667eea;
    }

    .card-footer {
      padding: 1.5rem;
      border-top: 1px solid #eee;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .card-actions {
      display: flex;
      gap: 0.5rem;
      align-items: center;
    }

    .date {
      font-size: 0.9rem;
      color: #666;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 8px;
      font-size: 1rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.3s ease;
      white-space: nowrap;
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

    .btn-secondary {
      background: transparent;
      color: #667eea;
      border: 2px solid #667eea;
    }

    .btn-secondary:hover {
      background: #667eea;
      color: white;
    }

    .btn-icon {
      padding: 0.5rem;
      border: none;
      background: transparent;
      color: #dc3545;
      font-size: 1.2rem;
      cursor: pointer;
      border-radius: 6px;
      transition: all 0.2s ease;
    }

    .btn-icon:hover {
      background: #f8d7da;
      transform: scale(1.1);
    }

    @media (max-width: 768px) {
      .estimation-list-container {
        padding: 1rem;
      }

      .header {
        flex-direction: column;
        align-items: stretch;
      }

      .estimations-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class EstimationListComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private estimationService = inject(EstimationService);
  private projectService = inject(ProjectService);

  projectId!: number;
  project = signal<Project | null>(null);
  estimations = signal<Estimation[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  showCreateModal = signal(false);
  showDeleteDialog = signal(false);
  deleting = signal(false);
  estimationToDelete = signal<Estimation | null>(null);

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.projectId = +params['projectId'];
      this.loadProject();
      this.loadEstimations();
    });
  }

  loadProject() {
    // Try to get from service first
    const currentProject = this.projectService.currentProjectValue;
    if (currentProject && currentProject.projectId === this.projectId) {
      this.project.set(currentProject);
    } else {
      // Load from API
      this.projectService.getProject(this.projectId).subscribe({
        next: (response: ApiResponse<Project>) => {
          if (response.success && response.data) {
            this.project.set(response.data);
          }
        },
        error: () => {
          // Silent fail, project name won't show but estimations will still load
        }
      });
    }
  }

  loadEstimations() {
    this.loading.set(true);
    this.error.set(null);

    this.estimationService.getEstimationsByProject(this.projectId).subscribe({
      next: (response: ApiResponse<Estimation[]>) => {
        if (response.success && response.data) {
          this.estimations.set(response.data);
        } else {
          this.error.set(response.message || 'Error al cargar las estimaciones');
        }
        this.loading.set(false);
      },
      error: (err: any) => {
        this.error.set(err.message || 'Ocurrió un error al cargar las estimaciones');
        this.loading.set(false);
      }
    });
  }

  createEstimation() {
    this.showCreateModal.set(true);
  }

  closeCreateModal() {
    this.showCreateModal.set(false);
  }

  onEstimationCreated() {
    this.closeCreateModal();
    this.loadEstimations(); // Refresh the list
  }

  viewEstimation(estimation: Estimation) {
    this.router.navigate(['/cocomo-ii-stage3/estimations', this.projectId, estimation.estimationId]);
  }

  confirmDelete(estimation: Estimation) {
    this.estimationToDelete.set(estimation);
    this.showDeleteDialog.set(true);
  }

  cancelDelete() {
    this.showDeleteDialog.set(false);
    this.estimationToDelete.set(null);
  }

  deleteEstimation() {
    const estimation = this.estimationToDelete();
    if (!estimation) return;

    this.deleting.set(true);
    this.estimationService.deleteEstimation(this.projectId, estimation.estimationId).subscribe({
      next: (response) => {
        if (response.success) {
          // Remove from local list
          const currentEstimations = this.estimations();
          this.estimations.set(currentEstimations.filter(e => e.estimationId !== estimation.estimationId));
        } else {
          this.error.set(response.message || 'Error al eliminar la estimación');
        }
        this.deleting.set(false);
        this.showDeleteDialog.set(false);
        this.estimationToDelete.set(null);
      },
      error: (err) => {
        this.error.set(err.message || 'Ocurrió un error al eliminar la estimación');
        this.deleting.set(false);
        this.showDeleteDialog.set(false);
        this.estimationToDelete.set(null);
      }
    });
  }

  goBack() {
    this.router.navigate(['/projects'], { queryParams: { variant: 'cocomo-ii-stage3' } });
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-MX', {
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });
  }

  formatNumber(value?: number): string {
    if (!value) return '0';
    return value.toFixed(2);
  }
}
