# Parameter Editor - COCOMO II

## Descripción
El **Parameter Editor** es un componente que permite a los usuarios editar las calificaciones de Factores de Escala (SF) y Multiplicadores de Esfuerzo (EM) después de crear una estimación.

## Características

### 1. **Modo Visualización**
- Muestra todas las calificaciones actuales en badges
- Muestra Sum SF y EAF calculados
- Botón "Editar Calificaciones" para activar el modo edición

### 2. **Modo Edición**
- Dropdowns para cambiar cada calificación SF y EM
- Valores en español (Extra Bajo, Muy Bajo, Bajo, Nominal, Alto, Muy Alto, Extra Alto)
- Validación en tiempo real
- Botón "Guardar Cambios" con indicador de carga
- Botón "Cancelar" para descartar cambios

### 3. **Factores de Escala (5 campos)**
- **PREC** - Precedentedness
- **FLEX** - Development Flexibility
- **RESL** - Architecture/Risk Resolution
- **TEAM** - Team Cohesion
- **PMAT** - Process Maturity

Opciones: VLO, LO, NOM, HI, VHI, XHI (Muy Bajo → Extra Alto)

### 4. **Multiplicadores de Esfuerzo (7 campos)**
- **PERS** - Personnel Capability
- **RCPX** - Product Reliability and Complexity
- **PDIF** - Platform Difficulty
- **PREX** - Personnel Experience
- **RUSE** - Reusability
- **FCIL** - Facilities
- **SCED** - Required Development Schedule

Opciones: XLO, VLO, LO, NOM, HI, VHI, XHI (Extra Bajo → Extra Alto)

### 5. **Integración Automática**
- Al guardar, el backend recalcula automáticamente:
  - Sum SF
  - EAF (Effort Adjustment Factor)
  - Effort PM
  - Duration (Tdev Months)
  - Team Size
- La página se actualiza automáticamente con los nuevos valores

## Ubicación en la UI
El Parameter Editor se muestra en la página **Estimation Detail**, justo después de la sección de resultados y antes de la entrada de Function Points.

## Flujo de Usuario

```
1. Usuario ve estimación con calificaciones iniciales
2. Usuario hace clic en "Editar Calificaciones"
3. Dropdowns se habilitan mostrando valores actuales
4. Usuario modifica las calificaciones deseadas
5. Usuario hace clic en "Guardar Cambios"
6. Backend recalcula Effort, Duration, Team Size
7. UI se actualiza mostrando nuevos resultados
```

## API Utilizada

### Endpoint
```
PUT /Projects/{projectId}/Estimations/{estimationId}/Ratings
```

### Request Body
```typescript
{
  selectedSfPrec?: string;
  selectedSfFlex?: string;
  selectedSfResl?: string;
  selectedSfTeam?: string;
  selectedSfPmat?: string;
  selectedEmPers?: string;
  selectedEmRcpx?: string;
  selectedEmPdif?: string;
  selectedEmPrex?: string;
  selectedEmRuse?: string;
  selectedEmFcil?: string;
  selectedEmSced?: string;
}
```

### Response
```typescript
ApiResponse<Estimation> // Estimation actualizada con nuevos cálculos
```

## Archivos Creados

1. **parameter-editor.component.ts** - Lógica del componente
2. **parameter-editor.component.html** - Template con formulario
3. **parameter-editor.component.css** - Estilos responsive

## Componente Principal Modificado
- **estimation-detail.component.ts** - Integrado Parameter Editor con evento `onParametersUpdated()`

## Manejo de Errores
- Mensajes de error en español
- Validación de respuesta del API
- Estado de carga durante guardado
- Opción de cancelar y revertir cambios

## Diseño Responsive
- Grid layout adaptativo
- En móviles: 1 columna
- En desktop: múltiples columnas (auto-fill)
- Botones se ajustan al ancho en móvil

## Estados del Componente

### Signals Utilizados
```typescript
isEditing = signal(false);     // Modo edición activado/desactivado
isSaving = signal(false);      // Guardando cambios al backend
errorMessage = signal<string | null>(null); // Mensaje de error
```

### Working Copy
```typescript
ratings: UpdateEstimationRatingsRequest = {}; // Copia de trabajo de las calificaciones
```

## Beneficios
✅ Permite ajustar parámetros COCOMO después de la creación inicial  
✅ Facilita el análisis "qué pasa si" cambiando calificaciones  
✅ Recalculación automática sin necesidad de recrear la estimación  
✅ UI intuitiva con valores en español  
✅ Integración perfecta con el flujo existente  

## Próximos Pasos Sugeridos
- Agregar tooltips explicando cada factor SF/EM
- Mostrar historial de cambios de calificaciones
- Comparación lado a lado: valores anteriores vs nuevos
- Exportar análisis de sensibilidad
