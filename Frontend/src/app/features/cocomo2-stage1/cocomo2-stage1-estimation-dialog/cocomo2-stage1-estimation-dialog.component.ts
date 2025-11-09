import { Component, EventEmitter, Input, OnInit, Output, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  Cocomo2Stage1Estimation,
  CreateCocomo2Stage1EstimationRequest,
  ParameterSet,
  Language,
  CreateComponentRequest
} from '../../../core/models/cocomo2-stage1/cocomo2-stage1.models';
import { Project } from '../../../core/models/kloc/kloc.models';
import { Cocomo2Stage1EstimationService } from '../../../core/services/cocomo2-stage1/cocomo2-stage1-estimation.service';
import { LanguageService } from '../../../core/services/cocomo2/language.service';

@Component({
  selector: 'app-cocomo2-stage1-estimation-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="modal-overlay" (click)="onCancel()">
      <div class="modal-content" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2>{{ isEditMode() ? 'Editar' : 'Nueva' }} Estimación COCOMO II Stage 1</h2>
          <button class="btn-close" (click)="onCancel()">×</button>
        </div>

        <!-- Progress Steps -->
        <div class="steps-container" *ngIf="!isEditMode()">
          <div class="step" [class.active]="currentStep() === 'basic'" [class.completed]="currentStep() === 'components'">
            <div class="step-number">1</div>
            <div class="step-label">Información Básica</div>
          </div>
          <div class="step-line" [class.completed]="currentStep() === 'components'"></div>
          <div class="step" [class.active]="currentStep() === 'components'">
            <div class="step-number">2</div>
            <div class="step-label">Componentes</div>
          </div>
        </div>

        <div class="modal-body">
          <!-- Loading State -->
          <div *ngIf="loading()" class="loading-state">
            <div class="spinner"></div>
            <p>Cargando datos...</p>
          </div>

          <!-- Step 1: Basic Information -->
          <div *ngIf="!loading() && currentStep() === 'basic'">
            <form #estimationForm="ngForm">
              <!-- Estimation Name -->
              <div class="form-group">
                <label for="estimationName">Nombre de la Estimación *</label>
                <input
                  type="text"
                  id="estimationName"
                  name="estimationName"
                  [(ngModel)]="formData.estimationName"
                  required
                  minlength="3"
                  placeholder="Ej: Estimación Sistema de Ventas"
                  class="form-input"
                  #estimationNameInput="ngModel"
                />
                <div *ngIf="estimationNameInput.invalid && estimationNameInput.touched" class="validation-error">
                  <span *ngIf="estimationNameInput.errors?.['required']">El nombre es obligatorio</span>
                  <span *ngIf="estimationNameInput.errors?.['minlength']">El nombre debe tener al menos 3 caracteres</span>
                </div>
              </div>

              <!-- Project (only for create) -->
              <div class="form-group" *ngIf="!isEditMode() && project">
                <label>Proyecto</label>
                <div class="readonly-field">
                  <span class="project-icon">📁</span>
                  {{ project.projectName }}
                </div>
              </div>

              <!-- Language -->
              <div class="form-group">
                <label for="languageId">Lenguaje de Programación *</label>
                <select
                  id="languageId"
                  name="languageId"
                  [(ngModel)]="formData.languageId"
                  required
                  class="form-select"
                  #languageSelect="ngModel"
                >
                  <option [value]="0" disabled>Seleccionar lenguaje...</option>
                  <option *ngFor="let lang of languages()" [value]="lang.languageId">
                    {{ lang.name }} ({{ lang.slocPerUfp }} SLOC/UFP)
                  </option>
                </select>
                <div *ngIf="languageSelect.invalid && languageSelect.touched" class="validation-error">
                  <span>Debe seleccionar un lenguaje</span>
                </div>
              </div>

              <!-- Parameter Set -->
              <div class="form-group">
                <label for="paramSetId">Conjunto de Parámetros *</label>
                <select
                  id="paramSetId"
                  name="paramSetId"
                  [(ngModel)]="formData.paramSetId"
                  required
                  class="form-select"
                  #paramSetSelect="ngModel"
                >
                  <option [value]="0" disabled>Seleccionar conjunto...</option>
                  <optgroup label="Conjuntos por Defecto" *ngIf="defaultParamSets().length > 0">
                    <option *ngFor="let ps of defaultParamSets()" [value]="ps.paramSetId">
                      {{ ps.setName }} (a={{ ps.constA }}, b={{ ps.constB }})
                    </option>
                  </optgroup>
                  <optgroup label="Mis Conjuntos Personalizados" *ngIf="userParamSets().length > 0">
                    <option *ngFor="let ps of userParamSets()" [value]="ps.paramSetId">
                      {{ ps.setName }} (a={{ ps.constA }}, b={{ ps.constB }})
                    </option>
                  </optgroup>
                </select>
                <div *ngIf="paramSetSelect.invalid && paramSetSelect.touched" class="validation-error">
                  <span>Debe seleccionar un conjunto de parámetros</span>
                </div>
              </div>

              <!-- ACEM Ratings Section -->
              <div class="section-divider">
                <h3>Factores ACEM</h3>
                <p class="section-description">Selecciona el nivel para cada factor de esfuerzo</p>
              </div>

              <div class="ratings-grid">
                <!-- AEXP -->
                <div class="form-group">
                  <label for="selectedAexp">AEXP - Experiencia del Analista</label>
                  <select id="selectedAexp" name="selectedAexp" [(ngModel)]="formData.selectedAexp" class="form-select">
                    <option value="very_low">Muy Baja</option>
                    <option value="low">Baja</option>
                    <option value="nominal" selected>Nominal</option>
                    <option value="high">Alta</option>
                    <option value="very_high">Muy Alta</option>
                  </select>
                </div>

                <!-- PEXP -->
                <div class="form-group">
                  <label for="selectedPexp">PEXP - Experiencia del Programador</label>
                  <select id="selectedPexp" name="selectedPexp" [(ngModel)]="formData.selectedPexp" class="form-select">
                    <option value="very_low">Muy Baja</option>
                    <option value="low">Baja</option>
                    <option value="nominal" selected>Nominal</option>
                    <option value="high">Alta</option>
                    <option value="very_high">Muy Alta</option>
                  </select>
                </div>

                <!-- PREC -->
                <div class="form-group">
                  <label for="selectedPrec">PREC - Precedentes</label>
                  <select id="selectedPrec" name="selectedPrec" [(ngModel)]="formData.selectedPrec" class="form-select">
                    <option value="very_low">Muy Bajo</option>
                    <option value="low">Bajo</option>
                    <option value="nominal" selected>Nominal</option>
                    <option value="high">Alto</option>
                    <option value="very_high">Muy Alto</option>
                  </select>
                </div>

                <!-- RELY -->
                <div class="form-group">
                  <label for="selectedRely">RELY - Confiabilidad Requerida</label>
                  <select id="selectedRely" name="selectedRely" [(ngModel)]="formData.selectedRely" class="form-select">
                    <option value="very_low">Muy Baja</option>
                    <option value="low">Baja</option>
                    <option value="nominal" selected>Nominal</option>
                    <option value="high">Alta</option>
                    <option value="very_high">Muy Alta</option>
                  </select>
                </div>

                <!-- TMSP -->
                <div class="form-group">
                  <label for="selectedTmsp">TMSP - Soporte del Equipo</label>
                  <select id="selectedTmsp" name="selectedTmsp" [(ngModel)]="formData.selectedTmsp" class="form-select">
                    <option value="very_low">Muy Bajo</option>
                    <option value="low">Bajo</option>
                    <option value="nominal" selected>Nominal</option>
                    <option value="high">Alto</option>
                    <option value="very_high">Muy Alto</option>
                  </select>
                </div>
              </div>
            </form>
          </div>

          <!-- Step 2: Components -->
          <div *ngIf="!loading() && currentStep() === 'components'" class="components-step">
            <div class="step-header">
              <div>
                <h3>Componentes del Proyecto</h3>
                <p class="step-description">Agrega los componentes (screens, reports, 3GL components) para calcular el esfuerzo</p>
              </div>
              <button class="btn btn-primary btn-sm" (click)="showComponentForm.set(true)" *ngIf="!showComponentForm()">
                + Agregar Componente
              </button>
            </div>

            <!-- Component Form -->
            <div class="component-form-card" *ngIf="showComponentForm()">
              <h4>Nuevo Componente</h4>
              <div class="form-row">
                <div class="form-group">
                  <label for="compName">Nombre *</label>
                  <input type="text" id="compName" [(ngModel)]="newComponent.componentName" class="form-input" placeholder="Ej: Pantalla Login">
                </div>
                <div class="form-group">
                  <label for="compType">Tipo *</label>
                  <select id="compType" [(ngModel)]="newComponent.componentType" class="form-select">
                    <option value="new">Nuevo</option>
                    <option value="reused">Reutilizado</option>
                    <option value="modified">Modificado</option>
                  </select>
                </div>
              </div>
              <div class="form-row">
                <div class="form-group">
                  <label for="compSize">Tamaño (FP) *</label>
                  <input type="number" id="compSize" [(ngModel)]="newComponent.sizeFp" class="form-input" min="0" step="0.1">
                </div>
                <div class="form-group" *ngIf="newComponent.componentType !== 'new'">
                  <label for="compReuse">% Reutilización</label>
                  <input type="number" id="compReuse" [(ngModel)]="newComponent.reusePercent" class="form-input" min="0" max="100">
                </div>
                <div class="form-group" *ngIf="newComponent.componentType !== 'new'">
                  <label for="compChange">% Cambio</label>
                  <input type="number" id="compChange" [(ngModel)]="newComponent.changePercent" class="form-input" min="0" max="100">
                </div>
              </div>
              <div class="form-actions">
                <button type="button" class="btn btn-secondary btn-sm" (click)="cancelComponentForm()">Cancelar</button>
                <button type="button" class="btn btn-primary btn-sm" (click)="addTempComponent()">Agregar</button>
              </div>
            </div>

            <!-- Components List -->
            <div class="components-list" *ngIf="tempComponents().length > 0">
              <table class="components-table">
                <thead>
                  <tr>
                    <th>Componente</th>
                    <th>Tipo</th>
                    <th>Tamaño (FP)</th>
                    <th>% Reuso</th>
                    <th>% Cambio</th>
                    <th>Acción</th>
                  </tr>
                </thead>
                <tbody>
                  <tr *ngFor="let comp of tempComponents(); let i = index">
                    <td><strong>{{ comp.componentName }}</strong></td>
                    <td>
                      <span class="type-badge" [class.type-new]="comp.componentType === 'new'"
                            [class.type-reused]="comp.componentType === 'reused'"
                            [class.type-modified]="comp.componentType === 'modified'">
                        {{ getTypeLabel(comp.componentType) }}
                      </span>
                    </td>
                    <td>{{ comp.sizeFp }}</td>
                    <td>{{ comp.reusePercent || 0 }}%</td>
                    <td>{{ comp.changePercent || 0 }}%</td>
                    <td>
                      <button class="btn-icon btn-delete" (click)="removeTempComponent(i)" title="Eliminar">🗑️</button>
                    </td>
                  </tr>
                </tbody>
              </table>
              <div class="components-summary">
                <span class="summary-label">Total FP:</span>
                <span class="summary-value">{{ getTotalFP() }}</span>
              </div>
            </div>

            <!-- Empty State -->
            <div class="empty-state" *ngIf="tempComponents().length === 0 && !showComponentForm()">
              <div class="empty-icon">📦</div>
              <p>No hay componentes agregados</p>
              <p class="empty-hint">Agrega al menos un componente para poder calcular el esfuerzo</p>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <!-- Step 1 Footer -->
          <div *ngIf="currentStep() === 'basic'" class="footer-actions">
            <button type="button" class="btn btn-secondary" (click)="onCancel()">Cancelar</button>
            <button type="button" class="btn btn-primary" (click)="goToComponents()" [disabled]="!isBasicFormValid()">
              Siguiente: Agregar Componentes →
            </button>
          </div>

          <!-- Step 2 Footer -->
          <div *ngIf="currentStep() === 'components'" class="footer-actions">
            <button type="button" class="btn btn-secondary" (click)="goBackToBasic()">← Atrás</button>
            <button type="button" class="btn btn-primary" (click)="onSubmit()" [disabled]="tempComponents().length === 0">
              {{ isEditMode() ? 'Actualizar' : 'Crear' }} Estimación
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background-color: rgba(0, 0, 0, 0.6);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
      padding: 1rem;
      animation: fadeIn 0.2s ease-out;
    }

    @keyframes fadeIn {
      from { opacity: 0; }
      to { opacity: 1; }
    }

    .modal-content {
      background: white;
      border-radius: 16px;
      width: 100%;
      max-width: 800px;
      max-height: 90vh;
      display: flex;
      flex-direction: column;
      box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
      animation: slideUp 0.3s ease-out;
    }

    @keyframes slideUp {
      from { 
        opacity: 0;
        transform: translateY(30px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }

    .modal-header {
      padding: 2rem 2rem 1rem;
      border-bottom: 1px solid #e2e8f0;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .modal-header h2 {
      margin: 0;
      color: #1e293b;
      font-size: 1.75rem;
      font-weight: 700;
    }

    .btn-close {
      background: none;
      border: none;
      font-size: 2rem;
      color: #64748b;
      cursor: pointer;
      padding: 0;
      width: 36px;
      height: 36px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 8px;
      transition: all 0.2s;
    }

    .btn-close:hover {
      background-color: #f1f5f9;
      color: #1e293b;
    }

    /* Steps Progress */
    .steps-container {
      display: flex;
      align-items: center;
      padding: 1.5rem 2rem;
      background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
      border-bottom: 1px solid #e2e8f0;
    }

    .step {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 0.5rem;
      flex: 1;
    }

    .step-number {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      background: white;
      border: 2px solid #cbd5e1;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 600;
      color: #94a3b8;
      transition: all 0.3s;
    }

    .step.active .step-number {
      background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
      border-color: #3b82f6;
      color: white;
      box-shadow: 0 4px 12px rgba(59, 130, 246, 0.3);
    }

    .step.completed .step-number {
      background: #10b981;
      border-color: #10b981;
      color: white;
    }

    .step.completed .step-number::before {
      content: '✓';
    }

    .step-label {
      font-size: 0.85rem;
      color: #64748b;
      font-weight: 500;
      text-align: center;
    }

    .step.active .step-label {
      color: #1e293b;
      font-weight: 600;
    }

    .step-line {
      height: 2px;
      flex: 1;
      background: #cbd5e1;
      margin: 0 1rem;
      margin-bottom: 2rem;
      transition: all 0.3s;
    }

    .step-line.completed {
      background: #10b981;
    }

    .modal-body {
      padding: 2rem;
      overflow-y: auto;
      flex: 1;
    }

    .loading-state {
      text-align: center;
      padding: 3rem 1rem;
    }

    .spinner {
      width: 50px;
      height: 50px;
      border: 4px solid #e2e8f0;
      border-top-color: #3b82f6;
      border-radius: 50%;
      animation: spin 1s linear infinite;
      margin: 0 auto 1rem;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }

    .form-group {
      margin-bottom: 1.5rem;
    }

    .form-group label {
      display: block;
      margin-bottom: 0.5rem;
      color: #334155;
      font-weight: 600;
      font-size: 0.9rem;
    }

    .form-input,
    .form-select {
      width: 100%;
      padding: 0.875rem 1rem;
      border: 2px solid #e2e8f0;
      border-radius: 8px;
      font-size: 1rem;
      transition: all 0.2s;
      background: white;
    }

    .form-input:focus,
    .form-select:focus {
      outline: none;
      border-color: #3b82f6;
      box-shadow: 0 0 0 4px rgba(59, 130, 246, 0.1);
    }

    .form-input.ng-invalid.ng-touched,
    .form-select.ng-invalid.ng-touched {
      border-color: #ef4444;
    }

    .validation-error {
      margin-top: 0.5rem;
      color: #ef4444;
      font-size: 0.875rem;
      font-weight: 500;
    }

    .readonly-field {
      padding: 0.875rem 1rem;
      background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
      border: 2px solid #e2e8f0;
      border-radius: 8px;
      color: #1e293b;
      font-weight: 600;
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .project-icon {
      font-size: 1.2rem;
    }

    .section-divider {
      margin: 2.5rem 0 1.5rem;
      padding-bottom: 0.75rem;
      border-bottom: 2px solid #e2e8f0;
    }

    .section-divider h3 {
      margin: 0 0 0.25rem;
      color: #1e293b;
      font-size: 1.25rem;
      font-weight: 700;
    }

    .section-description {
      margin: 0;
      color: #64748b;
      font-size: 0.9rem;
    }

    .ratings-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1rem;
    }

    /* Components Step */
    .components-step {
      min-height: 300px;
    }

    .step-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 1.5rem;
      gap: 1rem;
    }

    .step-header h3 {
      margin: 0 0 0.25rem;
      color: #1e293b;
      font-size: 1.25rem;
      font-weight: 700;
    }

    .step-description {
      margin: 0;
      color: #64748b;
      font-size: 0.9rem;
    }

    .component-form-card {
      background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
      border: 2px solid #e2e8f0;
      border-radius: 12px;
      padding: 1.5rem;
      margin-bottom: 1.5rem;
    }

    .component-form-card h4 {
      margin: 0 0 1rem;
      color: #1e293b;
      font-size: 1.1rem;
      font-weight: 600;
    }

    .form-row {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 1rem;
      margin-bottom: 1rem;
    }

    .form-actions {
      display: flex;
      gap: 0.75rem;
      justify-content: flex-end;
      margin-top: 1.5rem;
    }

    .components-table {
      width: 100%;
      border-collapse: collapse;
      background: white;
      border-radius: 8px;
      overflow: hidden;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    }

    .components-table th {
      background: linear-gradient(135deg, #1e293b 0%, #334155 100%);
      color: white;
      padding: 1rem;
      text-align: left;
      font-weight: 600;
      font-size: 0.85rem;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .components-table td {
      padding: 1rem;
      border-bottom: 1px solid #f1f5f9;
      color: #475569;
    }

    .components-table tbody tr:hover {
      background-color: #f8fafc;
    }

    .type-badge {
      padding: 0.35rem 0.75rem;
      border-radius: 20px;
      font-size: 0.8rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.3px;
    }

    .type-new {
      background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%);
      color: #1e40af;
    }

    .type-reused {
      background: linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%);
      color: #065f46;
    }

    .type-modified {
      background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%);
      color: #92400e;
    }

    .components-summary {
      margin-top: 1rem;
      padding: 1rem 1.5rem;
      background: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%);
      border-radius: 8px;
      display: flex;
      justify-content: flex-end;
      align-items: center;
      gap: 0.75rem;
    }

    .summary-label {
      color: #0369a1;
      font-weight: 600;
      font-size: 0.95rem;
    }

    .summary-value {
      color: #0c4a6e;
      font-size: 1.5rem;
      font-weight: 700;
    }

    .empty-state {
      text-align: center;
      padding: 3rem 2rem;
      color: #64748b;
    }

    .empty-icon {
      font-size: 4rem;
      margin-bottom: 1rem;
      opacity: 0.5;
    }

    .empty-state p {
      margin: 0.5rem 0;
    }

    .empty-hint {
      font-size: 0.9rem;
      color: #94a3b8;
    }

    .modal-footer {
      padding: 1.5rem 2rem;
      border-top: 1px solid #e2e8f0;
      background: #f8fafc;
      border-radius: 0 0 16px 16px;
    }

    .footer-actions {
      display: flex;
      gap: 1rem;
      justify-content: flex-end;
    }

    .btn {
      padding: 0.75rem 1.75rem;
      border: none;
      border-radius: 8px;
      font-size: 1rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s;
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
    }

    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .btn-sm {
      padding: 0.5rem 1.25rem;
      font-size: 0.9rem;
    }

    .btn-primary {
      background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
      color: white;
      box-shadow: 0 4px 12px rgba(59, 130, 246, 0.3);
    }

    .btn-primary:hover:not(:disabled) {
      background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
      box-shadow: 0 6px 16px rgba(59, 130, 246, 0.4);
      transform: translateY(-1px);
    }

    .btn-secondary {
      background: white;
      color: #475569;
      border: 2px solid #e2e8f0;
    }

    .btn-secondary:hover {
      background: #f8fafc;
      border-color: #cbd5e1;
    }

    .btn-icon {
      background: none;
      border: none;
      padding: 0.5rem;
      cursor: pointer;
      border-radius: 6px;
      transition: all 0.2s;
      font-size: 1.1rem;
    }

    .btn-icon:hover {
      background-color: #f1f5f9;
    }

    .btn-delete:hover {
      background-color: #fee2e2;
    }

    @media (max-width: 768px) {
      .modal-content {
        max-width: 100%;
        max-height: 100vh;
        border-radius: 0;
      }

      .ratings-grid,
      .form-row {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class Cocomo2Stage1EstimationDialogComponent implements OnInit {
  @Input() estimation: Cocomo2Stage1Estimation | null = null;
  @Input() project: Project | null = null;
  @Output() save = new EventEmitter<{ 
    estimation: CreateCocomo2Stage1EstimationRequest; 
    components: CreateComponentRequest[] 
  }>();
  @Output() cancel = new EventEmitter<void>();

  private estimationService = inject(Cocomo2Stage1EstimationService);
  private languageService = inject(LanguageService);

  loading = signal(false);
  languages = signal<Language[]>([]);
  defaultParamSets = signal<ParameterSet[]>([]);
  userParamSets = signal<ParameterSet[]>([]);
  currentStep = signal<'basic' | 'components'>('basic');
  tempComponents = signal<CreateComponentRequest[]>([]);
  showComponentForm = signal(false);

  newComponent: CreateComponentRequest = {
    componentName: '',
    componentType: 'new',
    sizeFp: 0,
    reusePercent: 0,
    changePercent: 0
  };

  formData: CreateCocomo2Stage1EstimationRequest = {
    projectId: 0,
    estimationName: '',
    paramSetId: 0,
    languageId: 0,
    selectedAexp: 'nominal',
    selectedPexp: 'nominal',
    selectedPrec: 'nominal',
    selectedRely: 'nominal',
    selectedTmsp: 'nominal'
  };

  ngOnInit() {
    console.log('Dialog initialized with project:', this.project);
    console.log('Dialog initialized with estimation:', this.estimation);

    if (this.project) {
      this.formData.projectId = this.project.projectId;
      console.log('Set projectId to:', this.formData.projectId);
    }

    if (this.estimation) {
      this.formData = {
        projectId: this.estimation.projectId,
        estimationName: this.estimation.estimationName,
        paramSetId: this.estimation.parameterSet?.paramSetId || 0,
        languageId: this.estimation.language?.languageId || 0,
        selectedAexp: this.estimation.selectedAexp,
        selectedPexp: this.estimation.selectedPexp,
        selectedPrec: this.estimation.selectedPrec,
        selectedRely: this.estimation.selectedRely,
        selectedTmsp: this.estimation.selectedTmsp
      };
    }

    this.loadData();
  }

  isEditMode(): boolean {
    return this.estimation !== null;
  }

  loadData() {
    this.loading.set(true);

    // Load languages
    this.languageService.getLanguages().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.languages.set(response.data);
        }
      },
      error: (err) => console.error('Error loading languages:', err)
    });

    // Load default parameter sets
    this.estimationService.getDefaultParameterSets().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.defaultParamSets.set(response.data);
        }
      },
      error: (err) => console.error('Error loading default parameter sets:', err)
    });

    // Load user parameter sets
    this.estimationService.getUserParameterSets().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.userParamSets.set(response.data);
        }
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading user parameter sets:', err);
        this.loading.set(false);
      }
    });
  }

  onSubmit() {
    console.log('Form submitted with data:', this.formData);
    console.log('Components:', this.tempComponents());

    // Validate required fields
    if (!this.formData.estimationName || this.formData.estimationName.trim() === '') {
      alert('El nombre de la estimación es obligatorio');
      return;
    }

    if (!this.formData.languageId || this.formData.languageId === 0) {
      alert('Debe seleccionar un lenguaje de programación');
      return;
    }

    if (!this.formData.paramSetId || this.formData.paramSetId === 0) {
      alert('Debe seleccionar un conjunto de parámetros');
      return;
    }

    if (!this.formData.projectId || this.formData.projectId === 0) {
      alert('Error: No se ha establecido el proyecto');
      return;
    }

    if (this.tempComponents().length === 0) {
      alert('Debe agregar al menos un componente');
      return;
    }

    // Ensure all values are properly typed
    const requestData: CreateCocomo2Stage1EstimationRequest = {
      projectId: Number(this.formData.projectId),
      estimationName: this.formData.estimationName.trim(),
      paramSetId: Number(this.formData.paramSetId),
      languageId: Number(this.formData.languageId),
      selectedAexp: this.formData.selectedAexp || 'nominal',
      selectedPexp: this.formData.selectedPexp || 'nominal',
      selectedPrec: this.formData.selectedPrec || 'nominal',
      selectedRely: this.formData.selectedRely || 'nominal',
      selectedTmsp: this.formData.selectedTmsp || 'nominal'
    };

    console.log('Emitting save event with:', requestData);
    this.save.emit({ estimation: requestData, components: this.tempComponents() });
  }

  onCancel() {
    this.cancel.emit();
  }

  isBasicFormValid(): boolean {
    return !!(
      this.formData.estimationName &&
      this.formData.estimationName.trim().length >= 3 &&
      this.formData.languageId > 0 &&
      this.formData.paramSetId > 0
    );
  }

  goToComponents() {
    if (this.isBasicFormValid()) {
      this.currentStep.set('components');
    }
  }

  goBackToBasic() {
    this.currentStep.set('basic');
  }

  addTempComponent() {
    if (!this.newComponent.componentName || this.newComponent.sizeFp <= 0) {
      alert('Complete los campos obligatorios');
      return;
    }

    this.tempComponents.update(list => [...list, { ...this.newComponent }]);
    this.resetNewComponent();
    this.showComponentForm.set(false);
  }

  removeTempComponent(index: number) {
    this.tempComponents.update(list => list.filter((_, i) => i !== index));
  }

  cancelComponentForm() {
    this.resetNewComponent();
    this.showComponentForm.set(false);
  }

  getTotalFP(): number {
    return this.tempComponents().reduce((sum, c) => sum + c.sizeFp, 0);
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
