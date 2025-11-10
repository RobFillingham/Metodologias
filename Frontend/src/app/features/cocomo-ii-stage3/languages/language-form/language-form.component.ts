import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { LanguageService, CreateLanguageDto, UpdateLanguageDto } from '../../../../core/services/cocomo-ii-stage3/language.service';
import { Language, ApiResponse } from '../../../../core/models/cocomo-ii-stage3/cocomo-ii-stage3.models';
import { NavbarComponent } from '../../../../shared/components/navbar/navbar.component';

@Component({
  selector: 'app-language-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NavbarComponent],
  template: `
    <app-navbar></app-navbar>

    <div class="form-container">
      <!-- Loading State -->
      <div *ngIf="loading()" class="loading-state">
        <div class="spinner"></div>
        <p>{{ isEditMode() ? 'Cargando lenguaje...' : 'Procesando...' }}</p>
      </div>

      <!-- Error State -->
      <div *ngIf="error()" class="error-alert">
        <strong>⚠️ Error:</strong> {{ error() }}
        <button class="btn btn-sm btn-outline-primary ms-2" (click)="clearError()">
          Cerrar
        </button>
      </div>

      <!-- Form -->
      <div *ngIf="!loading()" class="form-content">
        <div class="form-header">
          <h1>{{ isEditMode() ? '✏️ Editar Lenguaje' : '➕ Crear Lenguaje' }}</h1>
          <p>{{ isEditMode() ? 'Modifica los datos del lenguaje de programación' : 'Agrega un nuevo lenguaje de programación al sistema' }}</p>
        </div>

        <form [formGroup]="languageForm" (ngSubmit)="onSubmit()" class="language-form">
          <!-- Language Name -->
          <div class="form-group">
            <label for="name" class="form-label">
              Nombre del Lenguaje <span class="required">*</span>
            </label>
            <input
              type="text"
              id="name"
              formControlName="name"
              class="form-control"
              placeholder="Ej: Python, Java, C#"
              [class.is-invalid]="isFieldInvalid('name')"
            />
            <div class="invalid-feedback" *ngIf="isFieldInvalid('name')">
              <span *ngIf="languageForm.get('name')?.errors?.['required']">
                El nombre del lenguaje es requerido
              </span>
              <span *ngIf="languageForm.get('name')?.errors?.['minlength']">
                El nombre debe tener al menos 1 carácter
              </span>
              <span *ngIf="languageForm.get('name')?.errors?.['maxlength']">
                El nombre no puede exceder 100 caracteres
              </span>
            </div>
          </div>

          <!-- SLOC per UFP -->
          <div class="form-group">
            <label for="slocPerUfp" class="form-label">
              SLOC por UFP <span class="required">*</span>
            </label>
            <input
              type="number"
              id="slocPerUfp"
              formControlName="slocPerUfp"
              class="form-control"
              placeholder="Ej: 53"
              step="0.1"
              min="0.1"
              max="10000"
              [class.is-invalid]="isFieldInvalid('slocPerUfp')"
            />
            <small class="form-text">
              Factor de conversión de Puntos de Función sin Ajustar a Líneas de Código Fuente
            </small>
            <div class="invalid-feedback" *ngIf="isFieldInvalid('slocPerUfp')">
              <span *ngIf="languageForm.get('slocPerUfp')?.errors?.['required']">
                El factor SLOC por UFP es requerido
              </span>
              <span *ngIf="languageForm.get('slocPerUfp')?.errors?.['min']">
                El valor debe ser mayor o igual a 0.1
              </span>
              <span *ngIf="languageForm.get('slocPerUfp')?.errors?.['max']">
                El valor no puede exceder 10000
              </span>
            </div>
          </div>

          <!-- Info Box -->
          <div class="info-box">
            <div class="info-icon">ℹ️</div>
            <div class="info-content">
              <h4>¿Qué es SLOC por UFP?</h4>
              <p>
                <strong>SLOC</strong> (Líneas de Código Fuente) por <strong>UFP</strong> (Punto de Función sin Ajustar)
                es un factor que indica cuántas líneas de código se necesitan típicamente para implementar
                un punto de función en un lenguaje específico.
              </p>
              <p>
                Por ejemplo, un valor de <strong>53</strong> significa que se necesitan aproximadamente 53 líneas
                de código para implementar 1 punto de función.
              </p>
            </div>
          </div>

          <!-- Form Actions -->
          <div class="form-actions">
            <button type="button" class="btn btn-secondary" (click)="cancel()">
              <i class="bi bi-x-lg"></i>
              Cancelar
            </button>
            <button type="submit" class="btn btn-primary" [disabled]="!languageForm.valid || submitting()">
              <i class="bi bi-check-lg"></i>
              {{ isEditMode() ? 'Actualizar' : 'Crear' }} Lenguaje
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .form-container {
      min-height: 100vh;
      background: #f8f9fa;
      padding: 2rem;
    }

    .loading-state {
      text-align: center;
      padding: 3rem;
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .spinner {
      border: 3px solid #f3f3f3;
      border-top: 3px solid #667eea;
      border-radius: 50%;
      width: 40px;
      height: 40px;
      animation: spin 1s linear infinite;
      margin: 0 auto 1rem;
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
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .form-content {
      max-width: 800px;
      margin: 0 auto;
    }

    .form-header {
      background: white;
      padding: 2rem;
      border-radius: 12px;
      margin-bottom: 2rem;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .form-header h1 {
      margin: 0 0 0.5rem 0;
      color: #333;
      font-size: 2rem;
    }

    .form-header p {
      margin: 0;
      color: #666;
    }

    .language-form {
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .form-group {
      margin-bottom: 1.5rem;
    }

    .form-label {
      display: block;
      margin-bottom: 0.5rem;
      color: #333;
      font-weight: 600;
      font-size: 0.95rem;
    }

    .required {
      color: #dc3545;
    }

    .form-control {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #dee2e6;
      border-radius: 6px;
      font-size: 0.95rem;
      transition: border-color 0.2s;
    }

    .form-control:focus {
      outline: none;
      border-color: #667eea;
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    }

    .form-control.is-invalid {
      border-color: #dc3545;
    }

    .form-control.is-invalid:focus {
      box-shadow: 0 0 0 3px rgba(220, 53, 69, 0.1);
    }

    .form-text {
      display: block;
      margin-top: 0.25rem;
      color: #6c757d;
      font-size: 0.875rem;
    }

    .invalid-feedback {
      display: block;
      margin-top: 0.25rem;
      color: #dc3545;
      font-size: 0.875rem;
    }

    .info-box {
      display: flex;
      gap: 1rem;
      background: #e3f2fd;
      border-left: 4px solid #1976d2;
      padding: 1.5rem;
      border-radius: 8px;
      margin-bottom: 2rem;
    }

    .info-icon {
      font-size: 1.5rem;
      flex-shrink: 0;
    }

    .info-content h4 {
      margin: 0 0 0.5rem 0;
      color: #1976d2;
      font-size: 1rem;
    }

    .info-content p {
      margin: 0 0 0.5rem 0;
      color: #555;
      font-size: 0.9rem;
      line-height: 1.6;
    }

    .info-content p:last-child {
      margin-bottom: 0;
    }

    .form-actions {
      display: flex;
      gap: 1rem;
      justify-content: flex-end;
      padding-top: 1rem;
      border-top: 1px solid #e1e5e9;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: 1px solid #dee2e6;
      border-radius: 6px;
      font-size: 0.95rem;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s;
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
      background: white;
    }

    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .btn-primary {
      background: #667eea;
      color: white;
      border-color: #667eea;
    }

    .btn-primary:hover:not(:disabled) {
      background: #5568d3;
      border-color: #5568d3;
    }

    .btn-secondary {
      color: #6c757d;
      border-color: #6c757d;
    }

    .btn-secondary:hover {
      background: #6c757d;
      color: white;
    }

    .btn-outline-primary {
      color: #667eea;
      border-color: #667eea;
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
      .form-container {
        padding: 1rem;
      }

      .form-actions {
        flex-direction: column;
      }

      .btn {
        width: 100%;
        justify-content: center;
      }
    }
  `]
})
export class LanguageFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private languageService = inject(LanguageService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  languageForm!: FormGroup;
  loading = signal(false);
  submitting = signal(false);
  error = signal<string | null>(null);
  isEditMode = signal(false);
  languageId = signal<number | null>(null);

  ngOnInit() {
    this.initForm();
    
    const id = this.route.snapshot.params['id'];
    if (id && id !== 'new') {
      this.isEditMode.set(true);
      this.languageId.set(+id);
      this.loadLanguage(+id);
    }
  }

  initForm() {
    this.languageForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(100)]],
      slocPerUfp: [null, [Validators.required, Validators.min(0.1), Validators.max(10000)]]
    });
  }

  loadLanguage(id: number) {
    this.loading.set(true);
    this.error.set(null);

    this.languageService.getLanguage(id).subscribe({
      next: (response: ApiResponse<Language>) => {
        this.loading.set(false);
        if (response.success && response.data) {
          this.languageForm.patchValue({
            name: response.data.name,
            slocPerUfp: response.data.slocPerUfp
          });
        } else {
          this.error.set(response.errors?.[0] || 'Error al cargar el lenguaje');
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set('Error al cargar el lenguaje. Por favor intenta de nuevo.');
        console.error('Error loading language:', err);
      }
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.languageForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  clearError() {
    this.error.set(null);
  }

  onSubmit() {
    if (this.languageForm.invalid) {
      Object.keys(this.languageForm.controls).forEach(key => {
        this.languageForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.submitting.set(true);
    this.error.set(null);

    const formValue = this.languageForm.value;
    
    if (this.isEditMode()) {
      this.updateLanguage(formValue);
    } else {
      this.createLanguage(formValue);
    }
  }

  createLanguage(dto: CreateLanguageDto) {
    this.languageService.createLanguage(dto).subscribe({
      next: (response: ApiResponse<Language>) => {
        this.submitting.set(false);
        if (response.success) {
          this.router.navigate(['/cocomo-ii-stage3/languages']);
        } else {
          this.error.set(response.errors?.[0] || 'Error al crear el lenguaje');
        }
      },
      error: (err) => {
        this.submitting.set(false);
        this.error.set(err.error?.errors?.[0] || 'Error al crear el lenguaje. Por favor intenta de nuevo.');
        console.error('Error creating language:', err);
      }
    });
  }

  updateLanguage(dto: UpdateLanguageDto) {
    const id = this.languageId();
    if (!id) return;

    this.languageService.updateLanguage(id, dto).subscribe({
      next: (response: ApiResponse<Language>) => {
        this.submitting.set(false);
        if (response.success) {
          this.router.navigate(['/cocomo-ii-stage3/languages']);
        } else {
          this.error.set(response.errors?.[0] || 'Error al actualizar el lenguaje');
        }
      },
      error: (err) => {
        this.submitting.set(false);
        this.error.set(err.error?.errors?.[0] || 'Error al actualizar el lenguaje. Por favor intenta de nuevo.');
        console.error('Error updating language:', err);
      }
    });
  }

  cancel() {
    this.router.navigate(['/cocomo-ii-stage3/languages']);
  }
}
