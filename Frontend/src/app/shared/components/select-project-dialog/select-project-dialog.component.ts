import { Component, EventEmitter, Output, signal, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProjectService } from '../../../core/services/cocomo2/project.service';
import { Project } from '../../../core/models/kloc/kloc.models';

@Component({
  selector: 'app-select-project-dialog',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="modal-overlay" (click)="onCancel()">
      <div class="modal-content" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2>Seleccionar Proyecto</h2>
          <button class="btn-close" (click)="onCancel()" aria-label="Cerrar">×</button>
        </div>

        <div class="modal-body">
          <!-- Loading State -->
          <div *ngIf="loading()" class="loading-container">
            <div class="spinner"></div>
            <p>Cargando proyectos...</p>
          </div>

          <!-- Error State -->
          <div *ngIf="error()" class="error-container">
            <p class="error-message">{{ error() }}</p>
            <button class="btn btn-primary" (click)="loadProjects()">Reintentar</button>
          </div>

          <!-- Projects List -->
          <div *ngIf="!loading() && !error()" class="projects-list">
            <p class="info-text">Selecciona el proyecto para el cual deseas crear una estimación KLOC:</p>
            
            <div *ngIf="projects().length === 0" class="empty-state">
              <p>No tienes proyectos disponibles.</p>
              <p class="hint">Crea un proyecto primero desde la sección de Proyectos.</p>
            </div>

            <div 
              *ngFor="let project of projects()" 
              class="project-item"
              (click)="selectProject(project)"
              [class.selected]="selectedProject()?.projectId === project.projectId"
            >
              <div class="project-info">
                <h3>{{ project.projectName }}</h3>
                <p *ngIf="project.description">{{ project.description }}</p>
                <p *ngIf="!project.description" class="no-description">Sin descripción</p>
              </div>
              <div class="selection-indicator" *ngIf="selectedProject()?.projectId === project.projectId">
                ✓
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button 
            class="btn btn-secondary" 
            (click)="onCancel()"
          >
            Cancelar
          </button>
          <button 
            class="btn btn-primary" 
            (click)="onConfirm()"
            [disabled]="!selectedProject() || loading()"
          >
            Continuar
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
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
      max-width: 600px;
      max-height: 85vh;
      display: flex;
      flex-direction: column;
      box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
      overflow: hidden;
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
      color: #334155;
    }

    .modal-body {
      padding: 1.5rem;
      overflow-y: auto;
      flex: 1;
      min-height: 0;
    }

    /* Scrollbar personalizado */
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

    .loading-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 3rem 1rem;
    }

    .spinner {
      width: 40px;
      height: 40px;
      border: 4px solid #e2e8f0;
      border-top-color: #3b82f6;
      border-radius: 50%;
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }

    .loading-container p {
      margin-top: 1rem;
      color: #64748b;
    }

    .error-container {
      text-align: center;
      padding: 2rem 1rem;
    }

    .error-message {
      color: #dc2626;
      margin-bottom: 1rem;
    }

    .info-text {
      color: #64748b;
      margin-bottom: 1rem;
      font-size: 0.95rem;
    }

    .projects-list {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }

    .empty-state {
      text-align: center;
      padding: 3rem 1rem;
      color: #64748b;
    }

    .empty-state p {
      margin: 0.5rem 0;
    }

    .hint {
      font-size: 0.9rem;
      color: #94a3b8;
    }

    .project-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1rem;
      border: 2px solid #e2e8f0;
      border-radius: 8px;
      cursor: pointer;
      transition: all 0.2s;
    }

    .project-item:hover {
      border-color: #3b82f6;
      background-color: #f8fafc;
    }

    .project-item.selected {
      border-color: #3b82f6;
      background-color: #eff6ff;
    }

    .project-info h3 {
      margin: 0 0 0.25rem 0;
      color: #1e293b;
      font-size: 1.1rem;
    }

    .project-info p {
      margin: 0;
      color: #64748b;
      font-size: 0.9rem;
    }

    .no-description {
      font-style: italic;
      color: #94a3b8;
    }

    .selection-indicator {
      width: 32px;
      height: 32px;
      background-color: #3b82f6;
      color: white;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.2rem;
      font-weight: bold;
    }

    .modal-footer {
      padding: 1rem 1.5rem;
      border-top: 1px solid #e2e8f0;
      display: flex;
      gap: 1rem;
      justify-content: flex-end;
      flex-shrink: 0;
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

    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .btn-secondary {
      background-color: #f1f5f9;
      color: #475569;
    }

    .btn-secondary:hover:not(:disabled) {
      background-color: #e2e8f0;
    }

    .btn-primary {
      background-color: #3b82f6;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background-color: #2563eb;
    }
  `]
})
export class SelectProjectDialogComponent implements OnInit, OnDestroy {
  private projectService = inject(ProjectService);

  @Output() projectSelected = new EventEmitter<Project>();
  @Output() cancel = new EventEmitter<void>();

  projects = signal<Project[]>([]);
  selectedProject = signal<Project | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);

  ngOnInit() {
    this.loadProjects();
    // Prevenir scroll del body cuando el modal está abierto
    document.body.style.overflow = 'hidden';
  }

  ngOnDestroy() {
    // Restaurar scroll del body al cerrar el modal
    document.body.style.overflow = 'auto';
  }

  loadProjects() {
    this.loading.set(true);
    this.error.set(null);

    this.projectService.getProjects().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.projects.set(response.data);
        } else {
          this.error.set(response.message || 'Error al cargar proyectos');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.message || 'Error al cargar proyectos');
        this.loading.set(false);
      }
    });
  }

  selectProject(project: Project) {
    this.selectedProject.set(project);
  }

  onConfirm() {
    const project = this.selectedProject();
    if (project) {
      this.projectSelected.emit(project);
    }
  }

  onCancel() {
    this.cancel.emit();
  }
}
