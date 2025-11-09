-- ================================================================================
-- COCOMO 2 STAGE 1 (COMPOSITION MODEL) - DATABASE SCHEMA
-- ================================================================================
-- Este modelo es utilizado para estimaciones en fases iniciales cuando el
-- software es principalmente composición de componentes existentes.
--
-- Características principales:
-- - Utiliza Function Points (FP) como métrica de entrada
-- - Utiliza Application Composition Effort Multipliers (ACEMs)
-- - Sin factores de escala complejos como en Post-Architecture
-- - Orientado a prototipos rápidos y RAD (Rapid Application Development)
-- ================================================================================

-- Tabla: Language (Conversión FP -> SLOC)
-- Reutilizable desde COCOMO III (ya existe en el sistema)
-- CREATE TABLE IF NOT EXISTS Language (
--     language_id INT PRIMARY KEY AUTO_INCREMENT,
--     name VARCHAR(100) NOT NULL UNIQUE,
--     sloc_per_ufp DECIMAL(10, 2) NOT NULL,
--     created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
-- );

-- Tabla: ParameterSet (Sets de parámetros para COCOMO 2 Stage 1)
-- Almacena los multiplicadores de esfuerzo aplicables a este modelo
CREATE TABLE IF NOT EXISTS ParameterSetCocomo2Stage1 (
    param_set_id INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT,
    set_name VARCHAR(255) NOT NULL,
    is_default BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    -- Constantes del modelo COCOMO 2 Composition
    const_A DECIMAL(10, 4) NOT NULL DEFAULT 2.45,  -- Application Composition base
    const_B DECIMAL(10, 4) NOT NULL DEFAULT 1.10,  -- Exponent (para ACEMs)

    -- Multiplicadores de Esfuerzo para Application Composition (ACEMs)
    -- Los valores se pueden escalar de 0.7 a 1.5 según experiencia y herramientas
    
    -- AEXP: Application Experience
    -- Rango: 1.59 (muy bajo) a 0.58 (muy alto)
    aexp_very_low DECIMAL(10, 4) NULL,      -- <2 años
    aexp_low DECIMAL(10, 4) NULL,           -- 2-5 años
    aexp_nominal DECIMAL(10, 4) NULL,       -- 5-10 años
    aexp_high DECIMAL(10, 4) NULL,          -- 10+ años

    -- PEXP: Platform Experience
    -- Rango: 1.41 (muy bajo) a 0.67 (muy alto)
    pexp_very_low DECIMAL(10, 4) NULL,      -- <2 años
    pexp_low DECIMAL(10, 4) NULL,           -- 2-5 años
    pexp_nominal DECIMAL(10, 4) NULL,       -- 5-10 años
    pexp_high DECIMAL(10, 4) NULL,          -- 10+ años

    -- PREC: Precedentedness (Experiencia similar anterior)
    -- Rango: 1.50 (muy bajo) a 0.70 (muy alto)
    prec_very_low DECIMAL(10, 4) NULL,      -- Poco similar
    prec_low DECIMAL(10, 4) NULL,           -- Algo similar
    prec_nominal DECIMAL(10, 4) NULL,       -- Moderadamente similar
    prec_high DECIMAL(10, 4) NULL,          -- Muy similar

    -- RELY: Required Reliability
    -- Rango: 0.82 (bajo) a 1.60 (alto)
    rely_low DECIMAL(10, 4) NULL,           -- Baja fiabilidad
    rely_nominal DECIMAL(10, 4) NULL,       -- Fiabilidad nominal
    rely_high DECIMAL(10, 4) NULL,          -- Alta fiabilidad

    -- TMSP: Time to Market Pressure
    -- Rango: 1.00 (sin presión) a 1.43 (presión extrema)
    tmsp_low DECIMAL(10, 4) NULL,           -- Sin presión
    tmsp_nominal DECIMAL(10, 4) NULL,       -- Presión normal
    tmsp_high DECIMAL(10, 4) NULL,          -- Presión extrema

    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE SET NULL
);

