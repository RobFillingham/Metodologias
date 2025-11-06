import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ParameterSetService } from '../../../../core/services/cocomo2/parameter-set.service';
import { ParameterSet, ApiResponse } from '../../../../core/models/cocomo2/cocomo.models';
import { NavbarComponent } from '../../../../shared/components/navbar/navbar.component';

interface RatingDisplay {
  factor: string;
  label: string;
  ratings: { level: string; value: number | null }[];
}

@Component({
  selector: 'app-parameter-set-detail',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  template: `
    <app-navbar></app-navbar>

    <div class="detail-container">
      <!-- Loading State -->
      <div *ngIf="loading()" class="loading-state">
        <div class="spinner"></div>
        <p>Loading parameter set details...</p>
      </div>

      <!-- Error State -->
      <div *ngIf="error()" class="error-alert">
        <strong>⚠️ Error:</strong> {{ error() }}
        <button class="btn btn-sm btn-outline-primary ms-2" (click)="loadParameterSet()">
          Retry
        </button>
      </div>

      <!-- Parameter Set Details -->
      <div *ngIf="!loading() && !error() && parameterSet()" class="parameter-set-detail">
        <!-- Header -->
        <div class="detail-header">
          <div class="header-content">
            <h1>{{ parameterSet()!.setName }}</h1>
            <span class="badge" [class]="parameterSet()!.isDefault ? 'badge-default' : 'badge-custom'">
              {{ parameterSet()!.isDefault ? 'System Default' : 'Custom Parameter Set' }}
            </span>
          </div>
          <div class="header-actions" *ngIf="!parameterSet()!.isDefault">
            <button class="btn btn-outline-primary" (click)="editParameterSet()">
              <i class="bi bi-pencil"></i>
              Edit
            </button>
            <button class="btn btn-outline-danger" (click)="deleteParameterSet()">
              <i class="bi bi-trash"></i>
              Delete
            </button>
          </div>
        </div>

        <!-- COCOMO Constants -->
        <div class="detail-section">
          <h3>COCOMO Constants</h3>
          <p class="section-description">
            These constants are used in the COCOMO II effort calculation formula: Effort = A × Size^B × M
          </p>
          <div class="constants-grid">
            <div class="constant-card">
              <div class="constant-value">{{ parameterSet()!.constA }}</div>
              <div class="constant-label">Constant A</div>
            </div>
            <div class="constant-card">
              <div class="constant-value">{{ parameterSet()!.constB }}</div>
              <div class="constant-label">Constant B</div>
            </div>
            <div class="constant-card">
              <div class="constant-value">{{ parameterSet()!.constC }}</div>
              <div class="constant-label">Constant C</div>
            </div>
            <div class="constant-card">
              <div class="constant-value">{{ parameterSet()!.constD }}</div>
              <div class="constant-label">Constant D</div>
            </div>
          </div>
        </div>

        <!-- Scale Factors -->
        <div class="detail-section">
          <h3>Scale Factors (SF)</h3>
          <p class="section-description">
            Scale factors adjust the effort based on project characteristics. Higher values increase effort.
          </p>
          <div class="ratings-table">
            <div *ngFor="let sf of scaleFactors" class="rating-row">
              <div class="factor-name">{{ sf.label }}</div>
              <div class="rating-values">
                <span
                  *ngFor="let rating of sf.ratings"
                  class="rating-badge"
                  [class.filled]="rating.value !== null"
                  [title]="rating.level + ': ' + (rating.value || 'Not set')"
                >
                  {{ rating.level }}: {{ rating.value || '—' }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Effort Multipliers -->
        <div class="detail-section">
          <h3>Effort Multipliers (EM)</h3>
          <p class="section-description">
            Effort multipliers adjust the effort based on product, platform, personnel, and project attributes.
          </p>
          <div class="ratings-table">
            <div *ngFor="let em of effortMultipliers" class="rating-row">
              <div class="factor-name">{{ em.label }}</div>
              <div class="rating-values">
                <span
                  *ngFor="let rating of em.ratings"
                  class="rating-badge"
                  [class.filled]="rating.value !== null"
                  [title]="rating.level + ': ' + (rating.value || 'Not set')"
                >
                  {{ rating.level }}: {{ rating.value || '—' }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Statistics -->
        <div class="detail-section">
          <h3>Configuration Statistics</h3>
          <div class="stats-grid">
            <div class="stat-card">
              <div class="stat-value">{{ getConfiguredRatingsCount() }}</div>
              <div class="stat-label">Configured Ratings</div>
            </div>
            <div class="stat-card">
              <div class="stat-value">{{ getTotalPossibleRatings() }}</div>
              <div class="stat-label">Total Possible Ratings</div>
            </div>
            <div class="stat-card">
              <div class="stat-value">{{ getCompletionPercentage() }}%</div>
              <div class="stat-label">Completion</div>
            </div>
          </div>
        </div>

        <!-- Back Button -->
        <div class="detail-actions">
          <button class="btn btn-secondary" (click)="goBack()">
            <i class="bi bi-arrow-left"></i>
            Back to Parameter Sets
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .detail-container {
      min-height: 100vh;
      background: #f8f9fa;
      padding: 2rem;
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

    .parameter-set-detail {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      overflow: hidden;
    }

    .detail-header {
      padding: 2rem;
      border-bottom: 1px solid #e1e5e9;
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      gap: 1rem;
    }

    .header-content h1 {
      margin: 0 0 0.5rem 0;
      color: #333;
      font-size: 2rem;
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

    .header-actions {
      display: flex;
      gap: 0.5rem;
    }

    .detail-section {
      padding: 2rem;
      border-bottom: 1px solid #e1e5e9;
    }

    .detail-section:last-child {
      border-bottom: none;
    }

    .detail-section h3 {
      margin: 0 0 1rem 0;
      color: #333;
      font-size: 1.5rem;
    }

    .section-description {
      color: #666;
      margin-bottom: 1.5rem;
      line-height: 1.6;
    }

    .constants-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 1rem;
    }

    .constant-card {
      background: #f8f9fa;
      padding: 1.5rem;
      border-radius: 8px;
      text-align: center;
      border: 1px solid #e1e5e9;
    }

    .constant-value {
      font-size: 1.5rem;
      font-weight: bold;
      color: #333;
      margin-bottom: 0.5rem;
    }

    .constant-label {
      color: #666;
      font-size: 0.9rem;
    }

    .ratings-table {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .rating-row {
      display: flex;
      align-items: center;
      padding: 1rem;
      background: #f8f9fa;
      border-radius: 8px;
      border: 1px solid #e1e5e9;
    }

    .factor-name {
      font-weight: 600;
      color: #333;
      min-width: 200px;
      flex-shrink: 0;
    }

    .rating-values {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
      flex: 1;
    }

    .rating-badge {
      background: #e9ecef;
      color: #495057;
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      font-size: 0.8rem;
      font-weight: 500;
    }

    .rating-badge.filled {
      background: #d1ecf1;
      color: #0c5460;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 1rem;
    }

    .stat-card {
      background: #f8f9fa;
      padding: 1.5rem;
      border-radius: 8px;
      text-align: center;
      border: 1px solid #e1e5e9;
    }

    .stat-value {
      font-size: 2rem;
      font-weight: bold;
      color: #333;
      margin-bottom: 0.5rem;
    }

    .stat-label {
      color: #666;
      font-size: 0.9rem;
    }

    .detail-actions {
      padding: 2rem;
      border-top: 1px solid #e1e5e9;
      text-align: center;
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

    .btn-secondary {
      background: #6c757d;
      color: white;
      border-color: #6c757d;
    }

    .btn-secondary:hover {
      background: #5a6268;
      border-color: #5a6268;
    }

    .btn-outline-primary {
      color: #667eea;
      border-color: #667eea;
    }

    .btn-outline-primary:hover {
      background: #667eea;
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
      .detail-container {
        padding: 1rem;
      }

      .detail-header {
        flex-direction: column;
        align-items: stretch;
      }

      .header-actions {
        justify-content: center;
      }

      .rating-row {
        flex-direction: column;
        align-items: stretch;
        gap: 1rem;
      }

      .factor-name {
        min-width: auto;
      }

      .constants-grid {
        grid-template-columns: repeat(2, 1fr);
      }

      .stats-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class ParameterSetDetailComponent implements OnInit {
  private parameterSetService = inject(ParameterSetService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  parameterSet = signal<ParameterSet | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);
  parameterSetId = signal<number | null>(null);

  scaleFactors: RatingDisplay[] = [];
  effortMultipliers: RatingDisplay[] = [];

  ngOnInit() {
    const id = +this.route.snapshot.params['id'];
    this.parameterSetId.set(id);
    this.loadParameterSet(id);
  }

  loadParameterSet(id?: number) {
    const paramId = id || this.parameterSetId();
    if (!paramId) return;

    this.loading.set(true);
    this.error.set(null);

    this.parameterSetService.getParameterSet(paramId).subscribe({
      next: (response) => {
        this.loading.set(false);
        if (response.success && response.data) {
          this.parameterSet.set(response.data);
          this.buildRatingDisplays(response.data);
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

  buildRatingDisplays(parameterSet: ParameterSet) {
    // Build Scale Factors display
    this.scaleFactors = [
      {
        factor: 'PREC',
        label: 'Precedentedness (PREC)',
        ratings: [
          { level: 'VLO', value: parameterSet.sfPrecVlo },
          { level: 'LO', value: parameterSet.sfPrecLo },
          { level: 'NOM', value: parameterSet.sfPrecNom },
          { level: 'HI', value: parameterSet.sfPrecHi },
          { level: 'VHI', value: parameterSet.sfPrecVhi },
          { level: 'XHI', value: parameterSet.sfPrecXhi }
        ]
      },
      {
        factor: 'FLEX',
        label: 'Development Flexibility (FLEX)',
        ratings: [
          { level: 'VLO', value: parameterSet.sfFlexVlo },
          { level: 'LO', value: parameterSet.sfFlexLo },
          { level: 'NOM', value: parameterSet.sfFlexNom },
          { level: 'HI', value: parameterSet.sfFlexHi },
          { level: 'VHI', value: parameterSet.sfFlexVhi },
          { level: 'XHI', value: parameterSet.sfFlexXhi }
        ]
      },
      {
        factor: 'RESL',
        label: 'Architecture/Risk Resolution (RESL)',
        ratings: [
          { level: 'VLO', value: parameterSet.sfReslVlo },
          { level: 'LO', value: parameterSet.sfReslLo },
          { level: 'NOM', value: parameterSet.sfReslNom },
          { level: 'HI', value: parameterSet.sfReslHi },
          { level: 'VHI', value: parameterSet.sfReslVhi },
          { level: 'XHI', value: parameterSet.sfReslXhi }
        ]
      },
      {
        factor: 'TEAM',
        label: 'Team Cohesion (TEAM)',
        ratings: [
          { level: 'VLO', value: parameterSet.sfTeamVlo },
          { level: 'LO', value: parameterSet.sfTeamLo },
          { level: 'NOM', value: parameterSet.sfTeamNom },
          { level: 'HI', value: parameterSet.sfTeamHi },
          { level: 'VHI', value: parameterSet.sfTeamVhi },
          { level: 'XHI', value: parameterSet.sfTeamXhi }
        ]
      },
      {
        factor: 'PMAT',
        label: 'Process Maturity (PMAT)',
        ratings: [
          { level: 'VLO', value: parameterSet.sfPmatVlo },
          { level: 'LO', value: parameterSet.sfPmatLo },
          { level: 'NOM', value: parameterSet.sfPmatNom },
          { level: 'HI', value: parameterSet.sfPmatHi },
          { level: 'VHI', value: parameterSet.sfPmatVhi },
          { level: 'XHI', value: parameterSet.sfPmatXhi }
        ]
      }
    ];

    // Build Effort Multipliers display
    this.effortMultipliers = [
      {
        factor: 'PERS',
        label: 'Personnel Capability (PERS)',
        ratings: [
          { level: 'XLO', value: parameterSet.emPersXlo },
          { level: 'VLO', value: parameterSet.emPersVlo },
          { level: 'LO', value: parameterSet.emPersLo },
          { level: 'NOM', value: parameterSet.emPersNom },
          { level: 'HI', value: parameterSet.emPersHi },
          { level: 'VHI', value: parameterSet.emPersVhi },
          { level: 'XHI', value: parameterSet.emPersXhi }
        ]
      },
      {
        factor: 'RCPX',
        label: 'Product Reliability and Complexity (RCPX)',
        ratings: [
          { level: 'XLO', value: parameterSet.emRcpxXlo },
          { level: 'VLO', value: parameterSet.emRcpxVlo },
          { level: 'LO', value: parameterSet.emRcpxLo },
          { level: 'NOM', value: parameterSet.emRcpxNom },
          { level: 'HI', value: parameterSet.emRcpxHi },
          { level: 'VHI', value: parameterSet.emRcpxVhi },
          { level: 'XHI', value: parameterSet.emRcpxXhi }
        ]
      },
      {
        factor: 'PDIF',
        label: 'Platform Difficulty (PDIF)',
        ratings: [
          { level: 'XLO', value: parameterSet.emPdifXlo },
          { level: 'VLO', value: parameterSet.emPdifVlo },
          { level: 'LO', value: parameterSet.emPdifLo },
          { level: 'NOM', value: parameterSet.emPdifNom },
          { level: 'HI', value: parameterSet.emPdifHi },
          { level: 'VHI', value: parameterSet.emPdifVhi },
          { level: 'XHI', value: parameterSet.emPdifXhi }
        ]
      },
      {
        factor: 'PREX',
        label: 'Personnel Experience (PREX)',
        ratings: [
          { level: 'XLO', value: parameterSet.emPrexXlo },
          { level: 'VLO', value: parameterSet.emPrexVlo },
          { level: 'LO', value: parameterSet.emPrexLo },
          { level: 'NOM', value: parameterSet.emPrexNom },
          { level: 'HI', value: parameterSet.emPrexHi },
          { level: 'VHI', value: parameterSet.emPrexVhi },
          { level: 'XHI', value: parameterSet.emPrexXhi }
        ]
      },
      {
        factor: 'RUSE',
        label: 'Reusability (RUSE)',
        ratings: [
          { level: 'XLO', value: parameterSet.emRuseXlo },
          { level: 'VLO', value: parameterSet.emRuseVlo },
          { level: 'LO', value: parameterSet.emRuseLo },
          { level: 'NOM', value: parameterSet.emRuseNom },
          { level: 'HI', value: parameterSet.emRuseHi },
          { level: 'VHI', value: parameterSet.emRuseVhi },
          { level: 'XHI', value: parameterSet.emRuseXhi }
        ]
      },
      {
        factor: 'FCIL',
        label: 'Facilities (FCIL)',
        ratings: [
          { level: 'XLO', value: parameterSet.emFcilXlo },
          { level: 'VLO', value: parameterSet.emFcilVlo },
          { level: 'LO', value: parameterSet.emFcilLo },
          { level: 'NOM', value: parameterSet.emFcilNom },
          { level: 'HI', value: parameterSet.emFcilHi },
          { level: 'VHI', value: parameterSet.emFcilVhi },
          { level: 'XHI', value: parameterSet.emFcilXhi }
        ]
      },
      {
        factor: 'SCED',
        label: 'Required Development Schedule (SCED)',
        ratings: [
          { level: 'XLO', value: parameterSet.emScedXlo },
          { level: 'VLO', value: parameterSet.emScedVlo },
          { level: 'LO', value: parameterSet.emScedLo },
          { level: 'NOM', value: parameterSet.emScedNom },
          { level: 'HI', value: parameterSet.emScedHi },
          { level: 'VHI', value: parameterSet.emScedVhi },
          { level: 'XHI', value: parameterSet.emScedXhi }
        ]
      }
    ];
  }

  editParameterSet() {
    if (this.parameterSet()) {
      this.router.navigate(['/parameter-sets', this.parameterSet()!.paramSetId, 'edit']);
    }
  }

  deleteParameterSet() {
    if (this.parameterSet() && !this.parameterSet()!.isDefault) {
      if (confirm(`Are you sure you want to delete "${this.parameterSet()!.setName}"? This action cannot be undone.`)) {
        this.parameterSetService.deleteParameterSet(this.parameterSet()!.paramSetId).subscribe({
          next: (response) => {
            if (response.success) {
              this.router.navigate(['/parameter-sets']);
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
  }

  getConfiguredRatingsCount(): number {
    if (!this.parameterSet()) return 0;

    let count = 0;
    const paramSet = this.parameterSet()!;

    // Count SF ratings
    const sfFields = ['sfPrec', 'sfFlex', 'sfResl', 'sfTeam', 'sfPmat'];
    sfFields.forEach(field => {
      ['Vlo', 'Lo', 'Nom', 'Hi', 'Vhi', 'Xhi'].forEach(level => {
        const key = `${field}${level}` as keyof ParameterSet;
        if (paramSet[key] !== null && paramSet[key] !== undefined) {
          count++;
        }
      });
    });

    // Count EM ratings
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

  getTotalPossibleRatings(): number {
    return (5 * 6) + (7 * 7); // 30 SF + 49 EM = 79 total
  }

  getCompletionPercentage(): number {
    const configured = this.getConfiguredRatingsCount();
    const total = this.getTotalPossibleRatings();
    return Math.round((configured / total) * 100);
  }

  goBack() {
    this.router.navigate(['/parameter-sets']);
  }
}