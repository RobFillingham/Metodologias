import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { SelectProjectDialogComponent } from '../../../shared/components/select-project-dialog/select-project-dialog.component';
import { Cocomo2Stage1EstimationDialogComponent } from '../cocomo2-stage1-estimation-dialog/cocomo2-stage1-estimation-dialog.component';
import { Cocomo2Stage1ComponentsComponent } from '../cocomo2-stage1-components/cocomo2-stage1-components.component';
import { Cocomo2Stage1EstimationService } from '../../../core/services/cocomo2-stage1/cocomo2-stage1-estimation.service';
import { Cocomo2Stage1ComponentService } from '../../../core/services/cocomo2-stage1/cocomo2-stage1-component.service';
import { ProjectService } from '../../../core/services/cocomo2/project.service';
import { forkJoin, of } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { 
  Cocomo2Stage1Estimation,
  CreateCocomo2Stage1EstimationRequest,
  CreateComponentRequest,
  CreateBatchComponentsRequest
} from '../../../core/models/cocomo2-stage1/cocomo2-stage1.models';
import { Project } from '../../../core/models/kloc/kloc.models';

@Component({
  selector: 'app-cocomo2-stage1-estimations',
  standalone: true,
  imports: [
    CommonModule, 
    NavbarComponent,
    SelectProjectDialogComponent,
    Cocomo2Stage1EstimationDialogComponent,
    Cocomo2Stage1ComponentsComponent
  ],
  template: `
    <app-navbar></app-navbar>

    <div class="container">
      <div class="header">
        <div class="header-content">
          <button class="btn-back" (click)="goBack()" title="Regresar">
            ← Regresar
          </button>
          <h1>Estimaciones COCOMO II - Estadio 1</h1>
          <p class="subtitle">Modelo de Composición (Application Composition Model)</p>
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
        <h2>Sin estimaciones COCOMO II Estadio 1</h2>
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
              <th>Lenguaje</th>
              <th>FP Total</th>
              <th>SLOC</th>
              <th>KSLOC</th>
              <th>Esfuerzo (PM)</th>
              <th>Esfuerzo (Horas)</th>
              <th>Fecha Creación</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let estimation of estimations()" class="estimation-row">
              <td>
                <strong>{{ estimation.estimationName }}</strong>
              </td>
              <td>
                <span class="project-badge">{{ getProjectName(estimation.projectId) }}</span>
              </td>
              <td>{{ estimation.language?.name || 'N/A' }}</td>
              <td>{{ formatDecimal(estimation.totalFp) }}</td>
              <td>{{ formatDecimal(estimation.sloc) }}</td>
              <td>{{ formatDecimal(estimation.ksloc) }}</td>
              <td>{{ formatDecimal(estimation.effortPm) }}</td>
              <td>{{ formatDecimal(estimation.effortHours) }}</td>
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
              <div class="stat-value">{{ formatDecimal(getTotalFP()) }}</div>
              <div class="stat-label">Total FP</div>
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
            <div class="stat-icon">⏱️</div>
            <div class="stat-content">
              <div class="stat-value">{{ formatDecimal(getTotalHours()) }}</div>
              <div class="stat-label">Total Horas</div>
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
    <app-cocomo2-stage1-estimation-dialog
      *ngIf="showEstimationDialog()"
      [estimation]="selectedEstimation()"
      [project]="selectedProject()"
      (save)="saveEstimation($event)"
      (cancel)="closeEstimationDialog()"
    ></app-cocomo2-stage1-estimation-dialog>

    <!-- View Details Modal -->
    <div *ngIf="showDetailsDialog()" class="modal-overlay" (click)="closeDetailsDialog()">
      <div class="modal-content details-modal" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2>Detalles de Estimación COCOMO II Stage 1</h2>
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
                <label>Lenguaje:</label>
                <span>{{ selectedEstimation()!.language?.name || 'N/A' }}</span>
              </div>
              <div class="detail-item">
                <label>Conjunto de Parámetros:</label>
                <span>{{ selectedEstimation()!.parameterSet?.setName || 'N/A' }}</span>
              </div>
            </div>
          </div>

          <div class="detail-section">
            <h3>Factores ACEM</h3>
            <div class="detail-grid">
              <div class="detail-item">
                <label>AEXP (Analyst Experience):</label>
                <span>{{ selectedEstimation()!.selectedAexp }}</span>
              </div>
              <div class="detail-item">
                <label>PEXP (Programmer Experience):</label>
                <span>{{ selectedEstimation()!.selectedPexp }}</span>
              </div>
              <div class="detail-item">
                <label>PREC (Precedentedness):</label>
                <span>{{ selectedEstimation()!.selectedPrec }}</span>
              </div>
              <div class="detail-item">
                <label>RELY (Required Reliability):</label>
                <span>{{ selectedEstimation()!.selectedRely }}</span>
              </div>
              <div class="detail-item">
                <label>TMSP (Team Support):</label>
                <span>{{ selectedEstimation()!.selectedTmsp }}</span>
              </div>
            </div>
          </div>

          <div class="detail-section">
            <h3>Resultados Calculados</h3>
            <div class="metrics-grid">
              <div class="metric-box highlight">
                <div class="metric-label">Total FP</div>
                <div class="metric-value">{{ formatDecimal(selectedEstimation()!.totalFp) }}</div>
              </div>
              <div class="metric-box highlight">
                <div class="metric-label">SLOC</div>
                <div class="metric-value">{{ formatDecimal(selectedEstimation()!.sloc) }}</div>
              </div>
              <div class="metric-box highlight">
                <div class="metric-label">KSLOC</div>
                <div class="metric-value">{{ formatDecimal(selectedEstimation()!.ksloc) }}</div>
              </div>
              <div class="metric-box highlight">
                <div class="metric-label">EAF</div>
                <div class="metric-value">{{ formatDecimal(selectedEstimation()!.eaf) }}</div>
              </div>
              <div class="metric-box highlight">
                <div class="metric-label">Esfuerzo (PM)</div>
                <div class="metric-value">{{ formatDecimal(selectedEstimation()!.effortPm) }}</div>
              </div>
              <div class="metric-box highlight">
                <div class="metric-label">Esfuerzo (Horas)</div>
                <div class="metric-value">{{ formatDecimal(selectedEstimation()!.effortHours) }}</div>
              </div>
            </div>
          </div>

          <!-- Components Management -->
          <div class="detail-section">
            <app-cocomo2-stage1-components
              [estimation]="selectedEstimation()"
              (componentsChanged)="onComponentsChanged()"
            ></app-cocomo2-stage1-components>
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
      overflow-x: auto;
    }

    .estimations-table {
      width: 100%;
      border-collapse: collapse;
      min-width: 1000px;
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

    @media (max-width: 768px) {
      .header {
        flex-direction: column;
        align-items: stretch;
      }
    }

    /* Modal Styles */
    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background-color: rgba(0, 0, 0, 0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
      padding: 20px;
    }

    .modal-content {
      background: white;
      border-radius: 8px;
      max-width: 900px;
      width: 100%;
      max-height: 90vh;
      overflow-y: auto;
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
    }

    .detail-section {
      margin-bottom: 24px;
      padding-bottom: 24px;
      border-bottom: 1px solid #e0e0e0;
    }

    .detail-section:last-child {
      border-bottom: none;
      margin-bottom: 0;
    }

    .detail-section h3 {
      font-size: 1.1rem;
      font-weight: 600;
      color: #333;
      margin-bottom: 16px;
    }

    .detail-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 16px;
    }

    .detail-item {
      padding: 12px;
      background-color: #f8f9fa;
      border-radius: 6px;
    }

    .detail-item label {
      display: block;
      font-size: 0.85rem;
      color: #666;
      margin-bottom: 4px;
      font-weight: 500;
    }

    .detail-item span {
      font-size: 1rem;
      color: #333;
      font-weight: 600;
    }

    .metrics-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 16px;
    }

    .metric-box {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 16px;
      border-radius: 8px;
      text-align: center;
    }

    .metric-box label {
      display: block;
      font-size: 0.85rem;
      opacity: 0.9;
      margin-bottom: 8px;
    }

    .metric-box span {
      display: block;
      font-size: 1.5rem;
      font-weight: bold;
    }
  `]
})
export class Cocomo2Stage1EstimationsComponent implements OnInit {
  private cocomo2Service = inject(Cocomo2Stage1EstimationService);
  private componentService = inject(Cocomo2Stage1ComponentService);
  private projectService = inject(ProjectService);
  private router = inject(Router);

