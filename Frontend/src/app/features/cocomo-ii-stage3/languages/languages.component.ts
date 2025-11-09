import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { LanguageService } from '../../../core/services/cocomo-ii-stage3/language.service';
import { ApiResponse, Language } from '../../../core/models/cocomo-ii-stage3/cocomo-ii-stage3.models';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';

@Component({
  selector: 'app-languages',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  template: `
    <app-navbar></app-navbar>

    <div class="languages-container">
      <div class="header-section">
        <div class="header-content">
          <div>
            <h1>🔤 Lenguajes de Programación</h1>
            <p>Base de datos de lenguajes COCOMO II con factores de conversión SLOC</p>
          </div>
          <button class="btn btn-primary" (click)="createLanguage()">
            <i class="bi bi-plus-lg"></i>
            Crear Lenguaje
          </button>
        </div>
      </div>

      <!-- Loading State -->
      <div *ngIf="loading()" class="loading-state">
        <div class="spinner"></div>
        <p>Cargando lenguajes de programación...</p>
      </div>

      <!-- Error State -->
      <div *ngIf="error()" class="error-alert">
        <strong>⚠️ Error:</strong> {{ error() }}
        <button class="btn btn-sm btn-outline-primary ms-2" (click)="loadLanguages()">
          Reintentar
        </button>
      </div>

      <!-- Languages Table -->
      <div *ngIf="!loading() && !error()" class="languages-content">
        <div class="info-box">
          <h3>ℹ️ Acerca de SLOC por UFP</h3>
          <p>
            <strong>SLOC por UFP</strong> (Líneas de Código Fuente por Punto de Función sin Ajustar) es un factor de conversión
            utilizado para traducir Puntos de Función en líneas de código estimadas. Diferentes lenguajes de programación requieren
            diferentes cantidades de código para implementar la misma funcionalidad.
          </p>
          <p>
            Por ejemplo, un lenguaje de alto nivel como Python generalmente requiere menos líneas de código en comparación con un
            lenguaje de nivel más bajo como Assembly para implementar la misma característica.
          </p>
        </div>

        <div class="table-container">
          <table class="languages-table">
            <thead>
              <tr>
                <th>#</th>
                <th>Nombre del Lenguaje</th>
                <th>SLOC por UFP</th>
                <th>Verbosidad Relativa</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let language of languages(); let i = index" class="language-row">
                <td>{{ i + 1 }}</td>
                <td class="language-name">
                  <span class="language-icon">💻</span>
                  {{ language.name }}
                </td>
                <td class="sloc-value">
                  <span class="badge badge-sloc">{{ language.slocPerUfp }}</span>
                </td>
                <td>
                  <div class="verbosity-bar">
                    <div 
                      class="verbosity-fill" 
                      [style.width.%]="getVerbosityPercentage(language.slocPerUfp)"
                      [class.low]="language.slocPerUfp < 50"
                      [class.medium]="language.slocPerUfp >= 50 && language.slocPerUfp < 100"
                      [class.high]="language.slocPerUfp >= 100">
                    </div>
                  </div>
                  <small class="verbosity-label">{{ getVerbosityLabel(language.slocPerUfp) }}</small>
                </td>
                <td>
                  <div class="action-buttons">
                    <button 
                      class="btn btn-sm btn-outline-primary"
                      (click)="viewLanguageDetails(language)"
                      title="Ver Detalles">
                      <i class="bi bi-eye"></i>
                    </button>
                    <button 
                      class="btn btn-sm btn-outline-secondary"
                      (click)="editLanguage(language)"
                      title="Editar">
                      <i class="bi bi-pencil"></i>
                    </button>
                    <button 
                      class="btn btn-sm btn-outline-danger"
                      (click)="confirmDelete(language)"
                      title="Eliminar">
                      <i class="bi bi-trash"></i>
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Statistics -->
        <div class="statistics-section">
          <h3>📊 Estadísticas de Lenguajes</h3>
          <div class="stats-grid">
            <div class="stat-card">
              <div class="stat-icon">🔢</div>
              <div class="stat-value">{{ languages().length }}</div>
              <div class="stat-label">Total de Lenguajes</div>
            </div>
            <div class="stat-card">
              <div class="stat-icon">⬇️</div>
              <div class="stat-value">{{ getMinSlocLanguage()?.name || 'N/A' }}</div>
              <div class="stat-label">Más Conciso</div>
              <div class="stat-sublabel">{{ getMinSlocLanguage()?.slocPerUfp || 'N/A' }} SLOC/UFP</div>
            </div>
            <div class="stat-card">
              <div class="stat-icon">⬆️</div>
              <div class="stat-value">{{ getMaxSlocLanguage()?.name || 'N/A' }}</div>
              <div class="stat-label">Más Verboso</div>
              <div class="stat-sublabel">{{ getMaxSlocLanguage()?.slocPerUfp || 'N/A' }} SLOC/UFP</div>
            </div>
            <div class="stat-card">
              <div class="stat-icon">📈</div>
              <div class="stat-value">{{ getAverageSlocPerUfp() }}</div>
              <div class="stat-label">Promedio SLOC/UFP</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div *ngIf="!loading() && !error() && languages().length === 0" class="empty-state">
        <div class="empty-icon">🔤</div>
        <h3>No se Encontraron Lenguajes</h3>
        <p>No hay lenguajes de programación disponibles actualmente en la base de datos.</p>
      </div>
    </div>
  `,
  styles: [`
    .languages-container {
      min-height: 100vh;
      background: #f8f9fa;
      padding: 2rem;
    }

    .header-section {
      background: white;
      padding: 2rem;
      border-radius: 12px;
      margin-bottom: 2rem;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .header-content {
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 2rem;
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

    .languages-content {
      display: flex;
      flex-direction: column;
      gap: 2rem;
    }

    .info-box {
      background: #e3f2fd;
      border-left: 4px solid #1976d2;
      padding: 1.5rem;
      border-radius: 8px;
    }

    .info-box h3 {
      margin: 0 0 1rem 0;
      color: #1976d2;
      font-size: 1.2rem;
    }

    .info-box p {
      margin: 0 0 0.75rem 0;
      color: #555;
      line-height: 1.6;
    }

    .info-box p:last-child {
      margin-bottom: 0;
    }

    .table-container {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      overflow: hidden;
    }

    .languages-table {
      width: 100%;
      border-collapse: collapse;
    }

    .languages-table thead {
      background: #667eea;
      color: white;
    }

    .languages-table th {
      padding: 1rem;
      text-align: left;
      font-weight: 600;
    }

    .languages-table th:first-child {
      width: 60px;
    }

    .languages-table th:last-child {
      width: 120px;
      text-align: center;
    }

    .languages-table tbody tr {
      border-bottom: 1px solid #e1e5e9;
      transition: background-color 0.2s;
    }

    .languages-table tbody tr:hover {
      background-color: #f8f9fa;
    }

    .languages-table tbody tr:last-child {
      border-bottom: none;
    }

    .languages-table td {
      padding: 1rem;
    }

    .language-name {
      font-weight: 500;
      color: #333;
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .language-icon {
      font-size: 1.2rem;
    }

    .sloc-value {
      text-align: center;
    }

    .badge-sloc {
      background: #e3f2fd;
      color: #1976d2;
      padding: 0.375rem 0.75rem;
      border-radius: 20px;
      font-weight: 600;
      font-size: 0.9rem;
    }

    .verbosity-bar {
      width: 100%;
      height: 8px;
      background: #e9ecef;
      border-radius: 4px;
      overflow: hidden;
      margin-bottom: 0.25rem;
    }

    .verbosity-fill {
      height: 100%;
      transition: width 0.3s ease;
    }

    .verbosity-fill.low {
      background: linear-gradient(90deg, #4caf50, #66bb6a);
    }

    .verbosity-fill.medium {
      background: linear-gradient(90deg, #ff9800, #ffa726);
    }

    .verbosity-fill.high {
      background: linear-gradient(90deg, #f44336, #ef5350);
    }

    .verbosity-label {
      color: #666;
      font-size: 0.8rem;
    }

    .statistics-section {
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .statistics-section h3 {
      margin: 0 0 1.5rem 0;
      color: #333;
      font-size: 1.5rem;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1.5rem;
    }

    .stat-card {
      background: #f8f9fa;
      padding: 1.5rem;
      border-radius: 8px;
      text-align: center;
      border: 1px solid #e1e5e9;
    }

    .stat-icon {
      font-size: 2rem;
      margin-bottom: 0.5rem;
    }

    .stat-value {
      font-size: 1.5rem;
      font-weight: bold;
      color: #333;
      margin-bottom: 0.5rem;
    }

    .stat-label {
      color: #666;
      font-size: 0.9rem;
      font-weight: 500;
    }

    .stat-sublabel {
      color: #999;
      font-size: 0.8rem;
      margin-top: 0.25rem;
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
      background: white;
    }

    .btn-primary {
      background: #667eea;
      color: white;
      border-color: #667eea;
    }

    .btn-primary:hover {
      background: #5568d3;
      border-color: #5568d3;
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

    .action-buttons {
      display: flex;
      gap: 0.5rem;
      justify-content: center;
    }

    @media (max-width: 768px) {
      .languages-container {
        padding: 1rem;
      }

      .header-content {
        flex-direction: column;
        align-items: flex-start;
      }

      .btn-primary {
        width: 100%;
      }

      .table-container {
        overflow-x: auto;
      }

      .languages-table {
        min-width: 600px;
      }

      .stats-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class LanguagesComponent implements OnInit {
  private languageService = inject(LanguageService);
  private router = inject(Router);

  languages = signal<Language[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  ngOnInit() {
    this.loadLanguages();
  }

  loadLanguages() {
    this.loading.set(true);
    this.error.set(null);

    this.languageService.getLanguages().subscribe({
      next: (response: ApiResponse<Language[]>) => {
        this.loading.set(false);
        if (response.success && response.data) {
          // Sort languages by SLOC per UFP (ascending)
          const sortedLanguages = response.data.sort((a: Language, b: Language) => a.slocPerUfp - b.slocPerUfp);
          this.languages.set(sortedLanguages);
        } else {
          this.error.set(response.errors?.[0] || 'Failed to load languages');
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set('Failed to load languages. Please try again.');
        console.error('Error loading languages:', err);
      }
    });
  }

  viewLanguageDetails(language: Language) {
    this.router.navigate(['/cocomo2/languages', language.languageId]);
  }

  createLanguage() {
    this.router.navigate(['/cocomo2/languages/new']);
  }

  editLanguage(language: Language) {
    this.router.navigate(['/cocomo2/languages', language.languageId, 'edit']);
  }

  confirmDelete(language: Language) {
    if (confirm(`¿Estás seguro de que deseas eliminar el lenguaje "${language.name}"?\n\nEsta acción no se puede deshacer.`)) {
      this.deleteLanguage(language.languageId);
    }
  }

  deleteLanguage(languageId: number) {
    this.languageService.deleteLanguage(languageId).subscribe({
      next: (response: ApiResponse<void>) => {
        if (response.success) {
          // Remove language from the list
          this.languages.update(langs => langs.filter(l => l.languageId !== languageId));
          // Optionally show success message
          console.log('Language deleted successfully');
        } else {
          this.error.set(response.errors?.[0] || 'Error al eliminar el lenguaje');
        }
      },
      error: (err) => {
        this.error.set('Error al eliminar el lenguaje. Por favor intenta de nuevo.');
        console.error('Error deleting language:', err);
      }
    });
  }

  getVerbosityPercentage(slocPerUfp: number): number {
    // Calculate percentage based on max value (let's assume 320 as max for scaling)
    const maxSloc = 320;
    return Math.min((slocPerUfp / maxSloc) * 100, 100);
  }

  getVerbosityLabel(slocPerUfp: number): string {
    if (slocPerUfp < 50) return 'Bajo (Conciso)';
    if (slocPerUfp < 100) return 'Medio';
    return 'Alto (Verboso)';
  }

  getMinSlocLanguage(): Language | undefined {
    if (this.languages().length === 0) return undefined;
    return this.languages().reduce((min, lang) => 
      lang.slocPerUfp < min.slocPerUfp ? lang : min
    );
  }

  getMaxSlocLanguage(): Language | undefined {
    if (this.languages().length === 0) return undefined;
    return this.languages().reduce((max, lang) => 
      lang.slocPerUfp > max.slocPerUfp ? lang : max
    );
  }

  getAverageSlocPerUfp(): string {
    if (this.languages().length === 0) return 'N/A';
    const sum = this.languages().reduce((total, lang) => total + lang.slocPerUfp, 0);
    const avg = sum / this.languages().length;
    return avg.toFixed(1);
  }
}