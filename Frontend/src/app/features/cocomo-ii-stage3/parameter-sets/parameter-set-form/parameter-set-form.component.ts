import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ParameterSetService } from '../../../../core/services/cocomo-ii-stage3/parameter-set.service';
import { ParameterSet, CreateParameterSetRequest, ApiResponse } from '../../../../core/models/cocomo-ii-stage3/cocomo-ii-stage3.models';
import { NavbarComponent } from '../../../../shared/components/navbar/navbar.component';

interface RatingDefinition {
  key: string;
  label: string;
  fieldPrefix: string;
  options: { value: string; label: string }[];
}

@Component({
  selector: 'app-parameter-set-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NavbarComponent],
  template: `
    <app-navbar></app-navbar>

    <div class="form-container">
      <div class="form-header">
        <h1>{{ isEdit ? 'Edit' : 'Create' }} Parameter Set</h1>
        <p>{{ isEdit ? 'Modify your COCOMO II parameter configuration' : 'Create a new parameter set for your organization' }}</p>
      </div>

      <!-- Loading State -->
      <div *ngIf="loading()" class="loading-state">
        <div class="spinner"></div>
        <p>Loading parameter set...</p>
      </div>

      <!-- Form -->
      <form *ngIf="!loading()" [formGroup]="parameterSetForm" (ngSubmit)="onSubmit()" class="parameter-set-form">
        <!-- Basic Information -->
        <div class="form-section">
          <h3>Basic Information</h3>

          <div class="form-group">
            <label for="setName">Parameter Set Name *</label>
            <input
              type="text"
              id="setName"
              formControlName="setName"
              class="form-control"
              placeholder="e.g., My Organization Parameters"
              [class.error]="parameterSetForm.get('setName')?.invalid && parameterSetForm.get('setName')?.touched"
            >
            <div class="char-counter">
              {{ parameterSetForm.get('setName')?.value?.length || 0 }} / 100
            </div>
            <div
              *ngIf="parameterSetForm.get('setName')?.invalid && parameterSetForm.get('setName')?.touched"
              class="error-message"
            >
              Parameter set name is required (max. 100 characters)
            </div>
          </div>

          <div class="form-group">
            <label class="checkbox-label">
              <input type="checkbox" formControlName="isDefault">
              Mark as default parameter set
            </label>
            <small class="form-text text-muted">
              Default parameter sets are available to all users in the system
            </small>
          </div>
        </div>

        <!-- COCOMO Constants -->
        <div class="form-section">
          <h3>COCOMO Constants</h3>
          <p class="section-description">
            These constants are used in the COCOMO II effort calculation formula: Effort = A × Size^B × M
          </p>

          <div class="constants-grid">
            <div class="form-group">
              <label for="constA">Constant A *</label>
              <input
                type="number"
                id="constA"
                formControlName="constA"
                class="form-control"
                step="0.01"
                [class.error]="parameterSetForm.get('constA')?.invalid && parameterSetForm.get('constA')?.touched"
              >
              <div
                *ngIf="parameterSetForm.get('constA')?.invalid && parameterSetForm.get('constA')?.touched"
                class="error-message"
              >
                Constant A is required and must be a positive number
              </div>
            </div>

            <div class="form-group">
              <label for="constB">Constant B *</label>
              <input
                type="number"
                id="constB"
                formControlName="constB"
                class="form-control"
                step="0.01"
                [class.error]="parameterSetForm.get('constB')?.invalid && parameterSetForm.get('constB')?.touched"
              >
              <div
                *ngIf="parameterSetForm.get('constB')?.invalid && parameterSetForm.get('constB')?.touched"
                class="error-message"
              >
                Constant B is required and must be a positive number
              </div>
            </div>

            <div class="form-group">
              <label for="constC">Constant C *</label>
              <input
                type="number"
                id="constC"
                formControlName="constC"
                class="form-control"
                step="0.01"
                [class.error]="parameterSetForm.get('constC')?.invalid && parameterSetForm.get('constC')?.touched"
              >
              <div
                *ngIf="parameterSetForm.get('constC')?.invalid && parameterSetForm.get('constC')?.touched"
                class="error-message"
              >
                Constant C is required and must be a positive number
              </div>
            </div>

            <div class="form-group">
              <label for="constD">Constant D *</label>
              <input
                type="number"
                id="constD"
                formControlName="constD"
                class="form-control"
                step="0.01"
                [class.error]="parameterSetForm.get('constD')?.invalid && parameterSetForm.get('constD')?.touched"
              >
              <div
                *ngIf="parameterSetForm.get('constD')?.invalid && parameterSetForm.get('constD')?.touched"
                class="error-message"
              >
                Constant D is required and must be a positive number
              </div>
            </div>
          </div>
        </div>

        <!-- Scale Factors -->
        <div class="form-section">
          <h3>Scale Factors (SF)</h3>
          <p class="section-description">
            Scale factors adjust the effort based on project characteristics. Each factor has 6 rating levels.
          </p>

          <div class="ratings-section">
            <div *ngFor="let sf of scaleFactors" class="rating-group">
              <h4>{{ sf.label }}</h4>
              <div class="rating-grid">
                <div *ngFor="let option of sf.options" class="rating-input">
                  <label [for]="sf.key + option.value">{{ option.label }}</label>
                  <input
                    type="number"
                    [id]="sf.key + option.value"
                    [formControlName]="sf.fieldPrefix + option.value"
                    class="form-control"
                    step="0.01"
                    placeholder="0.00"
                  >
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Effort Multipliers -->
        <div class="form-section">
          <h3>Effort Multipliers (EM)</h3>
          <p class="section-description">
            Effort multipliers adjust the effort based on product, platform, personnel, and project attributes. Each factor has 7 rating levels.
          </p>

          <div class="ratings-section">
            <div *ngFor="let em of effortMultipliers" class="rating-group">
              <h4>{{ em.label }}</h4>
              <div class="rating-grid">
                <div *ngFor="let option of em.options" class="rating-input">
                  <label [for]="em.key + option.value">{{ option.label }}</label>
                  <input
                    type="number"
                    [id]="em.key + option.value"
                    [formControlName]="em.fieldPrefix + option.value"
                    class="form-control"
                    step="0.01"
                    placeholder="0.00"
                  >
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Error Message -->
        <div *ngIf="error()" class="error-alert">
          <strong>⚠️ Error:</strong> {{ error() }}
        </div>

        <!-- Form Actions -->
        <div class="form-actions">
          <button type="button" class="btn btn-secondary" (click)="goBack()">
            Cancel
          </button>
          <button type="submit" class="btn btn-primary" [disabled]="saving() || parameterSetForm.invalid">
            <span *ngIf="!saving()">{{ isEdit ? 'Update' : 'Create' }} Parameter Set</span>
            <span *ngIf="saving()">
              <span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
              {{ isEdit ? 'Updating...' : 'Creating...' }}
            </span>
          </button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .form-container {
      min-height: 100vh;
      background: #f8f9fa;
      padding: 2rem;
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

    .loading-state {
      text-align: center;
      padding: 4rem 2rem;
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
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

    .parameter-set-form {
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .form-section {
      margin-bottom: 3rem;
      padding-bottom: 2rem;
      border-bottom: 1px solid #e1e5e9;
    }

    .form-section:last-child {
      border-bottom: none;
      margin-bottom: 0;
      padding-bottom: 0;
    }

    .form-section h3 {
      margin: 0 0 1rem 0;
      color: #333;
      font-size: 1.5rem;
    }

    .section-description {
      color: #666;
      margin-bottom: 1.5rem;
      line-height: 1.6;
    }

    .form-group {
      margin-bottom: 1.5rem;
    }

    .form-group label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: #333;
    }

    .checkbox-label {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      cursor: pointer;
      font-weight: normal;
    }

    .checkbox-label input[type="checkbox"] {
      margin: 0;
    }

    .form-text {
      font-size: 0.875rem;
      color: #6c757d;
    }

    .form-control {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #dee2e6;
      border-radius: 6px;
      font-size: 1rem;
      transition: border-color 0.2s, box-shadow 0.2s;
    }

    .form-control:focus {
      outline: none;
      border-color: #667eea;
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    }

    .form-control.error {
      border-color: #dc3545;
    }

    .char-counter {
      text-align: right;
      font-size: 0.8rem;
      color: #6c757d;
      margin-top: 0.25rem;
    }

    .error-message {
      color: #dc3545;
      font-size: 0.875rem;
      margin-top: 0.25rem;
    }

    .constants-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1rem;
    }

    .ratings-section {
      display: flex;
      flex-direction: column;
      gap: 2rem;
    }

    .rating-group {
      border: 1px solid #e1e5e9;
      border-radius: 8px;
      padding: 1.5rem;
      background: #f8f9fa;
    }

    .rating-group h4 {
      margin: 0 0 1rem 0;
      color: #333;
      font-size: 1.1rem;
    }

    .rating-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 1rem;
    }

    .rating-input {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .rating-input label {
      font-size: 0.9rem;
      color: #666;
      font-weight: normal;
    }

    .error-alert {
      background: #f8d7da;
      color: #721c24;
      padding: 1rem;
      border-radius: 8px;
      margin-bottom: 2rem;
      border: 1px solid #f5c6cb;
    }

    .form-actions {
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
      padding-top: 2rem;
      border-top: 1px solid #e1e5e9;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: 1px solid #dee2e6;
      border-radius: 6px;
      font-size: 1rem;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s;
      text-decoration: none;
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
    }

    .btn-primary {
      background: #667eea;
      color: white;
      border-color: #667eea;
    }

    .btn-primary:hover:not(:disabled) {
      background: #5a6fd8;
      border-color: #5a6fd8;
    }

    .btn-secondary {
      background: #6c757d;
      color: white;
      border-color: #6c757d;
    }

    .btn-secondary:hover {
      background: #5a6268;
      border-color: #5a6268;
    }

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    @media (max-width: 768px) {
      .form-container {
        padding: 1rem;
      }

      .constants-grid {
        grid-template-columns: 1fr;
      }

      .rating-grid {
        grid-template-columns: 1fr;
      }

      .form-actions {
        flex-direction: column;
      }

      .form-header h1 {
        font-size: 1.5rem;
      }
    }
  `]
})
export class ParameterSetFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private parameterSetService = inject(ParameterSetService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  parameterSetForm!: FormGroup;
  isEdit = false;
  parameterSetId: number | null = null;
  loading = signal(false);
  saving = signal(false);
  error = signal<string | null>(null);

  // Scale Factors definitions
  scaleFactors: RatingDefinition[] = [
    {
      key: 'PREC',
      label: 'Precedentedness (PREC)',
      fieldPrefix: 'sfPrec',
      options: [
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'FLEX',
      label: 'Development Flexibility (FLEX)',
      fieldPrefix: 'sfFlex',
      options: [
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'RESL',
      label: 'Architecture/Risk Resolution (RESL)',
      fieldPrefix: 'sfResl',
      options: [
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'TEAM',
      label: 'Team Cohesion (TEAM)',
      fieldPrefix: 'sfTeam',
      options: [
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'PMAT',
      label: 'Process Maturity (PMAT)',
      fieldPrefix: 'sfPmat',
      options: [
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    }
  ];

  // Effort Multipliers definitions
  effortMultipliers: RatingDefinition[] = [
    {
      key: 'RELY',
      label: 'Required Software Reliability (RELY)',
      fieldPrefix: 'emRely',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'DATA',
      label: 'Database Size (DATA)',
      fieldPrefix: 'emData',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'CPLX',
      label: 'Product Complexity (CPLX)',
      fieldPrefix: 'emCplx',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'RUSE',
      label: 'Required Reusability (RUSE)',
      fieldPrefix: 'emRuse',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'DOCU',
      label: 'Documentation Match to Life-Cycle Needs (DOCU)',
      fieldPrefix: 'emDocu',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'TIME',
      label: 'Execution Time Constraint (TIME)',
      fieldPrefix: 'emTime',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'STOR',
      label: 'Main Storage Constraint (STOR)',
      fieldPrefix: 'emStor',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'PVOL',
      label: 'Platform Volatility (PVOL)',
      fieldPrefix: 'emPvol',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'ACAP',
      label: 'Analyst Capability (ACAP)',
      fieldPrefix: 'emAcap',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'PCAP',
      label: 'Programmer Capability (PCAP)',
      fieldPrefix: 'emPcap',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'PCON',
      label: 'Personnel Continuity (PCON)',
      fieldPrefix: 'emPcon',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'APEX',
      label: 'Applications Experience (APEX)',
      fieldPrefix: 'emApex',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'PLEX',
      label: 'Platform Experience (PLEX)',
      fieldPrefix: 'emPlex',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'LTEX',
      label: 'Language and Tool Experience (LTEX)',
      fieldPrefix: 'emLtex',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'TOOL',
      label: 'Use of Software Tools (TOOL)',
      fieldPrefix: 'emTool',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'SITE',
      label: 'Multi-site Development (SITE)',
      fieldPrefix: 'emSite',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    },
    {
      key: 'SCED',
      label: 'Required Development Schedule (SCED)',
      fieldPrefix: 'emSced',
      options: [
        { value: 'Xlo', label: 'Extra Low' },
        { value: 'Vlo', label: 'Very Low' },
        { value: 'Lo', label: 'Low' },
        { value: 'Nom', label: 'Nominal' },
        { value: 'Hi', label: 'High' },
        { value: 'Vhi', label: 'Very High' },
        { value: 'Xhi', label: 'Extra High' }
      ]
    }
  ];

  ngOnInit() {
    this.initForm();
    this.checkEditMode();
  }

  initForm() {
    this.parameterSetForm = this.fb.group({
      setName: ['', [Validators.required, Validators.maxLength(100)]],
      isDefault: [false],
      constA: [2.94, [Validators.required, Validators.min(0)]],
      constB: [0.91, [Validators.required, Validators.min(0)]],
      constC: [3.67, [Validators.required, Validators.min(0)]],
      constD: [0.28, [Validators.required, Validators.min(0)]]
    });

    // Add form controls for all SF ratings
    this.scaleFactors.forEach(sf => {
      sf.options.forEach(option => {
        this.parameterSetForm.addControl(`${sf.fieldPrefix}${option.value}`, this.fb.control(null));
      });
    });

    // Add form controls for all EM ratings
    this.effortMultipliers.forEach(em => {
      em.options.forEach(option => {
        this.parameterSetForm.addControl(`${em.fieldPrefix}${option.value}`, this.fb.control(null));
      });
    });
  }

  checkEditMode() {
    const id = this.route.snapshot.params['id'];
    const isEdit = this.route.snapshot.url.some(segment => segment.path === 'edit');

    if (id && isEdit) {
      this.isEdit = true;
      this.parameterSetId = +id;
      this.loadParameterSet(this.parameterSetId);
    }
  }

  loadParameterSet(id: number) {
    this.loading.set(true);
    this.parameterSetService.getParameterSet(id).subscribe({
      next: (response) => {
        this.loading.set(false);
        if (response.success && response.data) {
          this.populateForm(response.data);
        } else {
          this.error.set(response.errors?.[0] || 'Failed to load parameter set');
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set('Failed to load parameter set. Please try again.');
        console.error('Error loading parameter set:', err);
      }
    });
  }

  populateForm(parameterSet: ParameterSet) {
    this.parameterSetForm.patchValue({
      setName: parameterSet.setName,
      isDefault: parameterSet.isDefault,
      constA: parameterSet.constA,
      constB: parameterSet.constB,
      constC: parameterSet.constC,
      constD: parameterSet.constD
    });

    // Populate SF ratings
    this.scaleFactors.forEach(sf => {
      sf.options.forEach(option => {
        const fieldName = `${sf.fieldPrefix}${option.value}`;
        const value = (parameterSet as any)[fieldName.toLowerCase()];
        this.parameterSetForm.get(fieldName)?.setValue(value);
      });
    });

    // Populate EM ratings
    this.effortMultipliers.forEach(em => {
      em.options.forEach(option => {
        const fieldName = `${em.fieldPrefix}${option.value}`;
        const value = (parameterSet as any)[fieldName.toLowerCase()];
        this.parameterSetForm.get(fieldName)?.setValue(value);
      });
    });
  }

  onSubmit() {
    if (this.parameterSetForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.saving.set(true);
    this.error.set(null);

    const formValue = this.parameterSetForm.value;
    const request: CreateParameterSetRequest = {
      setName: formValue.setName,
      isDefault: formValue.isDefault,
      constA: formValue.constA,
      constB: formValue.constB,
      constC: formValue.constC,
      constD: formValue.constD,
      // Add all SF and EM fields
      ...this.extractRatings(formValue)
    };

    const operation = this.isEdit
      ? this.parameterSetService.updateParameterSet(this.parameterSetId!, request)
      : this.parameterSetService.createParameterSet(request);

    operation.subscribe({
      next: (response) => {
        this.saving.set(false);
        if (response.success) {
          this.router.navigate(['/cocomo-ii-stage3/parameter-sets']);
        } else {
          this.error.set(response.errors?.[0] || `Failed to ${this.isEdit ? 'update' : 'create'} parameter set`);
        }
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(`Failed to ${this.isEdit ? 'update' : 'create'} parameter set. Please try again.`);
        console.error('Error saving parameter set:', err);
      }
    });
  }

  private extractRatings(formValue: any): Partial<CreateParameterSetRequest> {
    const ratings: any = {};

    // Extract SF ratings
    this.scaleFactors.forEach(sf => {
      sf.options.forEach(option => {
        const fieldName = `${sf.fieldPrefix}${option.value}`;
        ratings[fieldName] = formValue[fieldName] || null;
      });
    });

    // Extract EM ratings
    this.effortMultipliers.forEach(em => {
      em.options.forEach(option => {
        const fieldName = `${em.fieldPrefix}${option.value}`;
        ratings[fieldName] = formValue[fieldName] || null;
      });
    });

    return ratings;
  }

  private markFormGroupTouched() {
    Object.keys(this.parameterSetForm.controls).forEach(key => {
      const control = this.parameterSetForm.get(key);
      control?.markAsTouched();
    });
  }

  goBack() {
    this.router.navigate(['/cocomo-ii-stage3/parameter-sets']);
  }
}