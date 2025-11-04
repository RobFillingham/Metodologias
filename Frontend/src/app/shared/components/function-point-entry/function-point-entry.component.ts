import { Component, Input, Output, EventEmitter, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EstimationFunctionService } from '../../../core/services/cocomo2/estimation-function.service';
import { EstimationFunction, CreateEstimationFunctionRequest, FunctionType, FUNCTION_TYPES, ApiResponse } from '../../../core/models/cocomo2/cocomo.models';

@Component({
  selector: 'app-function-point-entry',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="function-points-container">
      <div class="header">
        <h2>📊 Function Points</h2>
        <p class="subtitle">Define the functional components of your estimation</p>
      </div>

      <!-- Add Function Form -->
      <div class="add-form-card">
        <h3>➕ Add New Function</h3>
        <form (ngSubmit)="addFunction()" #functionForm="ngForm">
          <div class="form-row">
            <div class="form-group">
              <label for="name" class="required">Function Name</label>
              <input
                type="text"
                id="name"
                name="name"
                class="form-control"
                [(ngModel)]="newFunction.name"
                required
                maxlength="255"
                placeholder="e.g., User Registration"
                #name="ngModel"
              />
              <div class="error-message" *ngIf="name.invalid && name.touched">
                Function name is required
              </div>
            </div>

            <div class="form-group">
              <label for="type" class="required">Type</label>
              <select
                id="type"
                name="type"
                class="form-control"
                [(ngModel)]="newFunction.type"
                required
                #type="ngModel"
              >
                <option value="">Select type...</option>
                <option *ngFor="let t of functionTypes" [value]="t">{{ getFunctionTypeLabel(t) }}</option>
              </select>
              <div class="error-message" *ngIf="type.invalid && type.touched">
                Type is required
              </div>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="det" class="required">DET (Data Elements)</label>
              <input
                type="number"
                id="det"
                name="det"
                class="form-control"
                [(ngModel)]="newFunction.det"
                required
                min="1"
                max="999"
                placeholder="e.g., 10"
                #det="ngModel"
              />
              <small class="help-text">Number of data element types</small>
              <div class="error-message" *ngIf="det.invalid && det.touched">
                DET is required (1-999)
              </div>
            </div>

            <div class="form-group">
              <label for="retFtr" class="required">{{ getRetFtrLabel() }}</label>
              <input
                type="number"
                id="retFtr"
                name="retFtr"
                class="form-control"
                [(ngModel)]="newFunction.retFtr"
                required
                min="1"
                max="999"
                placeholder="e.g., 2"
                #retFtr="ngModel"
              />
              <small class="help-text">{{ getRetFtrHelp() }}</small>
              <div class="error-message" *ngIf="retFtr.invalid && retFtr.touched">
                {{ getRetFtrLabel() }} is required (1-999)
              </div>
            </div>
          </div>

          <div class="form-actions">
            <button 
              type="submit" 
              class="btn btn-primary"
              [disabled]="functionForm.invalid || saving()"
            >
              <span *ngIf="!saving()">➕ Add Function</span>
              <span *ngIf="saving()">
                <span class="spinner-small"></span> Adding...
              </span>
            </button>
          </div>

          <div class="alert alert-error" *ngIf="error()">
            <span class="icon">⚠️</span>
            {{ error() }}
          </div>
        </form>
      </div>

      <!-- Functions List -->
      <div class="functions-list">
        <div class="list-header">
          <h3>Functions ({{ functions().length }})</h3>
          <div class="total-ufp" *ngIf="functions().length > 0">
            <span class="label">Total UFP:</span>
            <span class="value">{{ getTotalUFP() }}</span>
          </div>
        </div>

        <!-- Empty State -->
        <div *ngIf="functions().length === 0" class="empty-state">
          <div class="empty-icon">📋</div>
          <p>No functions added yet</p>
          <small>Add functions above to start your estimation</small>
        </div>

        <!-- Functions Table -->
        <div *ngIf="functions().length > 0" class="table-container">
          <table class="functions-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Type</th>
                <th class="text-center">DET</th>
                <th class="text-center">RET/FTR</th>
                <th class="text-center">Complexity</th>
                <th class="text-right">Points</th>
                <th class="text-center">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let func of functions()" class="function-row">
                <td class="function-name">{{ func.name }}</td>
                <td>
                  <span class="badge badge-type">{{ getFunctionTypeLabel(func.type) }}</span>
                </td>
                <td class="text-center">{{ func.det }}</td>
                <td class="text-center">{{ func.retFtr }}</td>
                <td class="text-center">
                  <span class="badge" [ngClass]="getComplexityClass(func.complexity)">
                    {{ func.complexity || '-' }}
                  </span>
                </td>
                <td class="text-right points">{{ func.calculatedPoints || 0 }}</td>
                <td class="text-center">
                  <button 
                    class="btn-icon btn-danger"
                    (click)="deleteFunction(func)"
                    title="Delete function"
                  >
                    🗑️
                  </button>
                </td>
              </tr>
            </tbody>
            <tfoot>
              <tr class="total-row">
                <td colspan="5" class="text-right"><strong>Total UFP:</strong></td>
                <td class="text-right"><strong>{{ getTotalUFP() }}</strong></td>
                <td></td>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>

      <!-- Info Panel -->
      <div class="info-panel">
        <h4>📖 Function Point Types</h4>
        <div class="info-grid">
          <div class="info-item">
            <strong>EI</strong> - External Input
            <small>Data/control enters system boundary</small>
          </div>
          <div class="info-item">
            <strong>EO</strong> - External Output
            <small>Data leaves boundary with processing</small>
          </div>
          <div class="info-item">
            <strong>EQ</strong> - External Inquiry
            <small>Input-output with no significant processing</small>
          </div>
          <div class="info-item">
            <strong>ILF</strong> - Internal Logical File
            <small>Data stored and maintained by application</small>
          </div>
          <div class="info-item">
            <strong>EIF</strong> - External Interface File
            <small>Data referenced from other applications</small>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .function-points-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 2rem;
    }

    .header {
      margin-bottom: 2rem;
    }

    .header h2 {
      margin: 0;
      color: #333;
      font-size: 2rem;
    }

    .subtitle {
      margin: 0.5rem 0 0;
      color: #666;
      font-size: 1.1rem;
    }

    /* Add Form */
    .add-form-card {
      background: white;
      border-radius: 12px;
      padding: 2rem;
      margin-bottom: 2rem;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .add-form-card h3 {
      margin: 0 0 1.5rem;
      color: #333;
    }

    .form-row {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1.5rem;
      margin-bottom: 1.5rem;
    }

    .form-group {
      display: flex;
      flex-direction: column;
    }

    label {
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
      padding: 0.75rem;
      border: 2px solid #e1e5e9;
      border-radius: 8px;
      font-size: 1rem;
      transition: all 0.2s ease;
    }

    .form-control:focus {
      outline: none;
      border-color: #667eea;
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    }

    .help-text {
      margin-top: 0.25rem;
      font-size: 0.85rem;
      color: #666;
    }

    .error-message {
      margin-top: 0.5rem;
      color: #e53e3e;
      font-size: 0.875rem;
    }

    .form-actions {
      display: flex;
      justify-content: flex-end;
      margin-top: 1.5rem;
    }

    /* Functions List */
    .functions-list {
      background: white;
      border-radius: 12px;
      padding: 2rem;
      margin-bottom: 2rem;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .list-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
      padding-bottom: 1rem;
      border-bottom: 2px solid #e1e5e9;
    }

    .list-header h3 {
      margin: 0;
      color: #333;
    }

    .total-ufp {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.5rem 1rem;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      border-radius: 8px;
      color: white;
      font-weight: 600;
    }

    .total-ufp .value {
      font-size: 1.25rem;
    }

    /* Empty State */
    .empty-state {
      text-align: center;
      padding: 3rem 1rem;
      color: #666;
    }

    .empty-icon {
      font-size: 4rem;
      margin-bottom: 1rem;
    }

    /* Table */
    .table-container {
      overflow-x: auto;
    }

    .functions-table {
      width: 100%;
      border-collapse: collapse;
    }

    .functions-table th {
      background: #f9fafb;
      padding: 1rem;
      text-align: left;
      font-weight: 600;
      color: #333;
      border-bottom: 2px solid #e1e5e9;
    }

    .functions-table td {
      padding: 1rem;
      border-bottom: 1px solid #e1e5e9;
    }

    .function-row:hover {
      background: #f9fafb;
    }

    .function-name {
      font-weight: 500;
      color: #333;
    }

    .points {
      font-weight: 600;
      color: #667eea;
      font-size: 1.1rem;
    }

    .total-row {
      background: #f9fafb;
      font-weight: 600;
    }

    .total-row td {
      padding: 1.25rem 1rem;
      border-bottom: none;
    }

    /* Badges */
    .badge {
      display: inline-block;
      padding: 0.35rem 0.75rem;
      border-radius: 6px;
      font-size: 0.85rem;
      font-weight: 600;
    }

    .badge-type {
      background: #e0e7ff;
      color: #4c51bf;
    }

    .badge-baja {
      background: #d1fae5;
      color: #065f46;
    }

    .badge-media {
      background: #fef3c7;
      color: #92400e;
    }

    .badge-alta {
      background: #fee2e2;
      color: #991b1b;
    }

    /* Info Panel */
    .info-panel {
      background: #f0f4ff;
      border-radius: 12px;
      padding: 1.5rem;
      border-left: 4px solid #667eea;
    }

    .info-panel h4 {
      margin: 0 0 1rem;
      color: #333;
    }

    .info-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1rem;
    }

    .info-item {
      display: flex;
      flex-direction: column;
    }

    .info-item strong {
      color: #667eea;
      margin-bottom: 0.25rem;
    }

    .info-item small {
      color: #666;
      font-size: 0.85rem;
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

    /* Alert */
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

    /* Utility classes */
    .text-center {
      text-align: center;
    }

    .text-right {
      text-align: right;
    }

    @media (max-width: 768px) {
      .function-points-container {
        padding: 1rem;
      }

      .form-row {
        grid-template-columns: 1fr;
      }

      .table-container {
        font-size: 0.9rem;
      }

      .functions-table th,
      .functions-table td {
        padding: 0.75rem 0.5rem;
      }
    }
  `]
})
export class FunctionPointEntryComponent implements OnInit {
  @Input() estimationId!: number;
  @Output() functionsUpdated = new EventEmitter<void>();

  private functionService = inject(EstimationFunctionService);

  // Signals
  functions = signal<EstimationFunction[]>([]);
  saving = signal(false);
  error = signal<string | null>(null);

  // Form data
  newFunction: CreateEstimationFunctionRequest = {
    name: '',
    type: '' as FunctionType,
    det: 0,
    retFtr: 0
  };

  functionTypes = FUNCTION_TYPES;

  ngOnInit() {
    this.loadFunctions();
  }

  loadFunctions() {
    this.functionService.getFunctions(this.estimationId).subscribe({
      next: (response: ApiResponse<EstimationFunction[]>) => {
        if (response.success && response.data) {
          this.functions.set(response.data);
        }
      },
      error: (err: any) => {
        console.error('Error loading functions:', err);
      }
    });
  }

  addFunction() {
    this.saving.set(true);
    this.error.set(null);

    this.functionService.addFunction(this.estimationId, this.newFunction).subscribe({
      next: (response: ApiResponse<EstimationFunction>) => {
        if (response.success && response.data) {
          // Add to list
          this.functions.update(list => [...list, response.data!]);
          
          // Reset form
          this.newFunction = {
            name: '',
            type: '' as FunctionType,
            det: 0,
            retFtr: 0
          };

          // Notify parent
          this.functionsUpdated.emit();
        } else {
          this.error.set(response.message || 'Failed to add function');
        }
        this.saving.set(false);
      },
      error: (err: any) => {
        this.error.set(err.message || 'An error occurred while adding the function');
        this.saving.set(false);
      }
    });
  }

  deleteFunction(func: EstimationFunction) {
    if (!confirm(`Delete "${func.name}"?`)) {
      return;
    }

    this.functionService.deleteFunction(this.estimationId, func.functionId).subscribe({
      next: (response: ApiResponse<void>) => {
        if (response.success) {
          this.functions.update(list => list.filter(f => f.functionId !== func.functionId));
          this.functionsUpdated.emit();
        }
      },
      error: (err: any) => {
        alert('Error deleting function: ' + err.message);
      }
    });
  }

  getTotalUFP(): number {
    return this.functions().reduce((sum, func) => sum + (func.calculatedPoints || 0), 0);
  }

  getFunctionTypeLabel(type: string): string {
    const labels: Record<string, string> = {
      'EI': 'EI - External Input',
      'EO': 'EO - External Output',
      'EQ': 'EQ - External Inquiry',
      'ILF': 'ILF - Internal Logical File',
      'EIF': 'EIF - External Interface File'
    };
    return labels[type] || type;
  }

  getRetFtrLabel(): string {
    const type = this.newFunction.type;
    if (type === 'ILF' || type === 'EIF') {
      return 'RET (Record Elements)';
    }
    return 'FTR (File Types Referenced)';
  }

  getRetFtrHelp(): string {
    const type = this.newFunction.type;
    if (type === 'ILF' || type === 'EIF') {
      return 'Number of record element types';
    }
    return 'Number of file types referenced';
  }

  getComplexityClass(complexity?: string): string {
    if (!complexity) return '';
    return `badge-${complexity.toLowerCase()}`;
  }
}
