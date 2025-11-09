import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { LoginRequest } from '../../../core/models/auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="login-container">
      <div class="back-button">
        <a routerLink="/home" class="btn btn-secondary">← Regreso</a>
      </div>
      <div class="login-card">
        <h2>Iniciar Sesión</h2>

        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="login-form">
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

          <!-- Password Field -->
            <div class="form-group">
              <label for="password">Contraseña</label>
              <input
                type="password"
                id="password"
                formControlName="password"
                placeholder="Ingresa tu contraseña"
                class="form-control"
                [class.error]="isFieldInvalid('password')"
              />
            <div class="error-message" *ngIf="isFieldInvalid('password')">
              {{ getFieldError('password') }}
            </div>
          </div>

          <!-- Submit Button -->
            <button
              type="submit"
              class="btn btn-primary"
              [disabled]="loginForm.invalid || isLoading"
            >
              <span *ngIf="isLoading">Iniciando sesión...</span>
              <span *ngIf="!isLoading">Iniciar Sesión</span>
            </button>
        </form>

        <!-- Error Message -->
        <div class="alert alert-error" *ngIf="errorMessage">
          {{ errorMessage }}
        </div>

        <!-- Register Link -->
        <div class="auth-links">
          <p>¿No tienes cuenta?
              <a routerLink="../register" class="link">Registrarse</a>
          </p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
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

    .login-card {
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
      margin-bottom: 2rem;
      color: #333;
      font-weight: 600;
    }

    .login-form {
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
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  loginForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';

      const loginRequest: LoginRequest = this.loginForm.value;

      this.authService.login(loginRequest).subscribe({
        next: (user) => {
          this.isLoading = false;
          // Navigate to dashboard or home
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.message || 'Login failed. Please try again.';
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.loginForm.controls).forEach(key => {
      const control = this.loginForm.get(key);
      control?.markAsTouched();
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }
  getFieldError(fieldName: string): string {
    const field = this.loginForm.get(fieldName);
    if (field && field.errors && field.touched) {
      const names: { [key: string]: string } = {
        email: 'Correo',
        password: 'Contraseña'
      };
      const display = names[fieldName] || fieldName;

      if (field.errors['required']) {
        return `${display} es obligatorio`;
      }
      if (field.errors['email']) {
        return 'Por favor ingresa un correo válido';
      }
      if (field.errors['minlength']) {
        return `${display} debe tener al menos ${field.errors['minlength'].requiredLength} caracteres`;
      }
    }
    return '';
  }
}