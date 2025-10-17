# Documentación del Diseño de Base de Datos
## Sistema de Estimación de Proyectos de Software

---

## 📋 Índice
1. [Visión General](#visión-general)
2. [Diagrama Conceptual](#diagrama-conceptual)
3. [Descripción de Tablas](#descripción-de-tablas)
4. [Relaciones Principales](#relaciones-principales)
5. [Casos de Uso](#casos-de-uso)
6. [Instrucciones de Instalación](#instrucciones-de-instalación)

---

## 🎯 Visión General

Este diseño de base de datos soporta un sistema completo de estimación de proyectos de software que implementa cuatro métodos principales:

- **KLOC** (Thousand Lines of Code)
- **Puntos de Función** (Function Points)
- **Puntos de Casos de Uso** (Use Case Points)
- **COCOMO II** (Constructive Cost Model) - 3 estadios

### Características Principales

✅ **Multi-empresa**: Soporta múltiples empresas/clientes con configuraciones independientes
✅ **Configuración flexible**: Parámetros predeterminados y personalizables por empresa
✅ **Control de versiones**: Historial completo de estimaciones
✅ **Auditoría**: Registro de todos los cambios y accesos
✅ **Escalable**: Diseño normalizado y optimizado con índices

---

## 🗂️ Estructura de Tablas

### Categorías de Tablas

#### 1️⃣ **Gestión de Usuarios y Empresas**
- `Companies` - Empresas/clientes
- `Users` - Usuarios del sistema

#### 2️⃣ **Configuración de Parámetros**
- `ParameterConfigurations` - Configuraciones maestras (default o por empresa)

#### 3️⃣ **Parámetros COCOMO II**
- `CocomoCalibrationParameters` - Parámetros A, B, C, D
- `CocomoCostDrivers` - Definición de cost drivers
- `CocomoCostDriverValues` - Valores de multiplicadores
- `CocomoScaleFactors` - Factores de escala
- `CocomoScaleFactorValues` - Valores de factores de escala

#### 4️⃣ **Parámetros Puntos de Función**
- `FunctionPointILFComplexity` - Complejidad de archivos lógicos internos
- `FunctionPointEIFComplexity` - Complejidad de interfaces externas
- `FunctionPointEIComplexity` - Complejidad de entradas externas
- `FunctionPointEOComplexity` - Complejidad de salidas externas
- `FunctionPointEQComplexity` - Complejidad de consultas externas
- `FunctionPointTechnicalFactors` - Factores técnicos (F1-F14)
- `FunctionPointTechnicalFactorValues` - Valores de factores técnicos

#### 5️⃣ **Parámetros Puntos de Casos de Uso**
- `UseCasePointActorWeights` - Pesos de actores
- `UseCasePointUseCaseWeights` - Pesos de casos de uso
- `UseCasePointTechnicalFactors` - Factores técnicos (T1-T13)
- `UseCasePointTechnicalFactorValues` - Valores de factores técnicos
- `UseCasePointEnvironmentalFactors` - Factores ambientales (E1-E8)
- `UseCasePointEnvironmentalFactorValues` - Valores de factores ambientales

#### 6️⃣ **Parámetros KLOC**
- `KLOCLanguageProductivity` - Productividad por lenguaje
- `KLOCAdjustmentFactors` - Factores de ajuste

#### 7️⃣ **Proyectos y Estimaciones**
- `Projects` - Proyectos de software
- `Estimations` - Estimaciones con control de versiones
- `EstimationKLOC` - Datos específicos de estimación KLOC
- `EstimationFunctionPoints` - Datos específicos de Puntos de Función
- `EstimationFPTechnicalFactors` - Factores técnicos aplicados
- `EstimationUseCasePoints` - Datos específicos de UCP
- `EstimationUCPTechnicalFactors` - Factores técnicos UCP aplicados
- `EstimationUCPEnvironmentalFactors` - Factores ambientales UCP aplicados
- `EstimationCOCOMO` - Datos específicos de COCOMO II
- `EstimationCOCOMOScaleFactors` - Factores de escala aplicados
- `EstimationCOCOMOCostDrivers` - Cost drivers aplicados

#### 8️⃣ **Auditoría**
- `EstimationHistory` - Historial de cambios en estimaciones
- `AuditLog` - Log general del sistema

---

## 🔗 Relaciones Principales

### Flujo de Datos Principal

```
Companies ──┬──> ParameterConfigurations ──┬──> COCOMO Parameters
            │                               ├──> Function Point Parameters
            │                               ├──> Use Case Point Parameters
            │                               └──> KLOC Parameters
            │
            └──> Projects ──> Estimations ──┬──> EstimationKLOC
                                            ├──> EstimationFunctionPoints
                                            ├──> EstimationUseCasePoints
                                            └──> EstimationCOCOMO
```

### Relaciones Clave

1. **ParameterConfigurations** es la tabla central para configuración
   - Puede ser NULL (configuración default) o asociada a una empresa
   - Cada tabla de parámetros referencia a `ParameterConfigId`

2. **Estimations** controla versiones
   - Cada proyecto puede tener múltiples estimaciones
   - Cada método puede tener múltiples versiones
   - Constraint único: `(ProjectId, EstimationMethod, VersionNumber)`

3. **Tablas de Estimación Específicas** (polimorfismo)
   - Cada estimación apunta a UNA tabla específica según el método
   - Permite almacenar datos detallados de cada método

---

## 📊 Descripción Detallada de Tablas

### `Companies`
Almacena las empresas/clientes que utilizan el sistema.

| Campo | Tipo | Descripción |
|-------|------|-------------|
| CompanyId | INT | PK, Identificador único |
| Name | NVARCHAR(200) | Nombre de la empresa |
| Description | NVARCHAR(500) | Descripción opcional |
| IsActive | BIT | Estado activo/inactivo |
| CreatedAt | DATETIME2 | Fecha de creación |
| UpdatedAt | DATETIME2 | Última actualización |

### `ParameterConfigurations`
Configuraciones de parámetros (default o personalizadas por empresa).

| Campo | Tipo | Descripción |
|-------|------|-------------|
| ParameterConfigId | INT | PK, Identificador único |
| CompanyId | INT | FK a Companies (NULL = default) |
| ConfigurationName | NVARCHAR(200) | Nombre descriptivo |
| IsDefault | BIT | Indica si es configuración default |
| IsActive | BIT | Estado activo/inactivo |

**Uso**: Permite tener parámetros estándar (CompanyId = NULL) y personalizados por empresa.

### `Estimations`
Tabla principal de estimaciones con control de versiones.

| Campo | Tipo | Descripción |
|-------|------|-------------|
| EstimationId | INT | PK, Identificador único |
| ProjectId | INT | FK a Projects |
| EstimationMethod | NVARCHAR(50) | KLOC, FunctionPoints, UseCasePoints, COCOMO_Early, COCOMO_Post |
| VersionNumber | INT | Número de versión |
| ParameterConfigId | INT | FK a configuración usada |
| EstimatedEffort | DECIMAL(18,2) | Esfuerzo en persona-mes |
| EstimatedTime | DECIMAL(18,2) | Tiempo en meses |
| EstimatedCost | DECIMAL(18,2) | Costo total |
| CostPerPersonMonth | DECIMAL(18,2) | Costo por PM |

**Constraint**: Único por `(ProjectId, EstimationMethod, VersionNumber)`

### Tablas COCOMO II

#### `CocomoCostDrivers`
Define los 17 cost drivers de COCOMO II.

| Categoría | Cost Drivers |
|-----------|-------------|
| **Product** | RELY, DATA, CPLX, RUSE, DOCU |
| **Platform** | TIME, STOR, PVOL |
| **Personnel** | ACAP, PCAP, PCON, APEX, PLEX, LTEX |
| **Project** | TOOL, SITE, SCED |

#### `CocomoScaleFactors`
Define los 5 factores de escala.

| Código | Nombre |
|--------|--------|
| PREC | Precedentedness |
| FLEX | Development Flexibility |
| RESL | Risk Resolution |
| TEAM | Team Cohesion |
| PMAT | Process Maturity |

### Tablas Puntos de Función

Las tablas de complejidad (ILF, EIF, EI, EO, EQ) almacenan las matrices de pesos según:
- **Elementos de datos** (Data Elements/File Types)
- **Tipos de registro** (Record Types)
- **Complejidad resultante** (Low, Average, High)
- **Peso asignado**

Ejemplo para ILF:
```
Data Elements: 1-19, 20-50, 51+
Record Types: 1, 2-5, 6+
Complexity: Low (7), Average (10), High (15)
```

### Tablas Puntos de Casos de Uso

#### Actores
- **Simple** (1 punto): API
- **Average** (2 puntos): Protocolo/CLI
- **Complex** (3 puntos): GUI

#### Casos de Uso
- **Simple** (5 puntos): 1-3 transacciones
- **Average** (10 puntos): 4-7 transacciones
- **Complex** (15 puntos): >7 transacciones

---

## 💡 Casos de Uso

### 1. Crear una Estimación Nueva

```sql
-- 1. Crear proyecto
INSERT INTO Projects (CompanyId, UserId, ProjectName, ProjectType)
VALUES (1, 1, 'Sistema de Ventas', 'Web');

-- 2. Crear estimación usando configuración default
INSERT INTO Estimations (
    ProjectId, EstimationMethod, VersionNumber, 
    ParameterConfigId, CreatedBy
)
VALUES (1, 'COCOMO_Post', 1, 1, 1);

-- 3. Agregar datos específicos COCOMO
INSERT INTO EstimationCOCOMO (
    EstimationId, Stage, SizeInKLOC, CalibrationId,
    ScaleFactorSum, ExponentB, EffortMultiplier,
    NominalEffort, AdjustedEffort
)
VALUES (1, 'PostArchitecture', 50, 1, 15.5, 1.1, 1.2, 450, 540);
```

### 2. Consultar Todas las Versiones de una Estimación

```sql
SELECT 
    e.EstimationId,
    e.VersionNumber,
    e.EstimationMethod,
    e.EstimatedEffort,
    e.EstimatedTime,
    e.EstimatedCost,
    e.CreatedAt,
    u.FullName as CreatedBy
FROM Estimations e
JOIN Users u ON e.CreatedBy = u.UserId
WHERE e.ProjectId = 1
ORDER BY e.VersionNumber DESC;
```

### 3. Obtener Configuración de Parámetros

```sql
-- Obtener configuración para una empresa (o default si no tiene)
SELECT 
    pc.ParameterConfigId,
    pc.ConfigurationName,
    pc.IsDefault
FROM ParameterConfigurations pc
WHERE (pc.CompanyId = 1 OR pc.CompanyId IS NULL)
  AND pc.IsActive = 1
ORDER BY pc.CompanyId DESC, pc.IsDefault DESC
LIMIT 1;
```

### 4. Comparar Métodos de Estimación

```sql
SELECT 
    e.EstimationMethod,
    e.VersionNumber,
    e.EstimatedEffort as 'Esfuerzo (PM)',
    e.EstimatedTime as 'Tiempo (Meses)',
    e.EstimatedCost as 'Costo',
    e.CreatedAt
FROM Estimations e
WHERE e.ProjectId = 1
  AND e.VersionNumber = (
      SELECT MAX(VersionNumber) 
      FROM Estimations 
      WHERE ProjectId = e.ProjectId 
        AND EstimationMethod = e.EstimationMethod
  )
ORDER BY e.EstimatedEffort;
```

---

## 🚀 Instrucciones de Instalación

### Opción 1: SQL Server Management Studio (SSMS)

1. Abrir SSMS y conectarse al servidor
2. Crear base de datos:
   ```sql
   CREATE DATABASE EstimacionProyectos;
   GO
   USE EstimacionProyectos;
   GO
   ```
3. Ejecutar `DatabaseSchema.sql`
4. Ejecutar `SeedData.sql`

### Opción 2: Línea de comandos

```powershell
# Conectarse a SQL Server
sqlcmd -S localhost -U sa -P YourPassword

# Crear base de datos
CREATE DATABASE EstimacionProyectos;
GO

# Ejecutar scripts
sqlcmd -S localhost -U sa -P YourPassword -d EstimacionProyectos -i DatabaseSchema.sql
sqlcmd -S localhost -U sa -P YourPassword -d EstimacionProyectos -i SeedData.sql
```

### Opción 3: Entity Framework Core (Recomendado para .NET)

En tu proyecto Backend, instalar paquetes:

```powershell
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
```

---

## 📈 Optimizaciones Implementadas

### Índices Creados

- **Búsqueda por empresa**: `IX_Projects_CompanyId`, `IX_ParameterConfigurations_CompanyId`
- **Búsqueda por usuario**: `IX_Projects_UserId`, `IX_AuditLog_UserId`
- **Filtrado por estado**: `IX_Projects_Status`
- **Filtrado por método**: `IX_Estimations_Method`
- **Búsqueda temporal**: `IX_Estimations_CreatedAt`, `IX_AuditLog_Timestamp`
- **Historial**: `IX_EstimationHistory_EstimationId`

### Constraints

- **Unicidad**: Versiones únicas por proyecto y método
- **Integridad referencial**: Todas las FK configuradas
- **Validaciones**: CHECK constraints en valores de factores (0-5)

---

## 🔐 Seguridad y Auditoría

### Auditoría Automática

1. **EstimationHistory**: Registra cambios en estimaciones
   - Almacena valores antiguos y nuevos en JSON
   - Registra quién y cuándo hizo el cambio

2. **AuditLog**: Log general del sistema
   - Acciones de usuarios
   - IP de origen
   - Timestamp de cada acción

### Triggers Sugeridos (Implementación futura)

```sql
-- Trigger para actualizar UpdatedAt automáticamente
CREATE TRIGGER trg_UpdateTimestamp
ON Companies
AFTER UPDATE
AS
BEGIN
    UPDATE Companies
    SET UpdatedAt = GETDATE()
    WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM Inserted);
END;
```

---

## 📚 Referencias Bibliográficas

- **COCOMO II**: "Software Cost Estimation with COCOMO II" - Barry Boehm
- **Function Points**: IFPUG Counting Practices Manual
- **Use Case Points**: "Estimating with Use Case Points" - Gustav Karner
- **KLOC**: "Software Engineering Economics" - Barry Boehm

---

## 🤝 Próximos Pasos

1. **Crear DbContext en Entity Framework Core**
2. **Implementar repositorios para cada método**
3. **Crear servicios de cálculo de estimaciones**
4. **Desarrollar APIs REST para consumir desde Frontend**
5. **Implementar validaciones de negocio**
6. **Crear reportes y comparaciones**

---

## 📞 Soporte

Para preguntas sobre el diseño o implementación, contactar al equipo de desarrollo.

**Versión**: 1.0
**Fecha**: Octubre 2025
**Metodología**: OpenUP + TSP
