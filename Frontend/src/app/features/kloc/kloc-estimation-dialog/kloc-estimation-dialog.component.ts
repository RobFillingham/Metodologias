import { Component, EventEmitter, Input, Output, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { KlocEstimation, Project } from '../../../core/models/kloc/kloc.models';

@Component({
  selector: 'app-kloc-estimation-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="modal-overlay" (click)="onCancel()">
      <div class="modal-content" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2>{{ isEditMode() ? 'Editar Estimación KLOC' : 'Nueva Estimación KLOC' }}</h2>
          <button class="btn-close" (click)="onCancel()" aria-label="Cerrar">×</button>
        </div>

        <form (ngSubmit)="onSubmit()" #estimationForm="ngForm">
          <div class="modal-body">
            <!-- Project Info (Solo en creación) -->
            <div *ngIf="!isEditMode() && project" class="project-info">
              <label>Proyecto seleccionado:</label>
              <div class="project-badge">
                <strong>{{ project.projectName }}</strong>
                <p *ngIf="project.description">{{ project.description }}</p>
              </div>
            </div>

            <!-- Estimation Name -->
            <div class="form-group">
              <label for="estimationName">
                Nombre de la Estimación <span class="required">*</span>
              </label>
              <input
                type="text"
                id="estimationName"
                name="estimationName"
                [(ngModel)]="formData.estimationName"
                required
                maxlength="255"
                placeholder="Ej: Estimación Módulo de Usuarios"
                class="form-control"
                [class.error]="estimationForm.submitted && !formData.estimationName"
              />
              <small class="hint">Identificador descriptivo para esta estimación</small>
              <div 
                class="error-message" 
                *ngIf="estimationForm.submitted && !formData.estimationName"
              >
                El nombre es requerido
              </div>
            </div>

            <!-- Lines of Code -->
            <div class="form-group">
              <label for="linesOfCode">
                Líneas de Código (LOC) <span class="required">*</span>
              </label>
              <input
                type="number"
                id="linesOfCode"
                name="linesOfCode"
                [(ngModel)]="formData.linesOfCode"
                required
                min="1"
                step="1"
                placeholder="Ej: 5000"
                class="form-control"
                [class.error]="estimationForm.submitted && (!formData.linesOfCode || formData.linesOfCode < 1)"
              />
              <small class="hint">Cantidad total estimada de líneas de código</small>
              <div 
                class="error-message" 
                *ngIf="estimationForm.submitted && (!formData.linesOfCode || formData.linesOfCode < 1)"
              >
                Debe ingresar un número mayor a 0
              </div>
            </div>

            <!-- KLOC Display -->
            <div class="kloc-display" *ngIf="formData.linesOfCode > 0">
              <div class="metric-card">
                <span class="metric-label">KLOC (Miles de Líneas)</span>
                <span class="metric-value">{{ (formData.linesOfCode / 1000).toFixed(2) }}</span>
              </div>
            </div>

            <!-- Notes -->
            <div class="form-group">
              <label for="notes">
                Notas Adicionales <span class="optional">(opcional)</span>
              </label>
              <textarea
                id="notes"
                name="notes"
                [(ngModel)]="formData.notes"
                maxlength="1000"
                rows="4"
                placeholder="Comentarios, supuestos o detalles adicionales..."
                class="form-control"
              ></textarea>
              <small class="hint">{{ formData.notes?.length || 0 }} / 1000 caracteres</small>
            </div>

            <!-- Info Box -->
            <div class="info-box">
              <div class="info-icon">ℹ️</div>
              <div class="info-content">
                <strong>Sobre el método KLOC:</strong>
                <p>
                  La estimación KLOC calculará automáticamente el esfuerzo (persona-mes),
                  tiempo (meses) y costo estimado basándose en las líneas de código ingresadas.
                </p>
              </div>
            </div>

            <!-- Error Message -->
            <div class="error-container" *ngIf="error()">
              <p class="error-message">{{ error() }}</p>
            </div>
          </div>

          <div class="modal-footer">
            <button 
              type="button"
              class="btn btn-secondary" 
              (click)="onCancel()"
              [disabled]="saving()"
            >
              Cancelar
            </button>
            <button 
              type="submit"
              class="btn btn-primary"
              [disabled]="saving()"
            >
              <span *ngIf="!saving()">
                {{ isEditMode() ? 'Guardar Cambios' : 'Crear Estimación' }}
              </span>
              <span *ngIf="saving()">
                <span class="spinner-small"></span>
                Guardando...
              </span>
            </button>
          </div>
        </form>
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
      max-height: 80vh;
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
      overflow-y: auto !important;
      overflow-x: hidden;
      flex: 1 1 auto;
      max-height: calc(80vh - 140px);
    }

    /* Scrollbar personalizado para el modal-body */
    .modal-body::-webkit-scrollbar {
      width: 10px;
    }

    .modal-body::-webkit-scrollbar-track {
      background: #f1f5f9;
      border-radius: 4px;
      margin: 4px 0;
    }

    .modal-body::-webkit-scrollbar-thumb {
      background: #94a3b8;
      border-radius: 4px;
      border: 2px solid #f1f5f9;
    }

    .modal-body::-webkit-scrollbar-thumb:hover {
      background: #64748b;
    }

    .project-info {
      margin-bottom: 1.5rem;
    }

    .project-info label {
      display: block;
      margin-bottom: 0.5rem;
      color: #64748b;
      font-size: 0.9rem;
      font-weight: 500;
    }

    .project-badge {
      padding: 1rem;
      background-color: #eff6ff;
      border: 1px solid #bfdbfe;
      border-radius: 8px;
    }

    .project-badge strong {
      color: #1e40af;
      display: block;
      margin-bottom: 0.25rem;
    }

    .project-badge p {
      margin: 0;
      color: #64748b;
      font-size: 0.9rem;
    }

    .form-group {
      margin-bottom: 1.5rem;
    }

    label {
      display: block;
      margin-bottom: 0.5rem;
      color: #334155;
      font-weight: 500;
    }

    .required {
      color: #dc2626;
    }

    .optional {
      color: #94a3b8;
      font-weight: 400;
      font-size: 0.9rem;
    }

    .form-control {
      width: 100%;
      padding: 0.625rem;
      border: 1px solid #cbd5e1;
      border-radius: 6px;
      font-size: 1rem;
      transition: all 0.2s;
    }

    .form-control:focus {
      outline: none;
      border-color: #3b82f6;
      box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
    }

    .form-control.error {
      border-color: #dc2626;
    }

    .form-control.error:focus {
      box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.1);
    }

    textarea.form-control {
      resize: vertical;
      min-height: 100px;
    }

    .hint {
      display: block;
      margin-top: 0.25rem;
      color: #64748b;
      font-size: 0.85rem;
    }

    .kloc-display {
      margin: 1.5rem 0;
    }

    .metric-card {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 1.25rem;
      border-radius: 8px;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .metric-label {
      font-size: 0.9rem;
      opacity: 0.95;
    }

    .metric-value {
      font-size: 2rem;
      font-weight: 700;
    }

    .info-box {
      background-color: #f0f9ff;
      border: 1px solid #bae6fd;
      border-radius: 8px;
      padding: 1rem;
      display: flex;
      gap: 0.75rem;
      margin-top: 1.5rem;
    }

    .info-icon {
      font-size: 1.25rem;
      flex-shrink: 0;
    }

    .info-content strong {
      display: block;
      color: #0c4a6e;
      margin-bottom: 0.25rem;
    }

    .info-content p {
      margin: 0;
      color: #075985;
      font-size: 0.9rem;
      line-height: 1.5;
    }

    .error-container {
      margin-top: 1rem;
      padding: 1rem;
      background-color: #fef2f2;
      border: 1px solid #fecaca;
      border-radius: 6px;
    }

    .error-message {
      color: #dc2626;
      margin: 0;
      font-size: 0.9rem;
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
      display: flex;
      align-items: center;
      gap: 0.5rem;
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

    .spinner-small {
      width: 16px;
      height: 16px;
      border: 2px solid rgba(255, 255, 255, 0.3);
      border-top-color: white;
      border-radius: 50%;
      animation: spin 0.8s linear infinite;
      display: inline-block;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }
  `]
})
export class KlocEstimationDialogComponent implements OnInit, OnDestroy {
  @Input() estimation: KlocEstimation | null = null;
  @Input() project: Project | null = null;
  @Output() save = new EventEmitter<any>();
  @Output() cancel = new EventEmitter<void>();

  isEditMode = signal(false);
  saving = signal(false);
  error = signal<string | null>(null);

  formData = {
    estimationName: '',
    linesOfCode: 0,
    notes: ''
  };

  ngOnInit() {
    if (this.estimation) {
      this.isEditMode.set(true);
      this.formData = {
        estimationName: this.estimation.estimationName,
        linesOfCode: this.estimation.linesOfCode,
        notes: this.estimation.notes || ''
      };
    }
    // Prevenir scroll del body cuando el modal está abierto
    document.body.style.overflow = 'hidden';
  }

  ngOnDestroy() {
    // Restaurar scroll del body al cerrar el modal
    document.body.style.overflow = 'auto';
  }

  onSubmit() {
    this.error.set(null);
    
    if (!this.formData.estimationName || this.formData.linesOfCode < 1) {
      return;
    }

    const data = {
      estimationName: this.formData.estimationName.trim(),
      linesOfCode: this.formData.linesOfCode,
      notes: this.formData.notes?.trim() || undefined
    };

    this.saving.set(true);
    this.save.emit(data);
  }

  onCancel() {
    this.cancel.emit();
  }
}
