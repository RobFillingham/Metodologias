import { Component, EventEmitter, Input, Output, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProjectService } from '../../../core/services/cocomo2/project.service';
import { Project, CreateProjectRequest, UpdateProjectRequest, ApiResponse } from '../../../core/models/cocomo2/cocomo.models';

@Component({
  selector: 'app-project-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="modal-overlay" (click)="onCancel()">
      <div class="modal-content" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2>{{ isEditMode() ? '✏️ Edit Project' : '➕ Create New Project' }}</h2>
          <button class="btn-close" (click)="onCancel()" aria-label="Close">×</button>
        </div>

        <form (ngSubmit)="onSubmit()" #projectForm="ngForm">
          <div class="modal-body">
            <!-- Project Name -->
            <div class="form-group">
              <label for="projectName" class="required">Project Name</label>
              <input
                type="text"
                id="projectName"
                name="projectName"
                class="form-control"
                [(ngModel)]="formData.projectName"
                required
                maxlength="255"
                placeholder="e.g., Mobile Banking App"
                #projectName="ngModel"
              />
              <div class="error-message" *ngIf="projectName.invalid && projectName.touched">
                <span *ngIf="projectName.errors?.['required']">Project name is required</span>
              </div>
            </div>

            <!-- Description -->
            <div class="form-group">
              <label for="description">Description</label>
              <textarea
                id="description"
                name="description"
                class="form-control"
                [(ngModel)]="formData.description"
                rows="4"
                maxlength="1000"
                placeholder="Describe your project (optional)"
              ></textarea>
              <div class="char-count">
                {{ formData.description?.length || 0 }} / 1000 characters
              </div>
            </div>

            <!-- Error Message -->
            <div class="alert alert-error" *ngIf="error()">
              <span class="icon">⚠️</span>
              {{ error() }}
            </div>
          </div>

          <div class="modal-footer">
            <button 
              type="button" 
              class="btn btn-outline" 
              (click)="onCancel()"
              [disabled]="saving()"
            >
              Cancel
            </button>
            <button 
              type="submit" 
              class="btn btn-primary"
              [disabled]="projectForm.invalid || saving()"
            >
              <span *ngIf="!saving()">{{ isEditMode() ? 'Update' : 'Create' }} Project</span>
              <span *ngIf="saving()">
                <span class="spinner-small"></span>
                {{ isEditMode() ? 'Updating...' : 'Creating...' }}
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
      right: 0;
      bottom: 0;
      background: rgba(0, 0, 0, 0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
      padding: 1rem;
      animation: fadeIn 0.2s ease;
    }

    @keyframes fadeIn {
      from { opacity: 0; }
      to { opacity: 1; }
    }

    .modal-content {
      background: white;
      border-radius: 16px;
      box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
      width: 100%;
      max-width: 600px;
      max-height: 90vh;
      overflow: hidden;
      display: flex;
      flex-direction: column;
      animation: slideUp 0.3s ease;
    }

    @keyframes slideUp {
      from {
        opacity: 0;
        transform: translateY(20px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }

    .modal-header {
      padding: 1.5rem 2rem;
      border-bottom: 1px solid #e1e5e9;
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .modal-header h2 {
      margin: 0;
      font-size: 1.5rem;
      font-weight: 700;
    }

    .btn-close {
      background: transparent;
      border: none;
      color: white;
      font-size: 2rem;
      cursor: pointer;
      width: 40px;
      height: 40px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 8px;
      transition: all 0.2s ease;
    }

    .btn-close:hover {
      background: rgba(255, 255, 255, 0.2);
    }

    .modal-body {
      padding: 2rem;
      overflow-y: auto;
      flex: 1;
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

    label.required::after {
      content: ' *';
      color: #e53e3e;
    }

    .form-control {
      width: 100%;
      padding: 0.75rem 1rem;
      border: 2px solid #e1e5e9;
      border-radius: 8px;
      font-size: 1rem;
      transition: all 0.2s ease;
      font-family: inherit;
    }

    .form-control:focus {
      outline: none;
      border-color: #667eea;
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    }

    .form-control:disabled {
      background: #f3f4f6;
      cursor: not-allowed;
    }

    textarea.form-control {
      resize: vertical;
      min-height: 100px;
    }

    .char-count {
      margin-top: 0.5rem;
      font-size: 0.85rem;
      color: #666;
      text-align: right;
    }

    .error-message {
      margin-top: 0.5rem;
      color: #e53e3e;
      font-size: 0.875rem;
    }

    .alert {
      padding: 1rem;
      border-radius: 8px;
      margin-top: 1rem;
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }

    .alert-error {
      background: #fee;
      color: #c53030;
      border: 1px solid #feb2b2;
    }

    .alert .icon {
      font-size: 1.2rem;
    }

    .modal-footer {
      padding: 1.5rem 2rem;
      border-top: 1px solid #e1e5e9;
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
      background: #f9fafb;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 8px;
      font-size: 1rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.3s ease;
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
    }

    .btn:disabled {
      opacity: 0.6;
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

    .btn-outline {
      background: white;
      color: #667eea;
      border: 2px solid #667eea;
    }

    .btn-outline:hover:not(:disabled) {
      background: #f3f4f6;
    }

    .spinner-small {
      display: inline-block;
      width: 14px;
      height: 14px;
      border: 2px solid rgba(255, 255, 255, 0.3);
      border-top-color: white;
      border-radius: 50%;
      animation: spin 0.6s linear infinite;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }

    @media (max-width: 640px) {
      .modal-content {
        max-width: 100%;
        max-height: 100vh;
        border-radius: 0;
      }

      .modal-header,
      .modal-body,
      .modal-footer {
        padding: 1.5rem 1rem;
      }

      .modal-footer {
        flex-direction: column;
      }

      .btn {
        width: 100%;
        justify-content: center;
      }
    }
  `]
})
export class ProjectFormComponent implements OnInit {
  @Input() project: Project | null = null;
  @Output() save = new EventEmitter<Project>();
  @Output() cancel = new EventEmitter<void>();

  private projectService = inject(ProjectService);

  // Signals for reactive state
  saving = signal(false);
  error = signal<string | null>(null);
  isEditMode = signal(false);

  // Form data
  formData: { projectName: string; description: string | undefined } = {
    projectName: '',
    description: ''
  };

  ngOnInit() {
    if (this.project) {
      this.isEditMode.set(true);
      this.formData = {
        projectName: this.project.projectName,
        description: this.project.description || ''
      };
    }
  }

  onSubmit() {
    if (this.isEditMode()) {
      this.updateProject();
    } else {
      this.createProject();
    }
  }

  createProject() {
    this.saving.set(true);
    this.error.set(null);

    const request: CreateProjectRequest = {
      projectName: this.formData.projectName.trim(),
      description: this.formData.description?.trim() || undefined
    };

    this.projectService.createProject(request).subscribe({
      next: (response: ApiResponse<Project>) => {
        if (response.success && response.data) {
          this.save.emit(response.data);
        } else {
          this.error.set(response.message || 'Failed to create project');
        }
        this.saving.set(false);
      },
      error: (err: any) => {
        this.error.set(err.message || 'An error occurred while creating the project');
        this.saving.set(false);
      }
    });
  }

  updateProject() {
    if (!this.project) return;

    this.saving.set(true);
    this.error.set(null);

    const request: UpdateProjectRequest = {
      projectId: this.project.projectId,
      projectName: this.formData.projectName.trim(),
      description: this.formData.description?.trim() || undefined
    };

    this.projectService.updateProject(this.project.projectId, request).subscribe({
      next: (response: ApiResponse<Project>) => {
        if (response.success && response.data) {
          this.save.emit(response.data);
        } else {
          this.error.set(response.message || 'Failed to update project');
        }
        this.saving.set(false);
      },
      error: (err: any) => {
        this.error.set(err.message || 'An error occurred while updating the project');
        this.saving.set(false);
      }
    });
  }

  onCancel() {
    this.cancel.emit();
  }
}