  estimations = signal<Cocomo2Stage1Estimation[]>([]);
  projects = signal<Map<number, string>>(new Map());
  loading = signal(false);
  error = signal<string | null>(null);

  showSelectProjectDialog = signal(false);
  showEstimationDialog = signal(false);
  showDetailsDialog = signal(false);
  selectedProject = signal<Project | null>(null);
  selectedEstimation = signal<Cocomo2Stage1Estimation | null>(null);

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
            this.cocomo2Service.getEstimationsByProject(project.projectId).pipe(
              catchError(err => {
                console.error(`Error loading estimations for project ${project.projectId}:`, err);
                return of({ success: false, data: [] as Cocomo2Stage1Estimation[], message: '' });
              })
            )
          );

          // Ejecutar todas las peticiones en paralelo
          forkJoin(estimationRequests).subscribe({
            next: (responses) => {
              const allEstimations: Cocomo2Stage1Estimation[] = [];
              
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

  saveEstimation(data: { estimation: CreateCocomo2Stage1EstimationRequest; components: CreateComponentRequest[] }) {
    const project = this.selectedProject();

    console.log('saveEstimation called with data:', data);
    console.log('Selected project:', project);

    if (!project) {
      alert('Error: No se ha seleccionado un proyecto');
      return;
    }

    const estimationData = data.estimation;

    // Validate data before sending
    if (!estimationData.projectId || estimationData.projectId === 0) {
      alert('Error: El ID del proyecto no es válido');
      return;
    }

    if (!estimationData.estimationName || estimationData.estimationName.trim() === '') {
      alert('Error: El nombre de la estimación es obligatorio');
      return;
    }

    if (!estimationData.languageId || estimationData.languageId === 0) {
      alert('Error: Debe seleccionar un lenguaje');
      return;
    }

    if (!estimationData.paramSetId || estimationData.paramSetId === 0) {
      alert('Error: Debe seleccionar un conjunto de parámetros');
      return;
    }

    if (data.components.length === 0) {
      alert('Error: Debe agregar al menos un componente');
      return;
    }

    console.log('Creating estimation with valid data:', estimationData);

    // First create the estimation, then add components in batch
    this.cocomo2Service.createEstimation(estimationData).pipe(
      switchMap(response => {
        if (!response.success || !response.data) {
          throw new Error(response.message || 'Error creating estimation');
        }

        const estimationId = response.data.estimationId;
        console.log('Estimation created with ID:', estimationId);

        // Now create components in batch
        const batchRequest: CreateBatchComponentsRequest = {
          components: data.components
        };

        return this.componentService.createBatchComponents(estimationId, batchRequest).pipe(
          switchMap(componentResponse => {
            if (!componentResponse.success) {
              throw new Error('Error creating components');
            }

            console.log('Components created successfully');

            // Reload the estimation with updated calculations
            return this.cocomo2Service.getEstimationById(estimationId);
          })
        );
      })
    ).subscribe({
      next: (finalResponse) => {
        console.log('Final estimation response:', finalResponse);
        if (finalResponse.success && finalResponse.data) {
          this.estimations.update(list => [...list, finalResponse.data!]);
          this.projects.update(map => {
            map.set(project.projectId, project.projectName);
            return new Map(map);
          });
          this.closeEstimationDialog();
          alert('✓ Estimación creada exitosamente con ' + data.components.length + ' componente(s)');
        }
      },
      error: (err) => {
        console.error('Error creating estimation:', err);
        console.error('Error details:', {
          status: err.status,
          statusText: err.statusText,
          error: err.error,
          message: err.message
        });
        
        let errorMessage = 'Error al crear la estimación';
        
        if (err.error && err.error.message) {
          errorMessage += ': ' + err.error.message;
        } else if (err.error && typeof err.error === 'string') {
          errorMessage += ': ' + err.error;
        } else if (err.message) {
          errorMessage += ': ' + err.message;
        }
        
        alert(errorMessage);
      }
    });
  }

  viewEstimation(estimation: Cocomo2Stage1Estimation) {
    this.selectedEstimation.set(estimation);
    this.showDetailsDialog.set(true);
    document.body.style.overflow = 'hidden';
  }

  closeDetailsDialog() {
    this.showDetailsDialog.set(false);
    this.selectedEstimation.set(null);
    document.body.style.overflow = 'auto';
  }

  onComponentsChanged() {
    // Recargar la estimación actual para obtener los valores actualizados
    const estimation = this.selectedEstimation();
    if (estimation) {
      this.cocomo2Service.getEstimationById(estimation.estimationId).subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.selectedEstimation.set(response.data);
            // Actualizar también en la lista
            this.estimations.update(list =>
              list.map(e => e.estimationId === response.data!.estimationId ? response.data! : e)
            );
          }
        },
        error: (err) => console.error('Error reloading estimation:', err)
      });
    }
  }

  deleteEstimation(estimation: Cocomo2Stage1Estimation) {
    if (!confirm(`¿Estás seguro de eliminar la estimación "${estimation.estimationName}"?`)) {
      return;
    }

    this.cocomo2Service.deleteEstimation(estimation.estimationId).subscribe({
      next: (response) => {
        if (response.success) {
          this.estimations.update(list => 
            list.filter(e => e.estimationId !== estimation.estimationId)
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

  getProjectName(projectId: number): string {
    return this.projects().get(projectId) || `Proyecto #${projectId}`;
  }

  getTotalFP(): number {
    return this.estimations().reduce((sum, e) => sum + (e.totalFp || 0), 0);
  }

  getTotalEffort(): number {
    return this.estimations().reduce((sum, e) => sum + (e.effortPm || 0), 0);
  }

  getTotalHours(): number {
    return this.estimations().reduce((sum, e) => sum + (e.effortHours || 0), 0);
  }

  formatDecimal(value: number | undefined): string {
    return value ? value.toFixed(2) : 'N/A';
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
    alert(message);
  }

  private showErrorMessage(message: string): void {
    alert(message);
  }
}
