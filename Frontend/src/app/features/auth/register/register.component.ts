import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { RegisterRequest } from '../../../core/models/auth.models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="register-container">
      <div class="back-button">
        <a routerLink="/home" class="btn btn-secondary">← Regreso</a>
      </div>
      <div class="register-card">
        <h2>Crear Cuenta</h2>
        <p class="subtitle">Únete a la herramienta de estimación de proyectos</p>

        <form [formGroup]="registerForm" (ngSubmit)="onSubmit()" class="register-form">
          <div class="form-group">
            <label for="firstName">Nombre</label>
            <input
              type="text"
              id="firstName"
              formControlName="firstName"
              placeholder="Ingresa tu nombre"
              class="form-control"
              [class.error]="isFieldInvalid('firstName')"
            />
            <div class="error-message" *ngIf="isFieldInvalid('firstName')">
              {{ getFieldError('firstName') }}
            </div>
          </div>

          <div class="form-group">
            <label for="lastName">Apellido</label>
            <input
              type="text"
              id="lastName"
              formControlName="lastName"
              placeholder="Ingresa tu apellido"
              class="form-control"
              [class.error]="isFieldInvalid('lastName')"
            />
            <div class="error-message" *ngIf="isFieldInvalid('lastName')">
              {{ getFieldError('lastName') }}
            </div>
          </div>

          <div class="form-group">
            <label for="email">Correo</label>
            <input
              type="email"
              id="email"
              formControlName="email"
              placeholder="Ingresa tu correo"
              class="form-control"
              [class.error]="isFieldInvalid('email')"
            />
            <div class="error-message" *ngIf="isFieldInvalid('email')">
              {{ getFieldError('email') }}
            </div>
          </div>

          <div class="form-group">
            <label for="password">Contraseña</label>
            <input
              type="password"
              id="password"
              formControlName="password"
              placeholder="Crea una contraseña (mín 6 caracteres)"
              class="form-control"
              [class.error]="isFieldInvalid('password')"
            />
            <div class="error-message" *ngIf="isFieldInvalid('password')">
              {{ getFieldError('password') }}
            </div>
          </div>

          <div class="form-group">
            <label for="confirmPassword">Confirmar Contraseña</label>
            <input
              type="password"
              id="confirmPassword"
              formControlName="confirmPassword"
              placeholder="Confirma tu contraseña"
              class="form-control"
              [class.error]="isFieldInvalid('confirmPassword')"
            />
            <div class="error-message" *ngIf="isFieldInvalid('confirmPassword')">
              {{ getFieldError('confirmPassword') }}
            </div>
          </div>

          <button
            type="submit"
            class="btn btn-primary"
            [disabled]="registerForm.invalid || isLoading"
          >
            <span *ngIf="isLoading">Creando cuenta...</span>
            <span *ngIf="!isLoading">Crear Cuenta</span>
          </button>
        </form>

        <div class="alert alert-error" *ngIf="errorMessage">
          {{ errorMessage }}
        </div>

        <div class="auth-links">
          <p>¿Ya tienes cuenta?
            <a routerLink="../login" class="link">Iniciar Sesión</a>
          </p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .register-container {
      min-height: 100vh;
      /* very light blue background */
      background: #f0f7ff;
      padding: 1rem;
      position: relative;
    }

    .back-button {
      position: absolute;
      top: 1rem;
      left: 1rem;
      z-index: 10;
    }

    .register-card {
      background: white;
      border-radius: 12px;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
      padding: 2rem;
      width: 100%;
      max-width: 400px;
      margin: 4rem auto 0;
      border: 1px solid #e1e5e9;
    }

    h2 {
      text-align: center;
      margin-bottom: 0.5rem;
      color: #333;
      font-weight: 600;
    }

    .subtitle {
      text-align: center;
      margin-bottom: 2rem;
      color: #666;
      font-size: 0.9rem;
    }

    .register-form {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    label {
      font-weight: 500;
      color: #555;
    }

    .form-control {
      padding: 0.75rem;
      border: 2px solid #e1e5e9;
      border-radius: 8px;
      font-size: 1rem;
      transition: border-color 0.3s ease;
    }

    .form-control:focus {
      outline: none;
      border-color: #2563eb;
    }

    .form-control.error {
      border-color: #e74c3c;
    }

    .error-message {
      color: #e74c3c;
      font-size: 0.875rem;
      margin-top: -0.25rem;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 8px;
      font-size: 1rem;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.3s ease;
      text-decoration: none;
      display: inline-block;
    }

    .btn-primary {
      background: #2563eb;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background: #1d4ed8;
      transform: translateY(-1px);
    }

    .btn-secondary {
      background: white;
      color: #2563eb;
      border: 2px solid #2563eb;
    }

    .btn-secondary:hover {
      background: #2563eb;
      color: white;
    }

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
      transform: none;
    }

    .alert {
      padding: 0.75rem;
      border-radius: 8px;
      margin-top: 1rem;
    }

    .alert-error {
      background-color: #fee;
      border: 1px solid #fcc;
      color: #c66;
    }

    .auth-links {
      text-align: center;
      margin-top: 2rem;
      padding-top: 1rem;
      border-top: 1px solid #e1e5e9;
    }

    .link {
      color: #2563eb;
      text-decoration: none;
      font-weight: 500;
    }

    .link:hover {
      text-decoration: underline;
    }
  `]
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  registerForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  constructor() {
    this.registerForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, {
      validators: this.passwordMatchValidator
    });
  }

  passwordMatchValidator(group: FormGroup): any {
    const password = group.get('password');
    const confirmPassword = group.get('confirmPassword');

    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ mismatch: true });
    } else if (confirmPassword?.hasError('mismatch')) {
      const errors = { ...confirmPassword.errors };
      delete errors['mismatch'];
      confirmPassword.setErrors(Object.keys(errors).length ? errors : null);
    }

    return null;
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';

      const { confirmPassword, ...registerData } = this.registerForm.value;
      const registerRequest: RegisterRequest = registerData;

      this.authService.register(registerRequest).subscribe({
        next: (user) => {
          this.isLoading = false;
          // Navigate to dashboard or home
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.message || 'Registration failed. Please try again.';
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.registerForm.controls).forEach(key => {
      const control = this.registerForm.get(key);
      control?.markAsTouched();
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.registerForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  getFieldError(fieldName: string): string {
    const field = this.registerForm.get(fieldName);
    if (field && field.errors && field.touched) {
      const display = this.getFieldDisplayName(fieldName);
      if (field.errors['required']) {
        return `${display} es obligatorio`;
      }
      if (field.errors['email']) {
        return 'Por favor ingresa un correo válido';
      }
      if (field.errors['minlength']) {
        return `${display} debe tener al menos ${field.errors['minlength'].requiredLength} caracteres`;
      }
      if (field.errors['maxlength']) {
        return `${display} no puede exceder ${field.errors['maxlength'].requiredLength} caracteres`;
      }
      if (field.errors['mismatch']) {
        return 'Las contraseñas no coinciden';
      }
    }
    return '';
  }

  private getFieldDisplayName(fieldName: string): string {
    const names: { [key: string]: string } = {
      firstName: 'Nombre',
      lastName: 'Apellido',
      email: 'Correo',
      password: 'Contraseña',
      confirmPassword: 'Confirmar contraseña'
    };
    return names[fieldName] || fieldName;
  }
}