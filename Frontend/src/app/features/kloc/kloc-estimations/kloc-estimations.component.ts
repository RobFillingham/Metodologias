import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SelectProjectDialogComponent } from '../../../shared/components/select-project-dialog/select-project-dialog.component';
import { KlocEstimationDialogComponent } from '../kloc-estimation-dialog/kloc-estimation-dialog.component';
import { KlocEstimationService } from '../../../core/services/kloc/kloc-estimation.service';
import { ProjectService } from '../../../core/services/cocomo2/project.service';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { 
  KlocEstimation, 
  Project, 
  CreateKlocEstimationRequest, 
  UpdateKlocEstimationRequest 
} from '../../../core/models/kloc/kloc.models';

@Component({
  selector: 'app-kloc-estimations',
  standalone: true,
  imports: [
    CommonModule, 
    NavbarComponent, 
    SelectProjectDialogComponent,
    KlocEstimationDialogComponent
  ],
  template: `
    <app-navbar></app-navbar>

    <div class="container">
      <div class="header">
        <div class="header-content">
          <button class="btn-back" (click)="goBack()" title="Regresar">
            ← Regresar
          </button>
          <h1>Estimaciones KLOC</h1>
          <p class="subtitle">Gestión de estimaciones basadas en Líneas de Código</p>
        </div>
        <button class="btn btn-primary" (click)="openCreateFlow()">
          + Nueva Estimación
        </button>
      </div>

      <!-- Loading State -->
      <div *ngIf="loading()" class="loading-container">
        <div class="spinner"></div>
        <p>Cargando estimaciones...</p>
      </div>

      <!-- Error State -->
      <div *ngIf="error()" class="error-container">
        <div class="error-icon">!</div>
        <h3>Error al cargar estimaciones</h3>
        <p>{{ error() }}</p>
        <button class="btn btn-primary" (click)="loadAllEstimations()">Intentar nuevamente</button>
      </div>

      <!-- Empty State -->
      <div *ngIf="!loading() && !error() && estimations().length === 0" class="empty-state">
        <div class="empty-icon">📊</div>
        <h2>Sin estimaciones KLOC</h2>
        <p>Crea tu primera estimación para comenzar a gestionar tus proyectos.</p>
        <button class="btn btn-primary btn-lg" (click)="openCreateFlow()">
          Crear Primera Estimación
        </button>
      </div>

      <!-- Estimations Table -->
      <div *ngIf="!loading() && !error() && estimations().length > 0" class="table-container">
        <table class="estimations-table">
          <thead>
            <tr>
              <th>Nombre</th>
              <th>Proyecto</th>
              <th>LOC</th>
              <th>KLOC</th>
              <th>Esfuerzo (PM)</th>
              <th>Tiempo (Meses)</th>
              <th>Costo</th>
              <th>Fecha Creación</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let estimation of estimations()" class="estimation-row">
              <td>
                <strong>{{ estimation.estimationName }}</strong>
                <span *ngIf="estimation.notes" class="notes-indicator" [title]="estimation.notes">📝</span>
              </td>
              <td>
                <span class="project-badge">{{ getProjectName(estimation.projectId) }}</span>
              </td>
              <td>{{ formatNumber(estimation.linesOfCode) }}</td>
              <td>{{ formatKloc(estimation.linesOfCode) }}</td>
              <td>{{ formatDecimal(estimation.estimatedEffort) }}</td>
              <td>{{ formatDecimal(estimation.estimatedTime) }}</td>
              <td>{{ formatCurrency(estimation.estimatedCost) }}</td>
              <td>{{ formatDate(estimation.createdAt) }}</td>
              <td class="actions">
                <button 
                  class="btn-icon btn-view"
                  (click)="viewEstimation(estimation)"
                  title="Ver detalles"
                >
                  👁️
                </button>
                <button 
                  class="btn-icon btn-edit"
                  (click)="editEstimation(estimation)"
                  title="Editar"
                >
                  ✎
                </button>
                <button 
                  class="btn-icon btn-delete"
                  (click)="deleteEstimation(estimation)"
                  title="Eliminar"
                >
                  🗑️
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Statistics Summary -->
      <div *ngIf="!loading() && !error() && estimations().length > 0" class="statistics">
        <h3>Resumen General</h3>
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-icon">📊</div>
            <div class="stat-content">
              <div class="stat-value">{{ estimations().length }}</div>
              <div class="stat-label">Total Estimaciones</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon">💻</div>
            <div class="stat-content">
              <div class="stat-value">{{ formatNumber(getTotalLOC()) }}</div>
              <div class="stat-label">Total LOC</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon">👥</div>
            <div class="stat-content">
              <div class="stat-value">{{ formatDecimal(getTotalEffort()) }}</div>
              <div class="stat-label">Total Esfuerzo (PM)</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon">💰</div>
            <div class="stat-content">
              <div class="stat-value">{{ formatCurrency(getTotalCost()) }}</div>
              <div class="stat-label">Costo Total Estimado</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Select Project Dialog -->
    <app-select-project-dialog
      *ngIf="showSelectProjectDialog()"
      (projectSelected)="onProjectSelected($event)"
      (cancel)="closeSelectProjectDialog()"
    ></app-select-project-dialog>

    <!-- Estimation Dialog -->
    <app-kloc-estimation-dialog
      *ngIf="showEstimationDialog()"
      [estimation]="selectedEstimation()"
      [project]="selectedProject()"
      (save)="saveEstimation($event)"
      (cancel)="closeEstimationDialog()"
    ></app-kloc-estimation-dialog>

    <!-- View Details Modal -->
    <div *ngIf="showDetailsDialog()" class="modal-overlay" (click)="closeDetailsDialog()">
      <div class="modal-content details-modal" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2>Detalles de Estimación KLOC</h2>
          <button class="btn-close" (click)="closeDetailsDialog()">×</button>
        </div>
        <div class="modal-body" *ngIf="selectedEstimation()">
          <div class="detail-section">
            <h3>Información General</h3>
            <div class="detail-grid">
              <div class="detail-item">
                <label>Nombre:</label>
                <span>{{ selectedEstimation()!.estimationName }}</span>
              </div>
              <div class="detail-item">
                <label>Proyecto:</label>
                <span>{{ getProjectName(selectedEstimation()!.projectId) }}</span>
              </div>
              <div class="detail-item">
                <label>Fecha de Creación:</label>
                <span>{{ formatDate(selectedEstimation()!.createdAt) }}</span>
              </div>
              <div class="detail-item">
                <label>Última Actualización:</label>
                <span>{{ formatDate(selectedEstimation()!.updatedAt) }}</span>
              </div>
            </div>
          </div>

          <div class="detail-section">
            <h3>Métricas de Código</h3>
            <div class="metrics-grid">
              <div class="metric-box">
                <div class="metric-label">Líneas de Código</div>
                <div class="metric-value">{{ formatNumber(selectedEstimation()!.linesOfCode) }}</div>
              </div>
              <div class="metric-box">
                <div class="metric-label">KLOC</div>
                <div class="metric-value">{{ formatKloc(selectedEstimation()!.linesOfCode) }}</div>
              </div>
            </div>
          </div>

          <div class="detail-section">
            <h3>Estimaciones Calculadas</h3>
            <div class="metrics-grid">
              <div class="metric-box highlight">
                <div class="metric-label">Esfuerzo</div>
                <div class="metric-value">{{ formatDecimal(selectedEstimation()!.estimatedEffort) }} PM</div>
              </div>
              <div class="metric-box highlight">
                <div class="metric-label">Tiempo</div>
                <div class="metric-value">{{ formatDecimal(selectedEstimation()!.estimatedTime) }} Meses</div>
              </div>
              <div class="metric-box highlight">
                <div class="metric-label">Costo</div>
                <div class="metric-value">{{ formatCurrency(selectedEstimation()!.estimatedCost) }}</div>
              </div>
            </div>
          </div>

          <div class="detail-section" *ngIf="selectedEstimation()!.notes">
            <h3>Notas</h3>
            <div class="notes-content">
              {{ selectedEstimation()!.notes }}
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" (click)="closeDetailsDialog()">Cerrar</button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container {
      min-height: 100vh;
      background: linear-gradient(135deg, #f0f7ff 0%, #e8f0ff 100%);
      padding: 2rem;
    }

    .header {
      max-width: 1400px;
      margin: 0 auto 2rem;
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 2rem;
      flex-wrap: wrap;
    }

    .header-content h1 {
      margin: 0;
      color: #1e293b;
      font-size: 2.5rem;
      font-weight: 700;
    }

    .subtitle {
      margin: 0.5rem 0 0;
      color: #64748b;
      font-size: 1.1rem;
    }

    .btn-back {
      background: none;
      border: none;
      color: #3b82f6;
      font-size: 1rem;
      cursor: pointer;
      padding: 0.5rem 1rem;
      border-radius: 6px;
      transition: all 0.2s;
      margin-bottom: 0.5rem;
    }

    .btn-back:hover {
      background-color: #eff6ff;
    }

    .loading-container,
    .error-container,
    .empty-state {
      max-width: 600px;
      margin: 4rem auto;
      text-align: center;
      background: white;
      padding: 3rem;
      border-radius: 12px;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
    }

    .spinner {
      width: 50px;
      height: 50px;
      border: 4px solid #e2e8f0;
      border-top-color: #3b82f6;
      border-radius: 50%;
      animation: spin 1s linear infinite;
      margin: 0 auto 1rem;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }

    .error-icon,
    .empty-icon {
      font-size: 4rem;
      margin-bottom: 1rem;
    }

    .error-icon {
      color: #dc2626;
    }

    .table-container {
      max-width: 1400px;
      margin: 0 auto 2rem;
      background: white;
      border-radius: 12px;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
      overflow: hidden;
    }

    .estimations-table {
      width: 100%;
      border-collapse: collapse;
    }

    .estimations-table th {
      background-color: #f8fafc;
      padding: 1rem;
      text-align: left;
      font-weight: 600;
      color: #334155;
      border-bottom: 2px solid #e2e8f0;
      font-size: 0.9rem;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .estimations-table td {
      padding: 1rem;
      border-bottom: 1px solid #f1f5f9;
      color: #475569;
    }

    .estimation-row:hover {
      background-color: #f8fafc;
    }

    .project-badge {
      background-color: #eff6ff;
      color: #1e40af;
      padding: 0.25rem 0.75rem;
      border-radius: 12px;
      font-size: 0.85rem;
      font-weight: 500;
    }

    .notes-indicator {
      margin-left: 0.5rem;
      cursor: help;
    }

    .actions {
      display: flex;
      gap: 0.5rem;
    }

    .btn-icon {
      background: none;
      border: none;
      padding: 0.5rem;
      cursor: pointer;
      border-radius: 4px;
      transition: all 0.2s;
      font-size: 1.1rem;
    }

    .btn-icon:hover {
      background-color: #f1f5f9;
    }

    .btn-view:hover {
      background-color: #dbeafe;
    }

    .btn-edit:hover {
      background-color: #ddd6fe;
    }

    .btn-delete:hover {
      background-color: #fee2e2;
    }

    .statistics {
      max-width: 1400px;
      margin: 2rem auto;
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
    }

    .statistics h3 {
      margin: 0 0 1.5rem 0;
      color: #1e293b;
      font-size: 1.5rem;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1.5rem;
    }

    .stat-card {
      display: flex;
      gap: 1rem;
      padding: 1.5rem;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      border-radius: 8px;
    }

    .stat-icon {
      font-size: 2.5rem;
    }

    .stat-content {
      flex: 1;
    }

    .stat-value {
      font-size: 2rem;
      font-weight: 700;
      margin-bottom: 0.25rem;
    }

    .stat-label {
      font-size: 0.9rem;
      opacity: 0.95;
    }

    .btn {
      padding: 0.625rem 1.5rem;
      border: none;
      border-radius: 6px;
      font-size: 1rem;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s;
    }

    .btn-primary {
      background-color: #3b82f6;
      color: white;
    }

    .btn-primary:hover {
      background-color: #2563eb;
    }

    .btn-lg {
      padding: 0.875rem 2rem;
      font-size: 1.1rem;
    }

    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background-color: rgba(0, 0, 0, 0.6);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
      padding: 1rem;
      overflow: hidden;
    }

    .modal-content {
      background: white;
      border-radius: 12px;
      width: 100%;
      max-height: 90vh;
      display: flex;
      flex-direction: column;
      box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
      overflow: hidden;
    }

    .details-modal {
      max-width: 800px;
    }

    .modal-header {
      padding: 1.5rem;
      border-bottom: 1px solid #e2e8f0;
      display: flex;
      justify-content: space-between;
      align-items: center;
      flex-shrink: 0;
    }

    .modal-header h2 {
      margin: 0;
      color: #1e293b;
      font-size: 1.5rem;
    }

    .btn-close {
      background: none;
      border: none;
      font-size: 2rem;
      color: #64748b;
      cursor: pointer;
      padding: 0;
      width: 32px;
      height: 32px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 4px;
      transition: all 0.2s;
    }

    .btn-close:hover {
      background-color: #f1f5f9;
    }

    .modal-body {
      padding: 1.5rem;
      overflow-y: auto;
      flex: 1;
      min-height: 0;
    }

    /* Scrollbar personalizado para el modal */
    .modal-body::-webkit-scrollbar {
      width: 8px;
    }

    .modal-body::-webkit-scrollbar-track {
      background: #f1f5f9;
      border-radius: 4px;
    }

    .modal-body::-webkit-scrollbar-thumb {
      background: #cbd5e1;
      border-radius: 4px;
    }

    .modal-body::-webkit-scrollbar-thumb:hover {
      background: #94a3b8;
    }

    .detail-section {
      margin-bottom: 2rem;
    }

    .detail-section h3 {
      margin: 0 0 1rem 0;
      color: #1e293b;
      font-size: 1.2rem;
      padding-bottom: 0.5rem;
      border-bottom: 2px solid #e2e8f0;
    }

    .detail-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1rem;
    }

    .detail-item {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .detail-item label {
      font-weight: 600;
      color: #64748b;
      font-size: 0.85rem;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .detail-item span {
      color: #1e293b;
    }

    .metrics-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 1rem;
    }

    .metric-box {
      padding: 1.25rem;
      background-color: #f8fafc;
      border-radius: 8px;
      text-align: center;
    }

    .metric-box.highlight {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .metric-label {
      font-size: 0.85rem;
      margin-bottom: 0.5rem;
      opacity: 0.9;
    }

    .metric-value {
      font-size: 1.5rem;
      font-weight: 700;
    }

    .notes-content {
      background-color: #f8fafc;
      padding: 1rem;
      border-radius: 8px;
      color: #475569;
      line-height: 1.6;
      white-space: pre-wrap;
    }

    .modal-footer {
      padding: 1rem 1.5rem;
      border-top: 1px solid #e2e8f0;
      display: flex;
      gap: 1rem;
      justify-content: flex-end;
      flex-shrink: 0;
    }

    .btn-secondary {
      background-color: #f1f5f9;
      color: #475569;
    }

    .btn-secondary:hover {
      background-color: #e2e8f0;
    }

    @media (max-width: 768px) {
      .table-container {
        overflow-x: auto;
      }

      .estimations-table {
        min-width: 1000px;
      }
    }
  `]
})
export class KlocEstimationsComponent implements OnInit {
  private klocService = inject(KlocEstimationService);
  private projectService = inject(ProjectService);
  private router = inject(Router);

