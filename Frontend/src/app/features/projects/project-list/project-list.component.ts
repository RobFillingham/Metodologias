import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { ProjectService } from '../../../core/services/cocomo2/project.service';
import { Project } from '../../../core/models/cocomo2/cocomo.models';
import { ProjectFormComponent } from '../project-form/project-form.component';

@Component({
  selector: 'app-project-list',
  standalone: true,
  imports: [CommonModule, NavbarComponent, ProjectFormComponent],
  template: `
    <app-navbar></app-navbar>

    <div class="projects-container">
      <div class="projects-header">
        <div class="header-content">
          <h1>📁 My Projects</h1>
          <p class="subtitle">Manage your COCOMO II estimation projects</p>
        </div>
        <button class="btn btn-primary" (click)="openCreateModal()">
          <span class="icon">+</span> New Project
        </button>
      </div>

      <!-- Loading State -->
      <div *ngIf="loading()" class="loading-container">
        <div class="spinner"></div>
        <p>Loading projects...</p>
      </div>

      <!-- Error State -->
      <div *ngIf="error()" class="error-container">
        <div class="error-icon">⚠️</div>
        <h3>Error Loading Projects</h3>
        <p>{{ error() }}</p>
        <button class="btn btn-primary" (click)="loadProjects()">Try Again</button>
      </div>

      <!-- Empty State -->
      <div *ngIf="!loading() && !error() && projects().length === 0" class="empty-state">
        <div class="empty-icon">📂</div>
        <h2>No Projects Yet</h2>
        <p>Create your first COCOMO II estimation project to get started.</p>
        <button class="btn btn-primary btn-lg" (click)="openCreateModal()">
          Create Your First Project
        </button>
      </div>

      <!-- Projects Grid -->
      <div *ngIf="!loading() && !error() && projects().length > 0" class="projects-grid">
        <div *ngFor="let project of projects()" class="project-card">
          <div class="project-header">
            <h3>{{ project.projectName }}</h3>
            <div class="project-actions">
              <button 
                class="btn-icon" 
                (click)="openEditModal(project)"
                title="Edit project"
              >
                ✏️
              </button>
              <button 
                class="btn-icon btn-danger" 
                (click)="deleteProject(project)"
                title="Delete project"
              >
                🗑️
              </button>
            </div>
          </div>
          
          <p class="project-description" *ngIf="project.description">
            {{ project.description }}
          </p>
          <p class="project-description empty" *ngIf="!project.description">
            No description provided
          </p>
          
          <div class="project-footer">
            <div class="project-meta">
              <span class="meta-item">
                <span class="icon">📅</span>
                {{ formatDate(project.createdAt) }}
              </span>
            </div>
            <button 
              class="btn btn-outline btn-sm"
              (click)="viewEstimations(project)"
            >
              View Estimations →
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Project Form Modal -->
    <app-project-form
      *ngIf="showModal()"
      [project]="selectedProject()"
      (save)="handleSave($event)"
      (cancel)="closeModal()"
    ></app-project-form>
  `,
  styles: [`
    .projects-container {
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 2rem;
    }

    .projects-header {
      max-width: 1200px;
      margin: 0 auto 2rem;
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 2rem;
    }

    .header-content h1 {
      margin: 0;
      color: white;
      font-size: 2.5rem;
      font-weight: 700;
    }

    .subtitle {
      margin: 0.5rem 0 0;
      color: rgba(255, 255, 255, 0.9);
      font-size: 1.1rem;
    }

    /* Loading State */
    .loading-container {
      max-width: 1200px;
      margin: 4rem auto;
      text-align: center;
      background: white;
      padding: 4rem 2rem;
      border-radius: 16px;
      box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
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

    /* Error State */
    .error-container {
      max-width: 1200px;
      margin: 4rem auto;
      text-align: center;
      background: white;
      padding: 4rem 2rem;
      border-radius: 16px;
      box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
    }

    .error-icon {
      font-size: 4rem;
      margin-bottom: 1rem;
    }

    .error-container h3 {
      color: #e53e3e;
      margin: 0 0 1rem;
    }

    .error-container p {
      color: #666;
      margin-bottom: 2rem;
    }

    /* Empty State */
    .empty-state {
      max-width: 600px;
      margin: 4rem auto;
      text-align: center;
      background: white;
      padding: 4rem 2rem;
      border-radius: 16px;
      box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
    }

    .empty-icon {
      font-size: 5rem;
      margin-bottom: 1.5rem;
    }

    .empty-state h2 {
      margin: 0 0 1rem;
      color: #333;
    }

    .empty-state p {
      color: #666;
      margin-bottom: 2rem;
      font-size: 1.1rem;
    }

    /* Projects Grid */
    .projects-grid {
      max-width: 1200px;
      margin: 0 auto;
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
      gap: 1.5rem;
    }

    .project-card {
      background: white;
      border-radius: 16px;
      padding: 1.5rem;
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
      transition: all 0.3s ease;
      display: flex;
      flex-direction: column;
    }

    .project-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 30px rgba(0, 0, 0, 0.15);
    }

    .project-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 1rem;
      gap: 1rem;
    }

    .project-header h3 {
      margin: 0;
      color: #333;
      font-size: 1.4rem;
      flex: 1;
      word-break: break-word;
    }

    .project-actions {
      display: flex;
      gap: 0.5rem;
      flex-shrink: 0;
    }

    .project-description {
      color: #666;
      margin: 0 0 1.5rem;
      line-height: 1.6;
      flex: 1;
    }

    .project-description.empty {
      font-style: italic;
      color: #999;
    }

    .project-footer {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding-top: 1rem;
      border-top: 1px solid #e1e5e9;
      gap: 1rem;
    }

    .project-meta {
      display: flex;
      gap: 1rem;
      flex-wrap: wrap;
    }

    .meta-item {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      color: #666;
      font-size: 0.9rem;
    }

    .meta-item .icon {
      font-size: 1rem;
    }

    /* Buttons */
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
      text-decoration: none;
    }

    .btn-primary {
      background: #667eea;
      color: white;
    }

    .btn-primary:hover {
      background: #5568d3;
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
    }

    .btn-outline {
      background: transparent;
      color: #667eea;
      border: 2px solid #667eea;
    }

    .btn-outline:hover {
      background: #667eea;
      color: white;
    }

    .btn-sm {
      padding: 0.5rem 1rem;
      font-size: 0.9rem;
    }

    .btn-lg {
      padding: 1rem 2rem;
      font-size: 1.1rem;
    }

    .btn-icon {
      background: transparent;
      border: none;
      cursor: pointer;
      font-size: 1.2rem;
      padding: 0.5rem;
      border-radius: 6px;
      transition: all 0.2s ease;
    }

    .btn-icon:hover {
      background: #f3f4f6;
    }

    .btn-icon.btn-danger:hover {
      background: #fee;
    }

    @media (max-width: 768px) {
      .projects-container {
        padding: 1rem;
      }

      .projects-header {
        flex-direction: column;
        align-items: flex-start;
      }

      .header-content h1 {
        font-size: 2rem;
      }

      .projects-grid {
        grid-template-columns: 1fr;
      }

      .project-footer {
        flex-direction: column;
        align-items: flex-start;
      }
    }
  `]
})
export class ProjectListComponent implements OnInit {
  private projectService = inject(ProjectService);
  private router = inject(Router);

