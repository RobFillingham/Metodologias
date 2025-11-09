import { Component, EventEmitter, Input, OnInit, Output, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  Cocomo2Stage1Estimation,
  Component as Cocomo2Component,
  CreateComponentRequest
} from '../../../core/models/cocomo2-stage1/cocomo2-stage1.models';
import { Cocomo2Stage1ComponentService } from '../../../core/services/cocomo2-stage1/cocomo2-stage1-component.service';

@Component({
  selector: 'app-cocomo2-stage1-components',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="components-container">
      <div class="components-header">
        <h3>Componentes de la Estimación</h3>
        <button class="btn btn-primary btn-sm" (click)="showAddForm()">
          + Agregar Componente
        </button>
      </div>

      <!-- Add Component Form -->
      <div class="add-form" *ngIf="showingAddForm()">
        <h4>Nuevo Componente</h4>
        <form (ngSubmit)="addComponent()">
          <div class="form-row">
            <div class="form-group">
              <label for="componentName">Nombre del Componente *</label>
              <input
                type="text"
                id="componentName"
                [(ngModel)]="newComponent.componentName"
                name="componentName"
                required
                placeholder="Ej: Pantalla de Login"
                class="form-input"
              />
            </div>
          </div>
          
          <div class="form-row">
            <div class="form-group">
              <label for="componentType">Tipo *</label>
              <select
                id="componentType"
                [(ngModel)]="newComponent.componentType"
                name="componentType"
                required
                class="form-input"
              >
                <option value="new">Nuevo</option>
                <option value="reused">Reutilizado</option>
                <option value="modified">Modificado</option>
              </select>
            </div>
            
            <div class="form-group">
              <label for="sizeFp">Tamaño (FP) *</label>
              <input
                type="number"
                id="sizeFp"
                [(ngModel)]="newComponent.sizeFp"
                name="sizeFp"
                required
                min="0"
                step="0.1"
                class="form-input"
              />
            </div>
          </div>

          <div class="form-row" *ngIf="newComponent.componentType !== 'new'">
            <div class="form-group">
              <label for="reusePercent">% Reutilización</label>
              <input
                type="number"
                id="reusePercent"
                [(ngModel)]="newComponent.reusePercent"
                name="reusePercent"
                min="0"
                max="100"
                class="form-input"
              />
            </div>
            
            <div class="form-group">
              <label for="changePercent">% Cambio</label>
              <input
                type="number"
                id="changePercent"
                [(ngModel)]="newComponent.changePercent"
                name="changePercent"
                min="0"
                max="100"
                class="form-input"
              />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="description">Descripción</label>
              <input
                type="text"
                id="description"
                [(ngModel)]="newComponent.description"
                name="description"
                placeholder="Descripción opcional"
                class="form-input"
              />
            </div>
          </div>

          <div class="form-actions">
            <button type="button" class="btn btn-secondary btn-sm" (click)="cancelAdd()">
              Cancelar
            </button>
            <button type="submit" class="btn btn-primary btn-sm" [disabled]="saving()">
              {{ saving() ? 'Guardando...' : 'Guardar' }}
            </button>
          </div>
        </form>
      </div>

      <!-- Components List -->
      <div class="components-list" *ngIf="components().length > 0">
        <table class="components-table">
          <thead>
            <tr>
              <th>Componente</th>
              <th>Tipo</th>
              <th>Tamaño (FP)</th>
              <th>% Reuso</th>
              <th>% Cambio</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let component of components()">
              <!-- View Mode -->
              <ng-container *ngIf="editingComponentId() !== component.componentId">
                <td><strong>{{ component.componentName }}</strong></td>
                <td>
                  <span class="type-badge" [class.type-new]="component.componentType === 'new'"
                        [class.type-reused]="component.componentType === 'reused'"
                        [class.type-modified]="component.componentType === 'modified'">
                    {{ getTypeLabel(component.componentType) }}
                  </span>
                </td>
                <td>{{ component.sizeFp }}</td>
                <td>{{ component.reusePercent || 0 }}%</td>
                <td>{{ component.changePercent || 0 }}%</td>
                <td class="actions">
                  <button 
                    class="btn-icon btn-edit"
                    (click)="startEdit(component)"
                    title="Editar"
                  >
                    ✎
                  </button>
                  <button 
                    class="btn-icon btn-delete"
                    (click)="deleteComponent(component)"
                    title="Eliminar"
                  >
                    🗑️
                  </button>
                </td>
              </ng-container>

              <!-- Edit Mode -->
              <ng-container *ngIf="editingComponentId() === component.componentId">
                <td>
                  <input
                    type="text"
                    [(ngModel)]="editComponent.componentName"
                    class="form-input-inline"
                  />
                </td>
                <td>
                  <select
                    [(ngModel)]="editComponent.componentType"
                    class="form-input-inline"
                  >
                    <option value="new">Nuevo</option>
                    <option value="reused">Reutilizado</option>
                    <option value="modified">Modificado</option>
                  </select>
                </td>
                <td>
                  <input
                    type="number"
                    [(ngModel)]="editComponent.sizeFp"
                    min="0"
                    step="0.1"
                    class="form-input-inline"
                  />
                </td>
                <td>
                  <input
                    type="number"
                    [(ngModel)]="editComponent.reusePercent"
                    min="0"
                    max="100"
                    class="form-input-inline"
                  />
                </td>
                <td>
                  <input
                    type="number"
                    [(ngModel)]="editComponent.changePercent"
                    min="0"
                    max="100"
                    class="form-input-inline"
                  />
                </td>
                <td class="actions">
                  <button 
                    class="btn-icon btn-save"
                    (click)="saveEdit(component.componentId)"
                    title="Guardar"
                  >
                    ✓
                  </button>
                  <button 
                    class="btn-icon btn-cancel"
                    (click)="cancelEdit()"
                    title="Cancelar"
                  >
                    ✕
                  </button>
                </td>
              </ng-container>
            </tr>
          </tbody>
        </table>

        <!-- Summary -->
        <div class="components-summary">
          <div class="summary-item">
            <span class="summary-label">Total Componentes:</span>
            <span class="summary-value">{{ components().length }}</span>
          </div>
          <div class="summary-item">
            <span class="summary-label">Total FP:</span>
            <span class="summary-value">{{ getTotalFP() }}</span>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div class="empty-state" *ngIf="components().length === 0 && !showingAddForm()">
        <div class="empty-icon">📦</div>
        <p>No hay componentes agregados</p>
        <p class="empty-hint">Los componentes representan screens, reports y componentes 3GL</p>
      </div>
    </div>
  `,
  styles: [`
    .components-container {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      border: 1px solid #e2e8f0;
    }

    .components-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
    }

    .components-header h3 {
      margin: 0;
      color: #1e293b;
      font-size: 1.2rem;
    }

    .add-form {
      background: #f8fafc;
      padding: 1.5rem;
      border-radius: 8px;
      margin-bottom: 1.5rem;
    }

    .add-form h4 {
      margin: 0 0 1rem 0;
      color: #334155;
      font-size: 1rem;
    }

    .form-row {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1rem;
      margin-bottom: 1rem;
    }

    .form-group {
      display: flex;
      flex-direction: column;
    }

    .form-group label {
      margin-bottom: 0.5rem;
      color: #334155;
      font-weight: 500;
      font-size: 0.9rem;
    }

    .form-input {
      padding: 0.625rem;
      border: 1px solid #cbd5e1;
      border-radius: 6px;
      font-size: 0.95rem;
    }

    .form-input:focus {
      outline: none;
      border-color: #3b82f6;
      box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
    }

    .form-input-inline {
      padding: 0.5rem;
      border: 1px solid #cbd5e1;
      border-radius: 4px;
      font-size: 0.9rem;
      width: 100%;
    }

    .form-actions {
      display: flex;
      gap: 0.75rem;
      justify-content: flex-end;
      margin-top: 1rem;
    }

    .components-table {
      width: 100%;
      border-collapse: collapse;
    }

    .components-table th {
      background-color: #f8fafc;
      padding: 0.75rem;
      text-align: left;
      font-weight: 600;
      color: #334155;
      border-bottom: 2px solid #e2e8f0;
      font-size: 0.85rem;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .components-table td {
      padding: 0.75rem;
      border-bottom: 1px solid #f1f5f9;
      color: #475569;
    }

    .components-table tr:hover {
      background-color: #f8fafc;
    }

    .type-badge {
      padding: 0.25rem 0.625rem;
      border-radius: 12px;
      font-size: 0.8rem;
      font-weight: 500;
    }

    .type-new {
      background-color: #dbeafe;
      color: #1e40af;
    }

    .type-reused {
      background-color: #d1fae5;
      color: #065f46;
    }

    .type-modified {
      background-color: #fef3c7;
      color: #92400e;
    }

    .actions {
      display: flex;
      gap: 0.5rem;
    }

    .btn-icon {
      background: none;
      border: none;
      padding: 0.4rem;
      cursor: pointer;
      border-radius: 4px;
      transition: all 0.2s;
      font-size: 1rem;
    }

    .btn-icon:hover {
      background-color: #f1f5f9;
    }

    .btn-edit:hover {
      background-color: #ddd6fe;
    }

    .btn-delete:hover {
      background-color: #fee2e2;
    }

    .btn-save:hover {
      background-color: #dcfce7;
    }

    .btn-cancel:hover {
      background-color: #fee2e2;
    }

    .components-summary {
      margin-top: 1rem;
      padding: 1rem;
      background-color: #f8fafc;
      border-radius: 6px;
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 1rem;
    }

    .summary-item {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .summary-label {
      color: #64748b;
      font-size: 0.85rem;
    }

    .summary-value {
      color: #1e293b;
      font-size: 1.25rem;
      font-weight: 600;
    }

    .empty-state {
      text-align: center;
      padding: 3rem 1rem;
      color: #64748b;
    }

    .empty-icon {
      font-size: 3rem;
      margin-bottom: 1rem;
    }

    .empty-state p {
      margin: 0.5rem 0;
    }

    .empty-hint {
      font-size: 0.9rem;
      color: #94a3b8;
    }

    .btn {
      padding: 0.5rem 1rem;
      border: none;
      border-radius: 6px;
      font-size: 0.9rem;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s;
    }

    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .btn-sm {
      padding: 0.4rem 0.875rem;
      font-size: 0.85rem;
    }

    .btn-primary {
      background-color: #3b82f6;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background-color: #2563eb;
    }

    .btn-secondary {
      background-color: #f1f5f9;
      color: #475569;
    }

    .btn-secondary:hover {
      background-color: #e2e8f0;
    }
  `]
})
export class Cocomo2Stage1ComponentsComponent implements OnInit {
  @Input() estimation: Cocomo2Stage1Estimation | null = null;
  @Output() componentsChanged = new EventEmitter<void>();

  private componentService = inject(Cocomo2Stage1ComponentService);

  components = signal<Cocomo2Component[]>([]);
  showingAddForm = signal(false);
  saving = signal(false);
  editingComponentId = signal<number | null>(null);

  newComponent: CreateComponentRequest = {
    componentName: '',
    componentType: 'new',
    sizeFp: 0,
    reusePercent: 0,
    changePercent: 0
  };

  editComponent: CreateComponentRequest = {
    componentName: '',
    componentType: 'new',
    sizeFp: 0,
    reusePercent: 0,
    changePercent: 0
  };

  ngOnInit() {
    if (this.estimation) {
      this.loadComponents();
    }
  }

  loadComponents() {
    if (!this.estimation) return;

    this.componentService.getEstimationComponents(this.estimation.estimationId).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.components.set(response.data);
        }
      },
      error: (err) => console.error('Error loading components:', err)
    });
  }

  showAddForm() {
    this.showingAddForm.set(true);
    this.resetNewComponent();
  }

  cancelAdd() {
    this.showingAddForm.set(false);
    this.resetNewComponent();
  }

  addComponent() {
    if (!this.estimation || !this.newComponent.componentName) return;

    this.saving.set(true);
    this.componentService.createComponent(this.estimation.estimationId, this.newComponent).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.components.update(list => [...list, response.data!]);
          this.componentsChanged.emit();
          this.cancelAdd();
        }
        this.saving.set(false);
      },
      error: (err) => {
        console.error('Error creating component:', err);
        alert('Error al crear el componente: ' + (err.error?.message || err.message));
        this.saving.set(false);
      }
    });
  }

  startEdit(component: Cocomo2Component) {
    this.editingComponentId.set(component.componentId);
    this.editComponent = {
      componentName: component.componentName,
      componentType: component.componentType,
      sizeFp: component.sizeFp,
      reusePercent: component.reusePercent,
      changePercent: component.changePercent,
      description: component.description
    };
  }

  cancelEdit() {
    this.editingComponentId.set(null);
  }

  saveEdit(componentId: number) {
    if (!this.estimation) return;

    const updateData: any = {
      componentId: componentId,
      ...this.editComponent
    };

    this.componentService.updateComponent(this.estimation.estimationId, componentId, updateData).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.components.update(list =>
            list.map(c => c.componentId === componentId ? response.data! : c)
          );
          this.componentsChanged.emit();
          this.cancelEdit();
        }
      },
      error: (err) => {
        console.error('Error updating component:', err);
        alert('Error al actualizar el componente');
      }
    });
  }

  deleteComponent(component: Cocomo2Component) {
    if (!confirm(`¿Eliminar el componente "${component.componentName}"?`)) {
      return;
    }

    if (!this.estimation) return;

    this.componentService.deleteComponent(this.estimation.estimationId, component.componentId).subscribe({
      next: (response) => {
        if (response.success) {
          this.components.update(list => list.filter(c => c.componentId !== component.componentId));
          this.componentsChanged.emit();
        }
      },
      error: (err) => {
        console.error('Error deleting component:', err);
        alert('Error al eliminar el componente');
      }
    });
  }

  getTotalFP(): number {
    return this.components().reduce((sum, c) => sum + c.sizeFp, 0);
  }

  getTypeLabel(type: string): string {
    const labels: { [key: string]: string } = {
      'new': 'Nuevo',
      'reused': 'Reutilizado',
      'modified': 'Modificado'
    };
    return labels[type] || type;
  }

  private resetNewComponent() {
    this.newComponent = {
      componentName: '',
      componentType: 'new',
      sizeFp: 0,
      reusePercent: 0,
      changePercent: 0
    };
  }
}
