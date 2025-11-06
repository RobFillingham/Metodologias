import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ParameterSetService } from '../../../core/services/cocomo2/parameter-set.service';
import { ParameterSet, ApiResponse } from '../../../core/models/cocomo2/cocomo.models';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';

@Component({
  selector: 'app-parameter-sets',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  template: `
    <app-navbar></app-navbar>

    <div class="parameter-sets-container">
      <div class="header-section">
        <div class="header-content">
          <h1>⚙️ Parameter Sets</h1>
          <p>Manage your COCOMO II parameter configurations</p>
        </div>
        <button class="btn btn-primary" (click)="createNewParameterSet()">
          <i class="bi bi-plus-lg"></i>
          Create New Parameter Set
        </button>
      </div>

      <!-- Loading State -->
      <div *ngIf="loading()" class="loading-state">
        <div class="spinner"></div>
        <p>Loading parameter sets...</p>
      </div>

      <!-- Error State -->
      <div *ngIf="error()" class="error-alert">
        <strong>⚠️ Error:</strong> {{ error() }}
        <button class="btn btn-sm btn-outline-primary ms-2" (click)="loadParameterSets()">
          Retry
        </button>
      </div>

      <!-- Parameter Sets Grid -->
      <div *ngIf="!loading() && !error()" class="parameter-sets-grid">
        <div *ngFor="let paramSet of parameterSets()" class="parameter-set-card">
          <div class="card-header">
            <h3>{{ paramSet.setName }}</h3>
            <span class="badge" [class]="paramSet.isDefault ? 'badge-default' : 'badge-custom'">
              {{ paramSet.isDefault ? 'System Default' : 'Custom' }}
            </span>
          </div>

          <div class="card-body">
            <div class="constants-section">
              <h4>COCOMO Constants</h4>
              <div class="constants-grid">
                <div class="constant-item">
                  <label>A:</label>
                  <span>{{ paramSet.constA }}</span>
                </div>
                <div class="constant-item">
                  <label>B:</label>
                  <span>{{ paramSet.constB }}</span>
                </div>
                <div class="constant-item">
                  <label>C:</label>
                  <span>{{ paramSet.constC }}</span>
                </div>
                <div class="constant-item">
                  <label>D:</label>
                  <span>{{ paramSet.constD }}</span>
                </div>
              </div>
            </div>

            <div class="ratings-preview">
              <h4>Scale Factors & Effort Multipliers</h4>
              <p class="text-muted">Contains {{ getTotalRatings(paramSet) }} rating configurations</p>
            </div>
          </div>

          <div class="card-actions">
            <button
              class="btn btn-sm btn-outline-primary"
              (click)="viewParameterSet(paramSet)"
              title="View Details">
              <i class="bi bi-eye"></i>
              View
            </button>
            <button
              *ngIf="!paramSet.isDefault"
              class="btn btn-sm btn-outline-secondary"
              (click)="editParameterSet(paramSet)"
              title="Edit Parameter Set">
              <i class="bi bi-pencil"></i>
              Edit
            </button>
            <button
              *ngIf="!paramSet.isDefault"
              class="btn btn-sm btn-outline-danger"
              (click)="deleteParameterSet(paramSet)"
              title="Delete Parameter Set">
              <i class="bi bi-trash"></i>
              Delete
            </button>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div *ngIf="!loading() && !error() && parameterSets().length === 0" class="empty-state">
        <div class="empty-icon">⚙️</div>
        <h3>No Parameter Sets Found</h3>
        <p>You haven't created any custom parameter sets yet.</p>
        <button class="btn btn-primary" (click)="createNewParameterSet()">
          Create Your First Parameter Set
        </button>
      </div>
    </div>
  `,
  styles: [`
    .parameter-sets-container {
      min-height: 100vh;
      background: #f8f9fa;
      padding: 2rem;
    }

    .header-section {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 2rem;
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .header-content h1 {
      margin: 0 0 0.5rem 0;
      color: #333;
      font-size: 2rem;
    }

    .header-content p {
      margin: 0;
      color: #666;
    }

    .loading-state, .empty-state {
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

    .error-alert {
      background: #f8d7da;
      color: #721c24;
      padding: 1rem;
      border-radius: 8px;
      margin-bottom: 2rem;
      border: 1px solid #f5c6cb;
    }

    .parameter-sets-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
      gap: 1.5rem;
    }

    .parameter-set-card {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      overflow: hidden;
      transition: transform 0.2s, box-shadow 0.2s;
    }

    .parameter-set-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
    }

    .card-header {
      padding: 1.5rem;
      border-bottom: 1px solid #e1e5e9;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .card-header h3 {
      margin: 0;
      color: #333;
      font-size: 1.25rem;
    }

    .badge {
      padding: 0.25rem 0.75rem;
      border-radius: 20px;
      font-size: 0.75rem;
      font-weight: 500;
    }

    .badge-default {
      background: #e3f2fd;
      color: #1976d2;
    }

    .badge-custom {
      background: #f3e5f5;
      color: #7b1fa2;
    }

    .card-body {
      padding: 1.5rem;
    }

    .constants-section h4, .ratings-preview h4 {
      margin: 0 0 1rem 0;
      color: #333;
      font-size: 1rem;
    }

    .constants-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 0.75rem;
      margin-bottom: 1.5rem;
    }

    .constant-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.5rem;
      background: #f8f9fa;
      border-radius: 6px;
    }

    .constant-item label {
      font-weight: 500;
      color: #666;
    }

    .constant-item span {
      font-weight: 600;
      color: #333;
    }

    .ratings-preview p {
      margin: 0;
      color: #666;
      font-size: 0.9rem;
    }

    .card-actions {
      padding: 1rem 1.5rem;
      border-top: 1px solid #e1e5e9;
      display: flex;
      gap: 0.5rem;
      justify-content: flex-end;
    }

    .empty-icon {
      font-size: 4rem;
      margin-bottom: 1rem;
    }

    .empty-state h3 {
      color: #333;
      margin-bottom: 0.5rem;
    }

    .empty-state p {
      color: #666;
      margin-bottom: 2rem;
    }

    .btn {
      padding: 0.5rem 1rem;
      border: 1px solid #dee2e6;
      border-radius: 6px;
      font-size: 0.9rem;
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

    .btn-primary:hover {
      background: #5a6fd8;
      border-color: #5a6fd8;
    }

    .btn-outline-primary {
      color: #667eea;
      border-color: #667eea;
    }

    .btn-outline-primary:hover {
      background: #667eea;
      color: white;
    }

    .btn-outline-secondary {
      color: #6c757d;
      border-color: #6c757d;
    }

    .btn-outline-secondary:hover {
      background: #6c757d;
      color: white;
    }

    .btn-outline-danger {
      color: #dc3545;
      border-color: #dc3545;
    }

    .btn-outline-danger:hover {
      background: #dc3545;
      color: white;
    }

    .btn-sm {
      padding: 0.375rem 0.75rem;
      font-size: 0.8rem;
    }

    @media (max-width: 768px) {
      .parameter-sets-container {
        padding: 1rem;
      }

      .header-section {
        flex-direction: column;
        gap: 1rem;
        text-align: center;
      }

      .parameter-sets-grid {
        grid-template-columns: 1fr;
      }

      .card-actions {
        flex-direction: column;
      }

      .constants-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class ParameterSetsComponent implements OnInit {
  private parameterSetService = inject(ParameterSetService);
  private router = inject(Router);

  parameterSets = signal<ParameterSet[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  ngOnInit() {
    this.loadParameterSets();
  }

  loadParameterSets() {
    this.loading.set(true);
    this.error.set(null);

    this.parameterSetService.getAllParameterSets().subscribe({
      next: (response: ApiResponse<ParameterSet[]>) => {
        this.loading.set(false);
        if (response.success && response.data) {
          this.parameterSets.set(response.data);
        } else {
          this.error.set(response.errors?.[0] || 'Failed to load parameter sets');
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set('Failed to load parameter sets. Please try again.');
        console.error('Error loading parameter sets:', err);
      }
    });
  }

  createNewParameterSet() {
    // Navigate to create parameter set page
    this.router.navigate(['/parameter-sets/create']);
  }

  viewParameterSet(paramSet: ParameterSet) {
    this.router.navigate(['/parameter-sets', paramSet.paramSetId]);
  }

  editParameterSet(paramSet: ParameterSet) {
    this.router.navigate(['/parameter-sets', paramSet.paramSetId, 'edit']);
  }

  deleteParameterSet(paramSet: ParameterSet) {
    if (confirm(`Are you sure you want to delete "${paramSet.setName}"? This action cannot be undone.`)) {
      this.parameterSetService.deleteParameterSet(paramSet.paramSetId).subscribe({
        next: (response) => {
          if (response.success) {
            // Remove from local list
            this.parameterSets.update(sets => sets.filter(s => s.paramSetId !== paramSet.paramSetId));
          } else {
            alert('Failed to delete parameter set: ' + (response.errors?.[0] || 'Unknown error'));
          }
        },
        error: (err) => {
          alert('Failed to delete parameter set. Please try again.');
          console.error('Error deleting parameter set:', err);
        }
      });
    }
  }

  getTotalRatings(paramSet: ParameterSet): number {
    // Count non-null ratings for SF and EM
    let count = 0;
    // SF ratings (6 per factor × 5 factors = 30)
    const sfFields = ['sfPrec', 'sfFlex', 'sfResl', 'sfTeam', 'sfPmat'];
    sfFields.forEach(field => {
      ['Vlo', 'Lo', 'Nom', 'Hi', 'Vhi', 'Xhi'].forEach(level => {
        const key = `${field}${level}` as keyof ParameterSet;
        if (paramSet[key] !== null && paramSet[key] !== undefined) {
          count++;
        }
      });
    });
    // EM ratings (7 per factor × 7 factors = 49)
    const emFields = ['emPers', 'emRcpx', 'emPdif', 'emPrex', 'emRuse', 'emFcil', 'emSced'];
    emFields.forEach(field => {
      ['Xlo', 'Vlo', 'Lo', 'Nom', 'Hi', 'Vhi', 'Xhi'].forEach(level => {
        const key = `${field}${level}` as keyof ParameterSet;
        if (paramSet[key] !== null && paramSet[key] !== undefined) {
          count++;
        }
      });
    });
    return count;
  }
}