-- Tabla: Project (Proyecto para COCOMO 2 Stage 1)
CREATE TABLE IF NOT EXISTS ProjectCocomo2Stage1 (
    project_id INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT NOT NULL,
    project_name VARCHAR(255) NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Tabla: Estimation (Estimaciones para COCOMO 2 Stage 1)
-- Almacena una "snapshot" de un cálculo específico
CREATE TABLE IF NOT EXISTS EstimationCocomo2Stage1 (
    estimation_id INT PRIMARY KEY AUTO_INCREMENT,
    project_id INT NOT NULL,
    param_set_id INT NOT NULL,
    language_id INT NOT NULL,
    estimation_name VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    -- --- SELECCIÓN DE RATINGS (ACEMs) ---
    -- El usuario selecciona el nivel para cada factor
    selected_aexp VARCHAR(50) NOT NULL DEFAULT 'nominal',
    selected_pexp VARCHAR(50) NOT NULL DEFAULT 'nominal',
    selected_prec VARCHAR(50) NOT NULL DEFAULT 'nominal',
    selected_rely VARCHAR(50) NOT NULL DEFAULT 'nominal',
    selected_tmsp VARCHAR(50) NOT NULL DEFAULT 'nominal',

    -- --- RESULTADOS CALCULADOS ---
    -- Función Points totales
    total_fp DECIMAL(10, 2) NULL,

    -- Conversión a SLOC
    sloc DECIMAL(10, 2) NULL,
    ksloc DECIMAL(10, 4) NULL,

    -- Multiplicadores de esfuerzo
    aexp_multiplier DECIMAL(10, 4) NULL,
    pexp_multiplier DECIMAL(10, 4) NULL,
    prec_multiplier DECIMAL(10, 4) NULL,
    rely_multiplier DECIMAL(10, 4) NULL,
    tmsp_multiplier DECIMAL(10, 4) NULL,

    -- EAF: Effort Adjustment Factor (Producto de multiplicadores)
    eaf DECIMAL(10, 4) NULL,

    -- Esfuerzo estimado
    effort_pm DECIMAL(10, 2) NULL,      -- Person-Months
    effort_hours DECIMAL(10, 2) NULL,   -- Horas (PM * 152)

    -- --- RESULTADOS REALES (para comparación) ---
    actual_effort_pm DECIMAL(10, 2) NULL,
    actual_effort_hours DECIMAL(10, 2) NULL,
    actual_sloc DECIMAL(10, 2) NULL,

    FOREIGN KEY (project_id) REFERENCES ProjectCocomo2Stage1(project_id) ON DELETE CASCADE,
    FOREIGN KEY (param_set_id) REFERENCES ParameterSetCocomo2Stage1(param_set_id),
    FOREIGN KEY (language_id) REFERENCES Language(language_id)
);

-- Tabla: EstimationComponent (Componentes de la estimación)
-- Un componente es un elemento reutilizable (e.g., módulo, API, librería)
CREATE TABLE IF NOT EXISTS EstimationComponentCocomo2Stage1 (
    component_id INT PRIMARY KEY AUTO_INCREMENT,
    estimation_id INT NOT NULL,
    component_name VARCHAR(255) NOT NULL,
    description TEXT,

    -- Tipo de componente
    -- 'new' = Desarrollo nuevo
    -- 'adapted' = Adaptado de código existente
    -- 'cots' = COTS (Commercial Off-The-Shelf)
    component_type VARCHAR(50) NOT NULL DEFAULT 'new',

    -- Tamaño en Function Points
    size_fp DECIMAL(10, 2) NOT NULL,

    -- Si es adaptado o COTS, porcentaje de reutilización
    reuse_percent INT NULL,  -- 0-100

    -- Cantidad de cambios esperados (para componentes adaptados)
    change_percent INT NULL, -- 0-100

    -- Notas
    notes TEXT,

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    FOREIGN KEY (estimation_id) REFERENCES EstimationCocomo2Stage1(estimation_id) ON DELETE CASCADE
);

-- ================================================================================
-- DATOS INICIALES (DEFAULT VALUES)
-- ================================================================================

-- 1. Parámetros por defecto para COCOMO 2 Stage 1
INSERT INTO ParameterSetCocomo2Stage1 (
    UserId, set_name, is_default,
    const_A, const_B,
    aexp_very_low, aexp_low, aexp_nominal, aexp_high,
    pexp_very_low, pexp_low, pexp_nominal, pexp_high,
    prec_very_low, prec_low, prec_nominal, prec_high,
    rely_low, rely_nominal, rely_high,
    tmsp_low, tmsp_nominal, tmsp_high
) VALUES (
    NULL, 'Default COCOMO 2 Stage 1 (Composition)', TRUE,
    2.45, 1.10,
    -- AEXP (Application Experience): Basado en años de experiencia similar
    1.59, 1.33, 1.00, 0.58,
    -- PEXP (Platform Experience): Experiencia con la plataforma/tecnología
    1.41, 1.14, 1.00, 0.67,
    -- PREC (Precedentedness): Experiencia similar previa
    1.50, 1.24, 1.00, 0.70,
    -- RELY (Required Reliability): Necesidad de fiabilidad
    0.82, 1.00, 1.60,
    -- TMSP (Time to Market Pressure): Presión de tiempo
    1.00, 1.00, 1.43
);

-- 2. Lenguajes (reutilizados de COCOMO III - ya existen en el sistema)
-- INSERT INTO Language (name, sloc_per_ufp) VALUES
-- ('.NET/C#', 57),
-- ('Java', 53),
-- ('C++', 50),
-- ('JavaScript/TypeScript', 47),
-- ('Python', 38),
-- ('SQL', 13)
-- ON DUPLICATE KEY UPDATE sloc_per_ufp = VALUES(sloc_per_ufp);

-- ================================================================================
-- ÍNDICES PARA MEJORAR PERFORMANCE
-- ================================================================================

CREATE INDEX idx_project_cocomo2_userId ON ProjectCocomo2Stage1(UserId);
CREATE INDEX idx_estimation_cocomo2_projectId ON EstimationCocomo2Stage1(project_id);
CREATE INDEX idx_estimation_cocomo2_paramSetId ON EstimationCocomo2Stage1(param_set_id);
CREATE INDEX idx_component_cocomo2_estimationId ON EstimationComponentCocomo2Stage1(estimation_id);
CREATE INDEX idx_parameterset_cocomo2_userId ON ParameterSetCocomo2Stage1(UserId);
CREATE INDEX idx_parameterset_cocomo2_isDefault ON ParameterSetCocomo2Stage1(is_default);

-- ================================================================================
-- NOTAS DEL DISEÑO
-- ================================================================================
/*
COCOMO 2 Stage 1 (Composition Model):
- Se utiliza para estimaciones tempranas cuando el software es principalmente
  composición de componentes existentes.
  
- A diferencia del modelo Post-Architecture (COCOMO III en este proyecto),
  este modelo tiene:
  - 5 Effort Multipliers (AEXP, PEXP, PREC, RELY, TMSP) en lugar de 7
  - Factores de escala simplificados
  - Mejor aplicable a RAD y prototipos rápidos

- Fórmula:
  Effort (PM) = A × (KSLOC)^B × EAF
  
  Donde:
  - A = 2.45 (constante del modelo Composition)
  - B = 1.10 (exponent)
  - KSLOC = Lineas de código en miles (derivado de FP)
  - EAF = AEXP × PEXP × PREC × RELY × TMSP (producto de multiplicadores)

- Estructura similar a COCOMO III pero con tablas prefijadas "Cocomo2Stage1"
  para evitar conflictos y permitir múltiples modelos en la misma BD.

- Los componentes pueden ser:
  1. New (Nuevo desarrollo)
  2. Adapted (Adaptado de código existente)
  3. COTS (Componentes comerciales)

- El cálculo de esfuerzo total suma los FP de todos los componentes.
*/