  // Signals for reactive state
  projects = signal<Project[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  showModal = signal(false);
  selectedProject = signal<Project | null>(null);

  ngOnInit() {
    this.loadProjects();
  }

  loadProjects() {
    this.loading.set(true);
    this.error.set(null);

    this.projectService.getProjects().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.projects.set(response.data);
        } else {
          this.error.set(response.message || 'Failed to load projects');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.message || 'An error occurred while loading projects');
        this.loading.set(false);
      }
    });
  }

  openCreateModal() {
    this.selectedProject.set(null);
    this.showModal.set(true);
  }

  openEditModal(project: Project) {
    this.selectedProject.set(project);
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.selectedProject.set(null);
  }

  handleSave(project: Project) {
    // Reload projects after save
    this.loadProjects();
    this.closeModal();
    
    // Set as current project
    this.projectService.setCurrentProject(project);
  }

  deleteProject(project: Project) {
    if (!confirm(`Are you sure you want to delete "${project.projectName}"? This will delete all associated estimations.`)) {
      return;
    }

    this.projectService.deleteProject(project.projectId).subscribe({
      next: (response) => {
        if (response.success) {
          // Remove from local list
          this.projects.update(list => list.filter(p => p.projectId !== project.projectId));
          
          // Clear current project if it was deleted
          if (this.projectService.currentProjectValue?.projectId === project.projectId) {
            this.projectService.setCurrentProject(null);
          }
        } else {
          alert('Failed to delete project: ' + response.message);
        }
      },
      error: (err) => {
        alert('Error deleting project: ' + err.message);
      }
    });
  }

  viewEstimations(project: Project) {
    // Set as current project
    this.projectService.setCurrentProject(project);
    
    // Navigate to estimations (to be implemented)
    // For now, just navigate to dashboard
    this.router.navigate(['/dashboard']);
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}
