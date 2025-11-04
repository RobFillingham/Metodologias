import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, NavbarComponent],
  template: `
    <app-navbar></app-navbar>

    <div class="home-container">
      <!-- Hero Section -->
      <header class="hero">
        <h1>Herramienta de Estimación de Proyectos de Software</h1>
        <p class="subtitle">Métodos COCOMO I, II, III, KLOC y otras técnicas de estimación de tiempo y esfuerzo</p>

        <div class="hero-actions" *ngIf="!authService.isAuthenticated">
          <a routerLink="/auth/login" class="btn btn-primary">Iniciar Sesión</a>
          <a routerLink="/auth/register" class="btn btn-secondary">Registrarse</a>
        </div>

        <div class="hero-actions" *ngIf="authService.isAuthenticated">
          <p class="welcome-message">
            ¡Bienvenido de vuelta, {{ authService.currentUserValue?.firstName }}!
          </p>
          <button (click)="logout()" class="btn btn-secondary">Cerrar Sesión</button>
        </div>
      </header>

      <!-- Introduction Section -->
      <section class="introduction">
        <div class="container">
          <h2>¿Qué es la Estimación de Software?</h2>
          <p>
            La estimación de software es el proceso de predecir el esfuerzo, tiempo y costo requerido
            para desarrollar un proyecto de software. Una buena estimación es crucial para la planificación,
            presupuestación y gestión exitosa de proyectos.
          </p>
        </div>
      </section>

      <!-- COCOMO Methods Section -->
      <section class="methods-section">
        <div class="container">
          <h2>Métodos COCOMO</h2>

          <!-- COCOMO I -->
          <div class="method-card">
            <div class="method-header">
              <h3>🧮 COCOMO I (1981)</h3>
              <span class="method-type">Modelo Original</span>
            </div>
            <div class="method-content">
              <p>
                <strong>Constructive Cost Model</strong> desarrollado por Barry Boehm en 1981.
                Es el modelo fundamental de estimación de costo y esfuerzo en desarrollo de software.
              </p>
              <div class="method-details">
                <h4>Características Principales:</h4>
                <ul>
                  <li>Basado en líneas de código fuente (KLOC)</li>
                  <li>Tres modos de desarrollo: Orgánico, Semi-acoplado, Empotrado</li>
                  <li>Fórmula básica: Esfuerzo = a × (KLOC)^b</li>
                  <li>Tiempo = c × (Esfuerzo)^d</li>
                </ul>
              </div>
            </div>
          </div>

          <!-- COCOMO II -->
          <div class="method-card">
            <div class="method-header">
              <h3>🚀 COCOMO II (2000)</h3>
              <span class="method-type">Modelo Actualizado</span>
            </div>
            <div class="method-content">
              <p>
                Versión mejorada del modelo COCOMO original, adaptada para el desarrollo moderno
                de software con metodologías ágiles y orientadas a objetos.
              </p>
              <div class="method-details">
                <h4>Mejoras sobre COCOMO I:</h4>
                <ul>
                  <li>18 factores de escala (Scale Factors)</li>
                  <li>17 multiplicadores de esfuerzo (Effort Multipliers)</li>
                  <li>Soporte para desarrollo incremental y reutilización</li>
                  <li>Adaptado para lenguajes modernos y herramientas</li>
                </ul>
              </div>
            </div>
          </div>

          <!-- COCOMO III -->
          <div class="method-card">
            <div class="method-header">
              <h3>🔮 COCOMO III (Próximamente)</h3>
              <span class="method-type">Modelo Avanzado</span>
            </div>
            <div class="method-content">
              <p>
                La próxima evolución del modelo COCOMO, actualmente en desarrollo.
                Incorporará inteligencia artificial y aprendizaje automático para mejorar
                la precisión de las estimaciones.
              </p>
              <div class="method-details">
                <h4>Características Esperadas:</h4>
                <ul>
                  <li>Integración con IA y machine learning</li>
                  <li>Análisis de datos históricos más sofisticado</li>
                  <li>Estimación en tiempo real durante el desarrollo</li>
                  <li>Soporte para DevOps y CI/CD</li>
                </ul>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Other Methods Section -->
      <section class="other-methods">
        <div class="container">
          <h2>Otras Técnicas de Estimación</h2>

          <div class="methods-grid">
            <!-- KLOC -->
            <div class="method-card">
              <h3>� KLOC (Miles de Líneas de Código)</h3>
              <p>
                Método tradicional que mide el tamaño del software en miles de líneas de código fuente.
                Es la base de los modelos COCOMO pero tiene limitaciones en metodologías modernas.
              </p>
              <div class="method-pros-cons">
                <div class="pros">
                  <strong>✅ Ventajas:</strong>
                  <ul>
                    <li>Fácil de medir</li>
                    <li>Históricamente probado</li>
                    <li>Base para modelos complejos</li>
                  </ul>
                </div>
                <div class="cons">
                  <strong>❌ Limitaciones:</strong>
                  <ul>
                    <li>Depende del lenguaje de programación</li>
                    <li>No mide funcionalidad</li>
                    <li>Difícil en desarrollo ágil</li>
                  </ul>
                </div>
              </div>
            </div>

            <!-- Function Points -->
            <div class="method-card">
              <h3>🎯 Puntos de Función (Function Points)</h3>
              <p>
                Técnica desarrollada por Allan Albrecht en 1979. Mide el tamaño funcional del software
                basado en las funcionalidades que proporciona al usuario final.
              </p>
              <div class="method-pros-cons">
                <div class="pros">
                  <strong>✅ Ventajas:</strong>
                  <ul>
                    <li>Independiente del lenguaje</li>
                    <li>Mide funcionalidad, no código</li>
                    <li>Útil para mantenimiento</li>
                  </ul>
                </div>
                <div class="cons">
                  <strong>❌ Limitaciones:</strong>
                  <ul>
                    <li>Requiere experiencia</li>
                    <li>Subjetivo en algunos casos</li>
                    <li>Complejo de aprender</li>
                  </ul>
                </div>
              </div>
            </div>

            <!-- Use Case Points -->
            <div class="method-card">
              <h3>📋 Puntos de Caso de Uso (Use Case Points)</h3>
              <p>
                Adaptación de los puntos de función para desarrollo orientado a objetos y UML.
                Basado en la complejidad de los casos de uso del sistema.
              </p>
              <div class="method-pros-cons">
                <div class="pros">
                  <strong>✅ Ventajas:</strong>
                  <ul>
                    <li>Adecuado para OO y UML</li>
                    <li>Basado en requerimientos</li>
                    <li>Fácil de calcular temprano</li>
                  </ul>
                </div>
                <div class="cons">
                  <strong>❌ Limitaciones:</strong>
                  <ul>
                    <li>Requiere casos de uso bien definidos</li>
                    <li>Menos preciso que FP</li>
                    <li>Limitado a desarrollo OO</li>
                  </ul>
                </div>
              </div>
            </div>

            <!-- Expert Judgment -->
            <div class="method-card">
              <h3>� Juicio de Expertos</h3>
              <p>
                Método basado en la experiencia y conocimiento de profesionales expertos.
                Puede combinarse con otras técnicas para mejorar la precisión.
              </p>
              <div class="method-pros-cons">
                <div class="pros">
                  <strong>✅ Ventajas:</strong>
                  <ul>
                    <li>Rápido y económico</li>
                    <li>Considera factores cualitativos</li>
                    <li>Útil cuando faltan datos</li>
                  </ul>
                </div>
                <div class="cons">
                  <strong>❌ Limitaciones:</strong>
                  <ul>
                    <li>Subjetivo y variable</li>
                    <li>Depende de la experiencia</li>
                    <li>Difícil de repetir</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Features Section -->
      <section class="features">
        <div class="container">
          <h2>Características de Nuestra Herramienta</h2>
          <div class="features-grid">
            <div class="feature-card">
              <h3>🎯 Estimaciones Precisas</h3>
              <p>Utiliza modelos COCOMO II probados industrialmente para estimaciones confiables</p>
            </div>

            <div class="feature-card">
              <h3>📊 Análisis de Puntos de Función</h3>
              <p>Cálculo completo de puntos de función con evaluación automática de complejidad</p>
            </div>

            <div class="feature-card">
              <h3>📈 Factores de Escala y Multiplicadores</h3>
              <p>Personaliza los parámetros de estimación según las necesidades de tu organización</p>
            </div>

            <div class="feature-card">
              <h3>📋 Seguimiento de Proyectos</h3>
              <p>Rastrea múltiples proyectos y compara estimaciones vs. resultados reales</p>
            </div>

            <div class="feature-card">
              <h3>🔒 Autenticación Segura</h3>
              <p>Sistema de autenticación JWT para proteger tus datos y estimaciones</p>
            </div>

            <div class="feature-card">
              <h3>💾 Base de Datos Completa</h3>
              <p>Almacena proyectos, estimaciones y métricas históricas para análisis futuros</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Call to Action -->
      <section class="cta-section">
        <div class="container">
          <h2>¡Comienza a Estimar tus Proyectos!</h2>
          <p>
            Regístrate gratis y comienza a utilizar las mejores técnicas de estimación
            de software disponibles en la industria.
          </p>
          <div class="cta-actions" *ngIf="!authService.isAuthenticated">
            <a routerLink="/auth/register" class="btn btn-primary btn-large">Crear Cuenta Gratuita</a>
            <a routerLink="/auth/login" class="btn btn-secondary btn-large">Iniciar Sesión</a>
          </div>
          <div class="cta-actions" *ngIf="authService.isAuthenticated">
            <a routerLink="/dashboard" class="btn btn-primary btn-large">Ir al Dashboard</a>
          </div>
        </div>
      </section>

      <footer class="footer">
        <p>&copy; 2025 Herramienta de Estimación COCOMO. Desarrollado con Angular & ASP.NET Core.</p>
      </footer>
    </div>
  `,
  styles: [`
    .home-container {
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .hero {
      text-align: center;
      padding: 4rem 2rem 2rem;
      max-width: 1000px;
      margin: 0 auto;
    }

    h1 {
      font-size: 2.5rem;
      margin-bottom: 1rem;
      font-weight: 700;
      line-height: 1.2;
    }

    .subtitle {
      font-size: 1.25rem;
      margin-bottom: 2rem;
      opacity: 0.9;
      line-height: 1.4;
    }

    .hero-actions {
      display: flex;
      gap: 1rem;
      justify-content: center;
      align-items: center;
      flex-wrap: wrap;
    }

    .welcome-message {
      margin: 0;
      font-size: 1.1rem;
      opacity: 0.9;
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
      text-align: center;
    }

    .btn-primary {
      background: white;
      color: #667eea;
    }

    .btn-primary:hover {
      background: #f8f9fa;
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    }

    .btn-secondary {
      background: rgba(255, 255, 255, 0.1);
      color: white;
      border: 2px solid rgba(255, 255, 255, 0.3);
    }

    .btn-secondary:hover {
      background: rgba(255, 255, 255, 0.2);
      border-color: rgba(255, 255, 255, 0.5);
      transform: translateY(-2px);
    }

    .btn-large {
      padding: 1rem 2rem;
      font-size: 1.1rem;
      font-weight: 600;
    }

    /* Introduction Section */
    .introduction {
      padding: 3rem 2rem;
      background: rgba(255, 255, 255, 0.05);
    }

    .introduction h2 {
      text-align: center;
      font-size: 2rem;
      margin-bottom: 2rem;
      color: white;
    }

    .introduction p {
      max-width: 800px;
      margin: 0 auto;
      text-align: center;
      font-size: 1.1rem;
      line-height: 1.6;
      opacity: 0.9;
    }

    /* Methods Section */
    .methods-section {
      padding: 4rem 2rem;
      background: rgba(255, 255, 255, 0.02);
    }

    .methods-section h2 {
      text-align: center;
      font-size: 2.5rem;
      margin-bottom: 3rem;
      color: white;
    }

    .method-card {
      background: rgba(255, 255, 255, 0.1);
      backdrop-filter: blur(10px);
      border-radius: 16px;
      padding: 2rem;
      margin-bottom: 2rem;
      border: 1px solid rgba(255, 255, 255, 0.2);
      transition: transform 0.3s ease, box-shadow 0.3s ease;
    }

    .method-card:hover {
      transform: translateY(-5px);
      box-shadow: 0 8px 25px rgba(0, 0, 0, 0.2);
    }

    .method-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
      flex-wrap: wrap;
      gap: 1rem;
    }

    .method-header h3 {
      margin: 0;
      font-size: 1.5rem;
      color: white;
    }

    .method-type {
      background: rgba(255, 255, 255, 0.2);
      color: white;
      padding: 0.25rem 0.75rem;
      border-radius: 20px;
      font-size: 0.85rem;
      font-weight: 500;
    }

    .method-content p {
      margin-bottom: 1.5rem;
      line-height: 1.6;
      opacity: 0.9;
    }

    .method-details h4 {
      margin-bottom: 1rem;
      color: white;
      font-size: 1.1rem;
    }

    .method-details ul {
      margin: 0;
      padding-left: 1.5rem;
    }

    .method-details li {
      margin-bottom: 0.5rem;
      line-height: 1.5;
      opacity: 0.9;
    }

    /* Other Methods Section */
    .other-methods {
      padding: 4rem 2rem;
      background: rgba(255, 255, 255, 0.05);
    }

    .other-methods h2 {
      text-align: center;
      font-size: 2.5rem;
      margin-bottom: 3rem;
      color: white;
    }

    .methods-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
      gap: 2rem;
      max-width: 1200px;
      margin: 0 auto;
    }

    .method-pros-cons {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 1.5rem;
      margin-top: 1.5rem;
    }

    .pros, .cons {
      padding: 1rem;
      border-radius: 8px;
      font-size: 0.9rem;
    }

    .pros {
      background: rgba(76, 175, 80, 0.1);
      border: 1px solid rgba(76, 175, 80, 0.3);
    }

    .cons {
      background: rgba(244, 67, 54, 0.1);
      border: 1px solid rgba(244, 67, 54, 0.3);
    }

    .pros strong, .cons strong {
      display: block;
      margin-bottom: 0.5rem;
      font-size: 1rem;
    }

    .pros ul, .cons ul {
      margin: 0;
      padding-left: 1rem;
    }

    .pros li, .cons li {
      margin-bottom: 0.25rem;
      line-height: 1.4;
    }

    /* Features Section */
    .features {
      padding: 4rem 2rem;
      background: rgba(255, 255, 255, 0.02);
    }

    .features h2 {
      text-align: center;
      font-size: 2.5rem;
      margin-bottom: 3rem;
      color: white;
    }

    .features-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 2rem;
      max-width: 1200px;
      margin: 0 auto;
    }

    .feature-card {
      background: rgba(255, 255, 255, 0.1);
      backdrop-filter: blur(10px);
      border-radius: 12px;
      padding: 2rem;
      text-align: center;
      border: 1px solid rgba(255, 255, 255, 0.2);
      transition: transform 0.3s ease;
    }

    .feature-card:hover {
      transform: translateY(-5px);
    }

    .feature-card h3 {
      margin-bottom: 1rem;
      font-size: 1.25rem;
      color: white;
    }

    .feature-card p {
      opacity: 0.9;
      line-height: 1.6;
      margin: 0;
    }

    /* Call to Action Section */
    .cta-section {
      padding: 4rem 2rem;
      background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
      text-align: center;
    }

    .cta-section h2 {
      font-size: 2.5rem;
      margin-bottom: 1rem;
      color: white;
    }

    .cta-section p {
      font-size: 1.2rem;
      margin-bottom: 2rem;
      opacity: 0.9;
      max-width: 600px;
      margin-left: auto;
      margin-right: auto;
    }

    .cta-actions {
      display: flex;
      gap: 1rem;
      justify-content: center;
      flex-wrap: wrap;
    }

    .footer {
      text-align: center;
      padding: 2rem;
      opacity: 0.7;
      border-top: 1px solid rgba(255, 255, 255, 0.1);
      background: rgba(0, 0, 0, 0.1);
    }

    /* Container utility */
    .container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 1rem;
    }

    /* Responsive Design */
    @media (max-width: 768px) {
      h1 {
        font-size: 2rem;
      }

      .hero {
        padding: 3rem 1rem 1rem;
      }

      .hero-actions {
        flex-direction: column;
      }

      .methods-section, .other-methods, .features {
        padding: 3rem 1rem;
      }

      .methods-section h2, .other-methods h2, .features h2, .cta-section h2 {
        font-size: 2rem;
      }

      .methods-grid {
        grid-template-columns: 1fr;
      }

      .method-pros-cons {
        grid-template-columns: 1fr;
        gap: 1rem;
      }

      .features-grid {
        grid-template-columns: 1fr;
      }

      .cta-actions {
        flex-direction: column;
      }

      .method-card {
        padding: 1.5rem;
      }

      .feature-card {
        padding: 1.5rem;
      }
    }

    @media (max-width: 480px) {
      h1 {
        font-size: 1.75rem;
      }

      .subtitle {
        font-size: 1.1rem;
      }

      .method-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 0.5rem;
      }

      .method-type {
        align-self: flex-start;
      }
    }
  `]
})
export class HomeComponent {
  authService = inject(AuthService);
  private router = inject(Router);

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/home']);
  }
}
