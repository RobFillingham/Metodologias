import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EstimationService } from '../../../../core/services/cocomo-ii-stage3/estimation.service';
import { LanguageService } from '../../../../core/services/cocomo-ii-stage3/language.service';
import { ParameterSetService } from '../../../../core/services/cocomo-ii-stage3/parameter-set.service';
import { Language, ParameterSet, CreateEstimationRequest, ApiResponse } from '../../../../core/models/cocomo-ii-stage3/cocomo-ii-stage3.models';

type RatingValue = 'VLO' | 'LO' | 'NOM' | 'HI' | 'VHI' | 'XHI' | 'XLO';

interface RatingOption {
  value: RatingValue;
  label: string;
  description: string;
}

@Component({
  selector: 'app-estimation-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="modal-overlay" (click)="onCancel()">
      <div class="modal-content" (click)="$event.stopPropagation()">
        <!-- Header -->
        <div class="modal-header">
          <h2>{{ isEdit ? 'Editar' : 'Nueva' }} Estimación</h2>
          <button class="btn-close" (click)="onCancel()" aria-label="Cerrar">×</button>
        </div>

        <!-- Loading State -->
        <div *ngIf="loadingData()" class="loading-state">
          <div class="spinner"></div>
          <p>Cargando configuración...</p>
        </div>

        <!-- Form -->
        <form *ngIf="!loadingData()" [formGroup]="estimationForm" (ngSubmit)="onSubmit()">
          <div class="modal-body">
            <!-- Step Indicator -->
            <div class="steps">
              <div class="step" [class.active]="currentStep === 1" [class.completed]="currentStep > 1">
                <span class="step-number">1</span>
                <span class="step-label">Información Básica</span>
              </div>
              <div class="step" [class.active]="currentStep === 2" [class.completed]="currentStep > 2">
                <span class="step-number">2</span>
                <span class="step-label">Configuración</span>
              </div>
              <div class="step" [class.active]="currentStep === 3">
                <span class="step-number">3</span>
                <span class="step-label">Calificaciones (Opcional)</span>
              </div>
            </div>

            <!-- Step 1: Basic Info -->
            <div *ngIf="currentStep === 1" class="step-content">
              <div class="form-group">
                <label for="estimationName">Nombre de la Estimación *</label>
                <input
                  type="text"
                  id="estimationName"
                  formControlName="estimationName"
                  class="form-control"
                  placeholder="ej., Estimación Inicial V1.0"
                  [class.error]="estimationForm.get('estimationName')?.invalid && estimationForm.get('estimationName')?.touched"
                >
                <div class="char-counter">
                  {{ estimationForm.get('estimationName')?.value?.length || 0 }} / 255
                </div>
                <div 
                  *ngIf="estimationForm.get('estimationName')?.invalid && estimationForm.get('estimationName')?.touched"
                  class="error-message"
                >
                  El nombre de la estimación es requerido (máx. 255 caracteres)
                </div>
              </div>

              <div class="info-box">
                <strong>💡 Consejo:</strong> Elige un nombre descriptivo que identifique la versión o propósito de esta estimación.
              </div>
            </div>

            <!-- Step 2: Configuration -->
            <div *ngIf="currentStep === 2" class="step-content">
              <div class="form-group">
                <label for="languageId">Lenguaje de Programación *</label>
                <select
                  id="languageId"
                  formControlName="languageId"
                  class="form-control"
                  [class.error]="estimationForm.get('languageId')?.invalid && estimationForm.get('languageId')?.touched"
                >
                  <option value="">Selecciona un lenguaje</option>
                  <option *ngFor="let lang of languages()" [value]="lang.languageId">
                    {{ lang.name }} ({{ lang.slocPerUfp }} SLOC/UFP)
                  </option>
                </select>
                <div 
                  *ngIf="estimationForm.get('languageId')?.invalid && estimationForm.get('languageId')?.touched"
                  class="error-message"
                >
                  Por favor selecciona un lenguaje de programación
                </div>
              </div>

              <div class="form-group">
                <label for="paramSetId">Conjunto de Parámetros *</label>
                <select
                  id="paramSetId"
                  formControlName="paramSetId"
                  class="form-control"
                  [class.error]="estimationForm.get('paramSetId')?.invalid && estimationForm.get('paramSetId')?.touched"
                >
                  <option value="">Selecciona un conjunto de parámetros</option>
                  <option *ngFor="let ps of parameterSets()" [value]="ps.paramSetId">
                    {{ ps.setName }} {{ ps.isDefault ? '(Por defecto)' : '' }}
                  </option>
                </select>
                <div 
                  *ngIf="estimationForm.get('paramSetId')?.invalid && estimationForm.get('paramSetId')?.touched"
                  class="error-message"
                >
                  Por favor selecciona un conjunto de parámetros
                </div>
              </div>

              <div class="info-box">
                <strong>ℹ️ Info:</strong> El lenguaje determina el factor de conversión SLOC. El conjunto de parámetros define los coeficientes de COCOMO II.
              </div>
            </div>

            <!-- Step 3: Ratings -->
            <div *ngIf="currentStep === 3" class="step-content">
              <div class="ratings-intro">
                <p>Personaliza las calificaciones de Factores de Escala (SF) y Multiplicadores de Esfuerzo (EM) para esta estimación.</p>
                <p class="text-muted">Todas las calificaciones tienen como valor predeterminado NOM (Nominal). Puedes ajustarlas ahora o más tarde.</p>
              </div>

              <!-- Scale Factors -->
              <div class="rating-section">
                <h3>Factores de Escala (SF)</h3>
                
                <div class="form-group">
                  <label>PREC - Precedencia</label>
                  <select formControlName="selectedSfPrec" class="form-control">
                    <option *ngFor="let opt of sfRatings" [value]="opt.value">
                      {{ opt.label }} - {{ opt.description }}
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label>FLEX - Flexibilidad de Desarrollo</label>
                  <select formControlName="selectedSfFlex" class="form-control">
                    <option *ngFor="let opt of sfRatings" [value]="opt.value">
                      {{ opt.label }} - {{ opt.description }}
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label>RESL - Resolución de Arquitectura/Riesgo</label>
                  <select formControlName="selectedSfResl" class="form-control">
                    <option *ngFor="let opt of sfRatings" [value]="opt.value">
                      {{ opt.label }} - {{ opt.description }}
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label>TEAM - Cohesión del Equipo</label>
                  <select formControlName="selectedSfTeam" class="form-control">
                    <option *ngFor="let opt of sfRatings" [value]="opt.value">
                      {{ opt.label }} - {{ opt.description }}
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label>PMAT - Madurez del Proceso</label>
                  <select formControlName="selectedSfPmat" class="form-control">
                    <option *ngFor="let opt of sfRatings" [value]="opt.value">
                      {{ opt.label }} - {{ opt.description }}
                    </option>
                  </select>
                </div>
              </div>

              <!-- Effort Multipliers -->
              <div class="rating-section">
                <h3>Multiplicadores de Esfuerzo (EM)</h3>
                
                <div class="form-group">
                  <label>PERS - Capacidad del Personal</label>
                  <select formControlName="selectedEmPers" class="form-control">
                    <option *ngFor="let opt of emRatings" [value]="opt.value">
                      {{ opt.label }} - {{ opt.description }}
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label>RCPX - Confiabilidad y Complejidad del Producto</label>
                  <select formControlName="selectedEmRcpx" class="form-control">
                    <option *ngFor="let opt of emRatings" [value]="opt.value">
                      {{ opt.label }} - {{ opt.description }}
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label>PDIF - Dificultad de la Plataforma</label>
                  <select formControlName="selectedEmPdif" class="form-control">
                    <option *ngFor="let opt of emRatings" [value]="opt.value">
                      {{ opt.label }} - {{ opt.description }}
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label>PREX - Experiencia del Personal</label>
                  <select formControlName="selectedEmPrex" class="form-control">
                    <option *ngFor="let opt of emRatings" [value]="opt.value">
                      {{ opt.label }} - {{ opt.description }}
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label>RUSE - Reusabilidad Requerida</label>
                  <select formControlName="selectedEmRuse" class="form-control">
                    <option *ngFor="let opt of emRatings" [value]="opt.value">
                      {{ opt.label }} - {{ opt.description }}
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label>FCIL - Facilidades</label>
                  <select formControlName="selectedEmFcil" class="form-control">
                    <option *ngFor="let opt of emRatings" [value]="opt.value">
                      {{ opt.label }} - {{ opt.description }}
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label>SCED - Restricción de Cronograma</label>
                  <select formControlName="selectedEmSced" class="form-control">
                    <option *ngFor="let opt of emRatings" [value]="opt.value">
                      {{ opt.label }} - {{ opt.description }}
                    </option>
                  </select>
                </div>
              </div>
            </div>

            <!-- Error Message -->
            <div *ngIf="error()" class="error-alert">
              <strong>⚠️ Error:</strong> {{ error() }}
            </div>
          </div>

          <!-- Footer -->
          <div class="modal-footer">
            <button 
              type="button" 
              class="btn btn-secondary" 
              (click)="previousStep()"
              *ngIf="currentStep > 1"
            >
              ← Anterior
            </button>
            <button 
              type="button" 
              class="btn btn-secondary" 
              (click)="onCancel()"
            >
              Cancelar
            </button>
            <button 
              type="button" 
              class="btn btn-primary" 
              (click)="nextStep()"
              *ngIf="currentStep < 3"
              [disabled]="!isCurrentStepValid()"
            >
              Siguiente →
            </button>
            <button 
              type="submit" 
              class="btn btn-primary" 
              *ngIf="currentStep === 3"
              [disabled]="saving() || estimationForm.invalid"
            >
              {{ saving() ? 'Creando...' : 'Crear Estimación' }}
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
      background: rgba(0, 0, 0, 0.5);
      display: flex;
      justify-content: center;
      align-items: center;
      z-index: 1000;
      padding: 1rem;
    }

    .modal-content {
      background: white;
      border-radius: 16px;
      width: 100%;
      max-width: 700px;
      max-height: 90vh;
      display: flex;
      flex-direction: column;
      box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
    }

    .modal-header {
      padding: 2rem;
      border-bottom: 1px solid #e2e8f0;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .modal-header h2 {
      margin: 0;
      color: #333;
      font-size: 1.75rem;
    }

    .btn-close {
      background: none;
      border: none;
      font-size: 2rem;
      color: #999;
      cursor: pointer;
      line-height: 1;
      transition: color 0.2s;
      padding: 0;
      width: 32px;
      height: 32px;
    }

    .btn-close:hover {
      color: #333;
    }

    .loading-state {
      padding: 4rem 2rem;
      text-align: center;
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

    .modal-body {
      padding: 2rem;
      overflow-y: auto;
      flex: 1;
      max-height: calc(90vh - 200px); /* Ensure it doesn't exceed viewport */
    }

    .steps {
      display: flex;
      justify-content: space-between;
      margin-bottom: 2rem;
      position: relative;
    }

    .steps::before {
      content: '';
      position: absolute;
      top: 20px;
      left: 0;
      right: 0;
      height: 2px;
      background: #e2e8f0;
      z-index: 0;
    }

    .step {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 0.5rem;
      position: relative;
      z-index: 1;
      background: white;
      padding: 0 0.5rem;
    }

    .step-number {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      border: 2px solid #e2e8f0;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 600;
      color: #999;
      background: white;
      transition: all 0.3s ease;
    }

    .step.active .step-number {
      border-color: #667eea;
      background: #667eea;
      color: white;
    }

    .step.completed .step-number {
      border-color: #48bb78;
      background: #48bb78;
      color: white;
    }

    .step-label {
      font-size: 0.85rem;
      color: #666;
      text-align: center;
    }

    .step.active .step-label {
      color: #667eea;
      font-weight: 600;
    }

    .step-content {
      animation: fadeIn 0.3s ease;
    }

    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(10px); }
      to { opacity: 1; transform: translateY(0); }
    }

    .form-group {
      margin-bottom: 1.5rem;
    }

    label {
      display: block;
      margin-bottom: 0.5rem;
      color: #333;
      font-weight: 600;
      font-size: 0.95rem;
    }

    .form-control {
      width: 100%;
      padding: 0.75rem;
      border: 2px solid #e2e8f0;
      border-radius: 8px;
      font-size: 1rem;
      transition: all 0.2s;
      font-family: inherit;
    }

    .form-control:focus {
      outline: none;
      border-color: #667eea;
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    }

    .form-control.error {
      border-color: #e53e3e;
    }

    .char-counter {
      text-align: right;
      font-size: 0.85rem;
      color: #999;
      margin-top: 0.25rem;
    }

    .error-message {
      color: #e53e3e;
      font-size: 0.85rem;
      margin-top: 0.5rem;
    }

    .error-alert {
      background: #fee;
      border: 1px solid #fcc;
      color: #c33;
      padding: 1rem;
      border-radius: 8px;
      margin-top: 1rem;
    }

    .info-box {
      background: #e6f2ff;
      border-left: 4px solid #667eea;
      padding: 1rem;
      border-radius: 4px;
      margin-top: 1.5rem;
      font-size: 0.9rem;
      color: #555;
    }

    .ratings-intro {
      margin-bottom: 2rem;
      padding: 1rem;
      background: #f7fafc;
      border-radius: 8px;
    }

    .ratings-intro p {
      margin: 0.5rem 0;
      color: #555;
    }

    .text-muted {
      color: #999 !important;
      font-size: 0.9rem;
    }

    .rating-section {
      margin-bottom: 2rem;
      padding: 1.5rem;
      background: #f7fafc;
      border-radius: 12px;
    }

    .rating-section h3 {
      margin: 0 0 1rem;
      color: #333;
      font-size: 1.1rem;
    }

    .modal-footer {
      padding: 1.5rem 2rem;
      border-top: 1px solid #e2e8f0;
      display: flex;
      gap: 1rem;
      justify-content: flex-end;
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

    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .btn-primary {
      background: #667eea;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background: #5568d3;
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
    }

    .btn-secondary {
      background: transparent;
      color: #667eea;
      border: 2px solid #667eea;
    }

    .btn-secondary:hover:not(:disabled) {
      background: #f7fafc;
    }

    @media (max-width: 768px) {
      .modal-content {
        max-height: 95vh;
      }

      .modal-header,
      .modal-body,
      .modal-footer {
        padding: 1.5rem;
      }

      .steps {
        margin-bottom: 1.5rem;
      }

      .step-label {
        font-size: 0.75rem;
      }

      .modal-footer {
        flex-wrap: wrap;
      }

      .btn {
        flex: 1;
        min-width: 120px;
      }
    }
  `]
})
export class EstimationFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private estimationService = inject(EstimationService);
  private languageService = inject(LanguageService);
  private parameterSetService = inject(ParameterSetService);

  @Input() projectId!: number;
  @Input() isEdit = false;
  @Output() formSubmitted = new EventEmitter<void>();
  @Output() formCancelled = new EventEmitter<void>();

  estimationForm!: FormGroup;
  currentStep = 1;

  languages = signal<Language[]>([]);
  parameterSets = signal<ParameterSet[]>([]);
  loadingData = signal(false);
  saving = signal(false);
  error = signal<string | null>(null);

  sfRatings: RatingOption[] = [
    { value: 'VLO', label: 'Muy Bajo', description: 'Impacto mínimo' },
    { value: 'LO', label: 'Bajo', description: 'Impacto bajo' },
    { value: 'NOM', label: 'Nominal', description: 'Promedio' },
    { value: 'HI', label: 'Alto', description: 'Impacto alto' },
    { value: 'VHI', label: 'Muy Alto', description: 'Impacto muy alto' },
    { value: 'XHI', label: 'Extra Alto', description: 'Impacto máximo' }
  ];

  emRatings: RatingOption[] = [
    { value: 'XLO', label: 'Extra Bajo', description: 'Esfuerzo mínimo' },
    { value: 'VLO', label: 'Muy Bajo', description: 'Esfuerzo muy bajo' },
    { value: 'LO', label: 'Bajo', description: 'Esfuerzo bajo' },
    { value: 'NOM', label: 'Nominal', description: 'Esfuerzo promedio' },
    { value: 'HI', label: 'Alto', description: 'Esfuerzo alto' },
    { value: 'VHI', label: 'Muy Alto', description: 'Esfuerzo muy alto' },
    { value: 'XHI', label: 'Extra Alto', description: 'Esfuerzo máximo' }
  ];

  ngOnInit() {
    this.initForm();
    this.loadConfiguration();
  }

  initForm() {
    this.estimationForm = this.fb.group({
      estimationName: ['', [Validators.required, Validators.maxLength(255)]],
      languageId: ['', Validators.required],
      paramSetId: ['', Validators.required],
      selectedSfPrec: ['NOM'],
      selectedSfFlex: ['NOM'],
      selectedSfResl: ['NOM'],
      selectedSfTeam: ['NOM'],
      selectedSfPmat: ['NOM'],
      selectedEmPers: ['NOM'],
      selectedEmRcpx: ['NOM'],
      selectedEmPdif: ['NOM'],
      selectedEmPrex: ['NOM'],
      selectedEmRuse: ['NOM'],
      selectedEmFcil: ['NOM'],
      selectedEmSced: ['NOM']
    });
  }

  loadConfiguration() {
    this.loadingData.set(true);
    this.error.set(null);

    Promise.all([
      this.loadLanguages(),
      this.loadParameterSets()
    ]).then(() => {
      this.loadingData.set(false);
    }).catch(err => {
      this.error.set('Failed to load configuration data');
      this.loadingData.set(false);
    });
  }

  loadLanguages(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.languageService.getLanguages().subscribe({
        next: (response: ApiResponse<Language[]>) => {
          if (response.success && response.data) {
            this.languages.set(response.data);
            resolve();
          } else {
            reject(new Error('Failed to load languages'));
          }
        },
        error: reject
      });
    });
  }

  loadParameterSets(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.parameterSetService.getAllParameterSets().subscribe({
        next: (response: ApiResponse<ParameterSet[]>) => {
          if (response.success && response.data) {
            this.parameterSets.set(response.data);
            // Auto-select default parameter set if available
            const defaultSet = response.data.find(ps => ps.isDefault);
            if (defaultSet) {
              this.estimationForm.patchValue({ paramSetId: defaultSet.paramSetId });
            }
            resolve();
          } else {
            reject(new Error('Failed to load parameter sets'));
          }
        },
        error: reject
      });
    });
  }

  isCurrentStepValid(): boolean {
    switch (this.currentStep) {
      case 1:
        return this.estimationForm.get('estimationName')?.valid || false;
      case 2:
        return this.estimationForm.get('languageId')?.valid && 
               this.estimationForm.get('paramSetId')?.valid || false;
      case 3:
        return true; // Ratings are optional
      default:
        return false;
    }
  }

  nextStep() {
    if (this.currentStep < 3 && this.isCurrentStepValid()) {
      this.currentStep++;
    }
  }

  previousStep() {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  onSubmit() {
    if (this.estimationForm.invalid || this.saving()) {
      return;
    }

    this.saving.set(true);
    this.error.set(null);

    const formValue = this.estimationForm.value;
    const request: CreateEstimationRequest = {
      estimationName: formValue.estimationName,
      paramSetId: +formValue.paramSetId,
      languageId: +formValue.languageId,
      selectedSfPrec: formValue.selectedSfPrec,
      selectedSfFlex: formValue.selectedSfFlex,
      selectedSfResl: formValue.selectedSfResl,
      selectedSfTeam: formValue.selectedSfTeam,
      selectedSfPmat: formValue.selectedSfPmat,
      selectedEmRely: formValue.selectedEmRely,
      selectedEmData: formValue.selectedEmData,
      selectedEmCplx: formValue.selectedEmCplx,
      selectedEmRuse: formValue.selectedEmRuse,
      selectedEmDocu: formValue.selectedEmDocu,
      selectedEmTime: formValue.selectedEmTime,
      selectedEmStor: formValue.selectedEmStor,
      selectedEmPvol: formValue.selectedEmPvol,
      selectedEmAcap: formValue.selectedEmAcap,
      selectedEmPcap: formValue.selectedEmPcap,
      selectedEmPcon: formValue.selectedEmPcon,
      selectedEmApex: formValue.selectedEmApex,
      selectedEmPlex: formValue.selectedEmPlex,
      selectedEmLtex: formValue.selectedEmLtex,
      selectedEmTool: formValue.selectedEmTool,
      selectedEmSite: formValue.selectedEmSite,
      selectedEmSced: formValue.selectedEmSced
    };

    this.estimationService.createEstimation(this.projectId, request).subscribe({
      next: (response: ApiResponse<any>) => {
        if (response.success) {
          this.saving.set(false);
          this.formSubmitted.emit();
        } else {
          this.error.set(response.message || 'Failed to create estimation');
          this.saving.set(false);
        }
      },
      error: (err: any) => {
        this.error.set(err.message || 'An error occurred while creating the estimation');
        this.saving.set(false);
      }
    });
  }

  onCancel() {
    this.formCancelled.emit();
  }
}