  estimations = signal<KlocEstimation[]>([]);
  projects = signal<Map<number, string>>(new Map());
  loading = signal(false);
  error = signal<string | null>(null);

  showSelectProjectDialog = signal(false);
  showEstimationDialog = signal(false);
  showDetailsDialog = signal(false);
  selectedProject = signal<Project | null>(null);
  selectedEstimation = signal<KlocEstimation | null>(null);

  ngOnInit() {
    this.loadAllEstimations();
  }

  loadAllEstimations() {
    this.loading.set(true);
    this.error.set(null);

    // Primero, obtener todos los proyectos del usuario
    this.projectService.getProjects().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          const userProjects = response.data;
          
          // Guardar nombres de proyectos en el mapa
          const projectsMap = new Map<number, string>();
          userProjects.forEach(project => {
            projectsMap.set(project.projectId, project.projectName);
          });
          this.projects.set(projectsMap);

          // Si no hay proyectos, mostrar empty state
          if (userProjects.length === 0) {
            this.loading.set(false);
            return;
          }

          // Crear observables para cargar estimaciones de cada proyecto
          const estimationRequests = userProjects.map(project =>
            this.klocService.getEstimationsByProject(project.projectId).pipe(
              catchError(err => {
                console.error(`Error loading estimations for project ${project.projectId}:`, err);
                return of({ success: false, data: [] as KlocEstimation[], message: '' });
              })
            )
          );

          // Ejecutar todas las peticiones en paralelo
          forkJoin(estimationRequests).subscribe({
            next: (responses) => {
              const allEstimations: KlocEstimation[] = [];
              
              responses.forEach(response => {
                if (response.success && response.data) {
                  allEstimations.push(...response.data);
                }
              });

              this.estimations.set(allEstimations);
              this.loading.set(false);
            },
            error: (err) => {
              console.error('Error loading estimations:', err);
              this.error.set('Error al cargar las estimaciones. Por favor, intenta nuevamente.');
              this.loading.set(false);
            }
          });
        } else {
          this.error.set('Error al cargar los proyectos');
          this.loading.set(false);
        }
      },
      error: (err) => {
        console.error('Error loading projects:', err);
        this.error.set('Error al cargar los proyectos. Por favor, intenta nuevamente.');
        this.loading.set(false);
      }
    });
  }

  openCreateFlow() {
    this.selectedEstimation.set(null);
    this.showSelectProjectDialog.set(true);
  }

  onProjectSelected(project: Project) {
    this.selectedProject.set(project);
    this.showSelectProjectDialog.set(false);
    this.showEstimationDialog.set(true);
  }

  closeSelectProjectDialog() {
    this.showSelectProjectDialog.set(false);
    this.selectedProject.set(null);
  }

  closeEstimationDialog() {
    this.showEstimationDialog.set(false);
    this.selectedProject.set(null);
    this.selectedEstimation.set(null);
  }

  saveEstimation(data: any) {
    const project = this.selectedProject();
    const estimation = this.selectedEstimation();

    if (estimation) {
      // Update existing estimation
      const request: UpdateKlocEstimationRequest = {
        klocEstimationId: estimation.klocEstimationId,
        ...data
      };

      this.klocService.updateEstimation(estimation.klocEstimationId, request).subscribe({
        next: (response) => {
          if (response.success && response.data) {
            // Update in list
            this.estimations.update(list => 
              list.map(e => e.klocEstimationId === response.data!.klocEstimationId ? response.data! : e)
            );
            this.closeEstimationDialog();
            this.showSuccessMessage('Estimación actualizada exitosamente');
          } else {
            this.showErrorMessage('Error al actualizar: ' + response.message);
          }
        },
        error: (err) => {
          console.error('Error updating estimation:', err);
          this.showErrorMessage('Error al actualizar la estimación: ' + (err.error?.message || err.message || 'Error desconocido'));
        }
      });
    } else if (project) {
      // Create new estimation
      const request: CreateKlocEstimationRequest = {
        projectId: project.projectId,
        ...data
      };

      this.klocService.createEstimation(request).subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.estimations.update(list => [...list, response.data!]);
            this.projects.update(map => {
              map.set(project.projectId, project.projectName);
              return new Map(map);
            });
            this.closeEstimationDialog();
            this.showSuccessMessage('Estimación creada exitosamente');
          } else {
            this.showErrorMessage('Error al crear: ' + response.message);
          }
        },
        error: (err) => {
          console.error('Error creating estimation:', err);
          this.showErrorMessage('Error al crear la estimación: ' + (err.error?.message || err.message || 'Error desconocido'));
        }
      });
    }
  }

  editEstimation(estimation: KlocEstimation) {
    this.selectedEstimation.set(estimation);
    this.selectedProject.set(null);
    this.showEstimationDialog.set(true);
  }

  deleteEstimation(estimation: KlocEstimation) {
    if (!confirm(`¿Estás seguro de eliminar la estimación "${estimation.estimationName}"?`)) {
      return;
    }

    this.klocService.deleteEstimation(estimation.klocEstimationId).subscribe({
      next: (response) => {
        if (response.success) {
          this.estimations.update(list => 
            list.filter(e => e.klocEstimationId !== estimation.klocEstimationId)
          );
        } else {
          alert('Error al eliminar: ' + response.message);
        }
      },
      error: (err) => {
        alert('Error al eliminar: ' + err.message);
      }
    });
  }

  viewEstimation(estimation: KlocEstimation) {
    this.selectedEstimation.set(estimation);
    this.showDetailsDialog.set(true);
    // Prevenir scroll del body cuando se abre el modal de detalles
    document.body.style.overflow = 'hidden';
  }

  closeDetailsDialog() {
    this.showDetailsDialog.set(false);
    this.selectedEstimation.set(null);
    // Restaurar scroll del body
    document.body.style.overflow = 'auto';
  }

  getProjectName(projectId: number): string {
    return this.projects().get(projectId) || `Proyecto #${projectId}`;
  }

  getTotalLOC(): number {
    return this.estimations().reduce((sum, e) => sum + e.linesOfCode, 0);
  }

  getTotalEffort(): number {
    return this.estimations().reduce((sum, e) => sum + (e.estimatedEffort || 0), 0);
  }

  getTotalCost(): number {
    return this.estimations().reduce((sum, e) => sum + (e.estimatedCost || 0), 0);
  }

  formatNumber(value: number | undefined): string {
    return value ? value.toLocaleString() : 'N/A';
  }

  formatDecimal(value: number | undefined): string {
    return value ? value.toFixed(2) : 'N/A';
  }

  formatKloc(loc: number): string {
    return (loc / 1000).toFixed(2);
  }

  formatCurrency(value: number | undefined): string {
    return value ? `$${value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}` : 'N/A';
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }

  private showSuccessMessage(message: string): void {
    // Podrías usar un servicio de notificaciones aquí
    alert(message);
  }

  private showErrorMessage(message: string): void {
    alert(message);
  }
}
