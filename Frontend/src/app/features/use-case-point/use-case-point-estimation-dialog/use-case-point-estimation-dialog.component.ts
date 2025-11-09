import { Component, EventEmitter, Input, Output, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UseCasePointEstimation, Project } from '../../../core/models/use-case-point/use-case-point.models';

@Component({
  selector: 'app-use-case-point-estimation-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="modal-overlay" (click)="onCancel()">
      <div class="modal-content" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2>{{ isEditMode() ? 'Editar Estimación UCP' : 'Nueva Estimación de Puntos de Caso de Uso' }}</h2>
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
                placeholder="Ej: Estimación Sistema de Reservas"
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

            <!-- Use Cases Section -->
            <div class="section-header">
              <h3>Casos de Uso</h3>
              <p>Clasificados por complejidad según número de transacciones</p>
            </div>

            <div class="input-grid">
              <!-- Simple Use Cases -->
              <div class="form-group">
                <label for="simpleUccCount">
                  Casos de Uso Simples <span class="required">*</span>
                </label>
                <input
                  type="number"
                  id="simpleUccCount"
                  name="simpleUccCount"
                  [(ngModel)]="formData.simpleUccCount"
                  required
                  min="0"
                  step="1"
                  placeholder="0"
                  class="form-control"
                  [class.error]="estimationForm.submitted && formData.simpleUccCount < 0"
                />
                <small class="hint">3 o menos transacciones (peso: 5)</small>
              </div>

              <!-- Average Use Cases -->
              <div class="form-group">
                <label for="averageUccCount">
                  Casos de Uso Promedio <span class="required">*</span>
                </label>
                <input
                  type="number"
                  id="averageUccCount"
                  name="averageUccCount"
                  [(ngModel)]="formData.averageUccCount"
                  required
                  min="0"
                  step="1"
                  placeholder="0"
                  class="form-control"
                  [class.error]="estimationForm.submitted && formData.averageUccCount < 0"
                />
                <small class="hint">4 a 7 transacciones (peso: 10)</small>
              </div>

              <!-- Complex Use Cases -->
              <div class="form-group">
                <label for="complexUccCount">
                  Casos de Uso Complejos <span class="required">*</span>
                </label>
                <input
                  type="number"
                  id="complexUccCount"
                  name="complexUccCount"
                  [(ngModel)]="formData.complexUccCount"
                  required
                  min="0"
                  step="1"
                  placeholder="0"
                  class="form-control"
                  [class.error]="estimationForm.submitted && formData.complexUccCount < 0"
                />
                <small class="hint">Más de 7 transacciones (peso: 15)</small>
              </div>
            </div>

            <!-- Actors Section -->
            <div class="section-header">
              <h3>Actores</h3>
              <p>Clasificados por complejidad de interacción</p>
            </div>

            <div class="input-grid">
              <!-- Simple Actors -->
              <div class="form-group">
                <label for="simpleActorCount">
                  Actores Simples <span class="required">*</span>
                </label>
                <input
                  type="number"
                  id="simpleActorCount"
                  name="simpleActorCount"
                  [(ngModel)]="formData.simpleActorCount"
                  required
                  min="0"
                  step="1"
                  placeholder="0"
                  class="form-control"
                  [class.error]="estimationForm.submitted && formData.simpleActorCount < 0"
                />
                <small class="hint">API definida (peso: 1)</small>
              </div>

              <!-- Average Actors -->
              <div class="form-group">
                <label for="averageActorCount">
                  Actores Promedio <span class="required">*</span>
                </label>
                <input
                  type="number"
                  id="averageActorCount"
                  name="averageActorCount"
                  [(ngModel)]="formData.averageActorCount"
                  required
                  min="0"
                  step="1"
                  placeholder="0"
                  class="form-control"
                  [class.error]="estimationForm.submitted && formData.averageActorCount < 0"
                />
                <small class="hint">Protocolo o interfaz (peso: 2)</small>
              </div>

              <!-- Complex Actors -->
              <div class="form-group">
                <label for="complexActorCount">
                  Actores Complejos <span class="required">*</span>
                </label>
                <input
                  type="number"
                  id="complexActorCount"
                  name="complexActorCount"
                  [(ngModel)]="formData.complexActorCount"
                  required
                  min="0"
                  step="1"
                  placeholder="0"
                  class="form-control"
                  [class.error]="estimationForm.submitted && formData.complexActorCount < 0"
                />
                <small class="hint">Interfaz gráfica (peso: 3)</small>
              </div>
            </div>

            <!-- UUCP Preview -->
            <div class="preview-section" *ngIf="calculateUUCP() > 0">
              <h4>Vista Previa</h4>
              <div class="metrics-preview">
                <div class="metric-card">
                  <span class="metric-label">Puntos de Caso de Uso Sin Ajustar (UUCP)</span>
                  <span class="metric-value">{{ calculateUUCP() }}</span>
                </div>
              </div>
              <div class="breakdown">
                <div class="breakdown-item">
                  <span class="breakdown-label">UAW (Peso de Actores):</span>
                  <span class="breakdown-value">{{ calculateUAW() }}</span>
                </div>
                <div class="breakdown-item">
                  <span class="breakdown-label">UUCW (Peso de Casos de Uso):</span>
                  <span class="breakdown-value">{{ calculateUUCW() }}</span>
                </div>
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
                <strong>Sobre el método UCP (Use Case Points):</strong>
                <p>
                  Este método calcula automáticamente los puntos de caso de uso ajustados,
                  considerando factores técnicos y ambientales del proyecto. El backend
                  calculará el esfuerzo, tiempo y costo estimados.
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
      max-width: 800px;
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
      overflow-y: auto !important;
      overflow-x: hidden;
      flex: 1 1 auto;
      max-height: calc(85vh - 140px);
    }

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

    .section-header {
      margin: 2rem 0 1.5rem;
      padding-bottom: 0.75rem;
      border-bottom: 2px solid #e2e8f0;
    }

    .section-header h3 {
      margin: 0 0 0.25rem 0;
      color: #1e293b;
      font-size: 1.2rem;
    }

    .section-header p {
      margin: 0;
      color: #64748b;
      font-size: 0.9rem;
    }

    .input-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
      gap: 1.5rem;
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

    .preview-section {
      margin: 1.5rem 0;
      padding: 1.25rem;
      background-color: #fafafa;
      border-radius: 8px;
      border: 1px solid #e2e8f0;
    }

    .preview-section h4 {
      margin: 0 0 1rem 0;
      color: #1e293b;
      font-size: 1rem;
    }

    .metrics-preview {
      margin-bottom: 1rem;
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

    .breakdown {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 0.75rem;
    }

    .breakdown-item {
      display: flex;
      justify-content: space-between;
      padding: 0.75rem;
      background-color: white;
      border-radius: 6px;
      border: 1px solid #e2e8f0;
    }

    .breakdown-label {
      font-size: 0.85rem;
      color: #64748b;
      font-weight: 500;
    }

    .breakdown-value {
      font-size: 0.9rem;
      color: #1e293b;
      font-weight: 600;
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

    @media (max-width: 768px) {
      .input-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class UseCasePointEstimationDialogComponent implements OnInit, OnDestroy {
  @Input() estimation: UseCasePointEstimation | null = null;
  @Input() project: Project | null = null;
  @Output() save = new EventEmitter<any>();
  @Output() cancel = new EventEmitter<void>();

  isEditMode = signal(false);
  saving = signal(false);
  error = signal<string | null>(null);

  formData = {
    estimationName: '',
    simpleUccCount: 0,
    averageUccCount: 0,
    complexUccCount: 0,
    simpleActorCount: 0,
    averageActorCount: 0,
    complexActorCount: 0,
    notes: ''
  };

  // UCP weights
  private readonly weights = {
    useCase: { simple: 5, average: 10, complex: 15 },
    actor: { simple: 1, average: 2, complex: 3 }
  };

  ngOnInit() {
    if (this.estimation) {
      this.isEditMode.set(true);
      this.formData = {
        estimationName: this.estimation.estimationName,
        simpleUccCount: this.estimation.simpleUccCount,
        averageUccCount: this.estimation.averageUccCount,
        complexUccCount: this.estimation.complexUccCount,
        simpleActorCount: this.estimation.simpleActorCount,
        averageActorCount: this.estimation.averageActorCount,
        complexActorCount: this.estimation.complexActorCount,
        notes: this.estimation.notes || ''
      };
    }
    document.body.style.overflow = 'hidden';
  }

  ngOnDestroy() {
    document.body.style.overflow = 'auto';
  }

  calculateUAW(): number {
    return (
      this.formData.simpleActorCount * this.weights.actor.simple +
      this.formData.averageActorCount * this.weights.actor.average +
      this.formData.complexActorCount * this.weights.actor.complex
    );
  }

  calculateUUCW(): number {
    return (
      this.formData.simpleUccCount * this.weights.useCase.simple +
      this.formData.averageUccCount * this.weights.useCase.average +
      this.formData.complexUccCount * this.weights.useCase.complex
    );
  }

  calculateUUCP(): number {
    return this.calculateUAW() + this.calculateUUCW();
  }

  onSubmit() {
    this.error.set(null);
    
    if (!this.formData.estimationName || 
        this.formData.simpleUccCount < 0 || 
        this.formData.averageUccCount < 0 ||
        this.formData.complexUccCount < 0 ||
        this.formData.simpleActorCount < 0 ||
        this.formData.averageActorCount < 0 ||
        this.formData.complexActorCount < 0) {
      return;
    }

    const data = {
      estimationName: this.formData.estimationName.trim(),
      simpleUccCount: this.formData.simpleUccCount,
      averageUccCount: this.formData.averageUccCount,
      complexUccCount: this.formData.complexUccCount,
      simpleActorCount: this.formData.simpleActorCount,
      averageActorCount: this.formData.averageActorCount,
      complexActorCount: this.formData.complexActorCount,
      notes: this.formData.notes?.trim() || undefined
    };

    this.saving.set(true);
    this.save.emit(data);
  }

  onCancel() {
    this.cancel.emit();
  }
}
