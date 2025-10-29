/*
--------------------------------------------------------------------------------
-- ESQUEMA DE BASE DE DATOS PARA ESTIMACIÓN COCOMO II (Estadio II)
--
-- Diseño optimizado para ser simple y cumplir con los requisitos:
-- 1. Múltiples proyectos por usuario.
-- 2. Múltiples estimaciones (versiones) por proyecto.
-- 3. Sets de parámetros (constantes, SF, EM) guardados y reutilizables.
-- 4. Almacenamiento de los inputs (puntos de función) de cada estimación.
-- 5. Almacenamiento de los resultados calculados.
-- 6. Almacenamiento de los resultados reales del proyecto (para comparación).
--------------------------------------------------------------------------------
*/



-- Tabla de Lenguajes: Almacena los factores de conversión UFP -> SLOC.
-- Esta tabla será de "solo lectura" para la mayoría de los usuarios,
-- y servirá para poblar un selector en la UI.
CREATE TABLE Language (
    language_id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(100) NOT NULL,        -- Ej: ".NET", "Java", "C++"
    sloc_per_ufp DECIMAL(10, 2) NOT NULL -- Ej: 57
);

-- Tabla de Sets de Parámetros: El núcleo de tu requisito.
-- ACTUARÁ COMO UNA "TABLA DE CONSULTA"
-- El usuario define el valor numérico para cada rating (XLO, VLO, LO, NOM, HI, VHI, XHI)
CREATE TABLE ParameterSet (
    param_set_id INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT, -- NULL para sets default del sistema, modificado para referenciar Users(Id)
    set_name VARCHAR(255) NOT NULL, -- Ej: "Default COCOMO II", "Mi Set Web"
    is_default BOOLEAN DEFAULT FALSE,

    -- Constantes del modelo
    const_A DECIMAL(10, 4) NOT NULL DEFAULT 2.94,
    const_B DECIMAL(10, 4) NOT NULL DEFAULT 0.91,
    const_C DECIMAL(10, 4) NOT NULL DEFAULT 3.67,
    const_D DECIMAL(10, 4) NOT NULL DEFAULT 0.28,

    /* -- Factores de Escala (SFs) - 5 factores con 6 ratings cada uno
     -- Basado en el modelo COCOMO (VLO, LO, NOM, HI, VHI, XHI)
    */
    sf_prec_vlo DECIMAL(10, 4) NULL, sf_prec_lo DECIMAL(10, 4) NULL, sf_prec_nom DECIMAL(10, 4) NULL, sf_prec_hi DECIMAL(10, 4) NULL, sf_prec_vhi DECIMAL(10, 4) NULL, sf_prec_xhi DECIMAL(10, 4) NULL,
    sf_flex_vlo DECIMAL(10, 4) NULL, sf_flex_lo DECIMAL(10, 4) NULL, sf_flex_nom DECIMAL(10, 4) NULL, sf_flex_hi DECIMAL(10, 4) NULL, sf_flex_vhi DECIMAL(10, 4) NULL, sf_flex_xhi DECIMAL(10, 4) NULL,
    sf_resl_vlo DECIMAL(10, 4) NULL, sf_resl_lo DECIMAL(10, 4) NULL, sf_resl_nom DECIMAL(10, 4) NULL, sf_resl_hi DECIMAL(10, 4) NULL, sf_resl_vhi DECIMAL(10, 4) NULL, sf_resl_xhi DECIMAL(10, 4) NULL,
    sf_team_vlo DECIMAL(10, 4) NULL, sf_team_lo DECIMAL(10, 4) NULL, sf_team_nom DECIMAL(10, 4) NULL, sf_team_hi DECIMAL(10, 4) NULL, sf_team_vhi DECIMAL(10, 4) NULL, sf_team_xhi DECIMAL(10, 4) NULL,
    sf_pmat_vlo DECIMAL(10, 4) NULL, sf_pmat_lo DECIMAL(10, 4) NULL, sf_pmat_nom DECIMAL(10, 4) NULL, sf_pmat_hi DECIMAL(10, 4) NULL, sf_pmat_vhi DECIMAL(10, 4) NULL, sf_pmat_xhi DECIMAL(10, 4) NULL,

    /* -- Multiplicadores de Esfuerzo (EMs) - 7 factores con 7 ratings cada uno
     -- (XLO, VLO, LO, NOM, HI, VHI, XHI)
     -- Los campos son NULLables porque algunos ratings no aplican (ej: PDIF)
    */
    -- PERS (Personnel Capability)
    em_pers_xlo DECIMAL(10, 4) NULL, em_pers_vlo DECIMAL(10, 4) NULL, em_pers_lo DECIMAL(10, 4) NULL, em_pers_nom DECIMAL(10, 4) NULL, em_pers_hi DECIMAL(10, 4) NULL, em_pers_vhi DECIMAL(10, 4) NULL, em_pers_xhi DECIMAL(10, 4) NULL,
    -- RCPX (Reliability and Complexity)
    em_rcpx_xlo DECIMAL(10, 4) NULL, em_rcpx_vlo DECIMAL(10, 4) NULL, em_rcpx_lo DECIMAL(10, 4) NULL, em_rcpx_nom DECIMAL(10, 4) NULL, em_rcpx_hi DECIMAL(10, 4) NULL, em_rcpx_vhi DECIMAL(10, 4) NULL, em_rcpx_xhi DECIMAL(10, 4) NULL,
    -- PDIF (Platform Difficulty)
    em_pdif_xlo DECIMAL(10, 4) NULL, em_pdif_vlo DECIMAL(10, 4) NULL, em_pdif_lo DECIMAL(10, 4) NULL, em_pdif_nom DECIMAL(10, 4) NULL, em_pdif_hi DECIMAL(10, 4) NULL, em_pdif_vhi DECIMAL(10, 4) NULL, em_pdif_xhi DECIMAL(10, 4) NULL,
    -- PREX (Personnel Experience)
    em_prex_xlo DECIMAL(10, 4) NULL, em_prex_vlo DECIMAL(10, 4) NULL, em_prex_lo DECIMAL(10, 4) NULL, em_prex_nom DECIMAL(10, 4) NULL, em_prex_hi DECIMAL(10, 4) NULL, em_prex_vhi DECIMAL(10, 4) NULL, em_prex_xhi DECIMAL(10, 4) NULL,
    -- RUSE (Required Reusability)
    em_ruse_xlo DECIMAL(10, 4) NULL, em_ruse_vlo DECIMAL(10, 4) NULL, em_ruse_lo DECIMAL(10, 4) NULL, em_ruse_nom DECIMAL(10, 4) NULL, em_ruse_hi DECIMAL(10, 4) NULL, em_ruse_vhi DECIMAL(10, 4) NULL, em_ruse_xhi DECIMAL(10, 4) NULL,
    -- FCIL (Facilities)
    em_fcil_xlo DECIMAL(10, 4) NULL, em_fcil_vlo DECIMAL(10, 4) NULL, em_fcil_lo DECIMAL(10, 4) NULL, em_fcil_nom DECIMAL(10, 4) NULL, em_fcil_hi DECIMAL(10, 4) NULL, em_fcil_vhi DECIMAL(10, 4) NULL, em_fcil_xhi DECIMAL(10, 4) NULL,
    -- SCED (Schedule)
    em_sced_xlo DECIMAL(10, 4) NULL, em_sced_vlo DECIMAL(10, 4) NULL, em_sced_lo DECIMAL(10, 4) NULL, em_sced_nom DECIMAL(10, 4) NULL, em_sced_hi DECIMAL(10, 4) NULL, em_sced_vhi DECIMAL(10, 4) NULL, em_sced_xhi DECIMAL(10, 4) NULL,

    FOREIGN KEY (UserId) REFERENCES Users(Id) -- Modificado
);


-- Tabla de Estimaciones: Es una "foto" (snapshot) de un cálculo específico.
-- Un proyecto puede tener muchas estimaciones (versiones).
CREATE TABLE Estimation (
    estimation_id INT PRIMARY KEY AUTO_INCREMENT,
    project_id INT NOT NULL,
    param_set_id INT NOT NULL, -- El set de parámetros que se USÓ para este cálculo
    language_id INT NOT NULL,  -- El lenguaje que se USÓ para este cálculo
    estimation_name VARCHAR(255) NOT NULL, -- Ej: "v1 - Estimación inicial"
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    -- --- SELECCIÓN DE RATINGS ---
    -- Aquí guardamos la *selección* del usuario (ej: 'NOM', 'HI')
    -- El backend usará esto + param_set_id para buscar el valor numérico
    selected_sf_prec VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_sf_flex VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_sf_resl VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_sf_team VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_sf_pmat VARCHAR(10) NOT NULL DEFAULT 'NOM',

    selected_em_pers VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_rcpx VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_pdif VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_prex VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_ruse VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_fcil VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_sced VARCHAR(10) NOT NULL DEFAULT 'NOM',

    -- --- RESULTADOS CALCULADOS ---
    -- Estos son los outputs del modelo
    total_ufp DECIMAL(10, 2), -- Suma de todos los puntos de EstimationFunction
    sloc DECIMAL(10, 2),      -- UFP * Factor del Lenguaje
    ksloc DECIMAL(10, 4),     -- sloc / 1000
    sum_sf DECIMAL(10, 4),    -- Suma de los pesos de los 5 SF
    exponent_E DECIMAL(10, 4),-- B + 0.01 * sum_sf
    eaf DECIMAL(10, 4),       -- Producto de los pesos de los 17 EM
    effort_pm DECIMAL(10, 2), -- Person-Months (PM)
    tdev_months DECIMAL(10, 2),-- Time to Develop (TDEV)
    avg_team_size DECIMAL(10, 2), -- PM / TDEV

    -- --- RESULTADOS REALES ---
    -- El usuario llena esto al final del proyecto (todos son NULLables)
    actual_effort_pm DECIMAL(10, 2) NULL,
    actual_tdev_months DECIMAL(10, 2) NULL,
    actual_sloc DECIMAL(10, 2) NULL,

    FOREIGN KEY (project_id) REFERENCES Project(project_id) ON DELETE CASCADE,
    FOREIGN KEY (param_set_id) REFERENCES ParameterSet(param_set_id),
    FOREIGN KEY (language_id) REFERENCES Language(language_id)
);


-- Tabla de Funciones: Almacena los inputs del conteo de Puntos de Función.
-- Cada fila es una de las funciones (EI, EO, EQ, ILF, EIF) que el usuario ingresó.
CREATE TABLE EstimationFunction (
    function_id INT PRIMARY KEY AUTO_INCREMENT,
    estimation_id INT NOT NULL,
    name VARCHAR(255) NOT NULL, -- Ej: "Crear proyecto", "Reporte Resumen"
    type VARCHAR(10) NOT NULL,  -- 'EI', 'EO', 'EQ', 'ILF', 'EIF'
    det INT,                    -- Data Element Types
    ret_ftr INT,                -- Record Element Types / File Types Referenced
    
    -- El backend calculará esto basado en DET/RET y tipo
    complexity VARCHAR(20),     -- Ej: "Baja", "Media", "Alta"
    
    -- El backend asignará esto basado en la complejidad
    calculated_points DECIMAL(10, 2), -- Los puntos de función para esta fila

    FOREIGN KEY (estimation_id) REFERENCES Estimation(estimation_id) ON DELETE CASCADE
);


-- -- DATOS INICIALES (VALORES DEFAULT)
-- -----------------------------------------------------------------------------

-- 1. Set de Parámetros Default (Basado en COCOMO II Post-Architecture y el ejemplo)
--    UserId es NULL porque es un set del sistema.
INSERT INTO ParameterSet (
    UserId, set_name, is_default,
    const_A, const_B, const_C, const_D,
    
    -- Factores de Escala (SF) - Valores estándar Post-Architecture
    -- PREC (Precedentedness)
    sf_prec_vlo, sf_prec_lo, sf_prec_nom, sf_prec_hi, sf_prec_vhi, sf_prec_xhi,
    -- FLEX (Development Flexibility)
    sf_flex_vlo, sf_flex_lo, sf_flex_nom, sf_flex_hi, sf_flex_vhi, sf_flex_xhi,
    -- RESL (Architecture/Risk Resolution)
    sf_resl_vlo, sf_resl_lo, sf_resl_nom, sf_resl_hi, sf_resl_vhi, sf_resl_xhi,
    -- TEAM (Team Cohesion)
    sf_team_vlo, sf_team_lo, sf_team_nom, sf_team_hi, sf_team_vhi, sf_team_xhi,
    -- PMAT (Process Maturity)
    sf_pmat_vlo, sf_pmat_lo, sf_pmat_nom, sf_pmat_hi, sf_pmat_vhi, sf_pmat_xhi,

    -- Multiplicadores de Esfuerzo (EM) - Valores del ejemplo
    -- PERS (Personnel Capability)
    em_pers_xlo, em_pers_vlo, em_pers_lo, em_pers_nom, em_pers_hi, em_pers_vhi, em_pers_xhi,
    -- RCPX (Reliability and Complexity)
    em_rcpx_xlo, em_rcpx_vlo, em_rcpx_lo, em_rcpx_nom, em_rcpx_hi, em_rcpx_vhi, em_rcpx_xhi,
    -- PDIF (Platform Difficulty)
    em_pdif_xlo, em_pdif_vlo, em_pdif_lo, em_pdif_nom, em_pdif_hi, em_pdif_vhi, em_pdif_xhi,
    -- PREX (Personnel Experience)
    em_prex_xlo, em_prex_vlo, em_prex_lo, em_prex_nom, em_prex_hi, em_prex_vhi, em_prex_xhi,
    -- RUSE (Required Reusability)
    em_ruse_xlo, em_ruse_vlo, em_ruse_lo, em_ruse_nom, em_ruse_hi, em_ruse_vhi, em_ruse_xhi,
    -- FCIL (Facilities)
    em_fcil_xlo, em_fcil_vlo, em_fcil_lo, em_fcil_nom, em_fcil_hi, em_fcil_vhi, em_fcil_xhi,
    -- SCED (Schedule)
    em_sced_xlo, em_sced_vlo, em_sced_lo, em_sced_nom, em_sced_hi, em_sced_vhi, em_sced_xhi
) VALUES (
    NULL, 'Default COCOMO II', TRUE,
    2.94, 0.91, 3.67, 0.28, -- Constantes A, B, C, D del ejemplo

    -- SF: PREC
    1.62, 1.26, 1.00, 0.83, 0.63, 0.50,
    -- SF: FLEX
    0.60, 0.83, 1.00, 1.33, 1.91, 2.72,
    -- SF: RESL
    0.00, 0.87, 1.00, 1.29, 1.81, 2.61,
    -- SF: TEAM
    1.33, 1.12, 1.00, 0.87, 0.74, 0.62,
    -- SF: PMAT
    1.30, 1.10, 1.00, 0.87, 0.73, 0.62,

    -- EM: PERS (del ejemplo: 2.12, 1.62, 1.26, 1, 0.83, 0.63, 0.5)
    2.12, 1.62, 1.26, 1.00, 0.83, 0.63, 0.50,
    -- EM: RCPX (del ejemplo: 0.49, 0.6, 0.83, 1, 1.33, 1.91, 2.72)
    0.49, 0.60, 0.83, 1.00, 1.33, 1.91, 2.72,
    -- EM: PDIF (del ejemplo: -, -, 0.87, 1, 1.29, 1.81, 2.61)
    NULL, NULL, 0.87, 1.00, 1.29, 1.81, 2.61,
    -- EM: PREX (del ejemplo: 1.59, 1.33, 1.12, 1, 0.87, 0.74, 0.62)
    1.59, 1.33, 1.12, 1.00, 0.87, 0.74, 0.62,
    -- EM: RUSE (del ejemplo: -, -, 0.95, 1, 1.07, 1.15, 1.24)
    NULL, NULL, 0.95, 1.00, 1.07, 1.15, 1.24,
    -- EM: FCIL (del ejemplo: 1.43, 1.3, 1.10, 1, 0.87, 0.73, 0.62)
    1.43, 1.30, 1.10, 1.00, 0.87, 0.73, 0.62,
    -- EM: SCED (del ejemplo: -, 1.43, 1.14, 1, 1, 1, -)
    NULL, 1.43, 1.14, 1.00, 1.00, 1.00, NULL
);

-- 2. Datos iniciales para Lenguajes (ejemplo)
INSERT INTO Language (name, sloc_per_ufp) VALUES
('.NET', 57),
('Java', 53),
('C++', 50),
('JavaScript', 47),
('Python', 38),
('SQL', 13);

/*
-- Notas de Diseño (v2):
--
-- 1. ¿Por qué `ParameterSet` ahora es tan "ancha"?
--    - Para cumplir el requisito de que el usuario *defina* los valores.
--    - `ParameterSet` ya no almacena la *selección* (ej: "NOM"), sino que
--      almacena la *tabla de valores* completa (ej: `em_pers_nom` = 1.00,
--      `em_pers_hi` = 0.83, etc.).
--    - Es una "plantilla de cálculo".
--
-- 2. ¿Dónde se guarda la selección del usuario?
--    - Se guarda en la tabla `Estimation`.
--    - Se añadieron las columnas `selected_sf_...` y `selected_em_...`.
--
-- 3. Flujo de Trabajo (Actualizado):
--    a. Usuario (o Admin) crea un `ParameterSet` y rellena los ~80 campos
--       (const_A, sf_prec_vlo, sf_prec_lo, ..., em_sced_xhi).
--    b. Usuario crea un `Project` e inicia una `Estimation`.
--    c. La App le pregunta:
--       - Nombre de la estimación (ej: "v1").
--       - Qué `ParameterSet` usar (esto carga la "plantilla").
--       - Qué `Language` usar.
--    d. El usuario ingresa todas las `EstimationFunction` (Puntos de Función).
--    e. El usuario *selecciona los ratings* para los 5 SF y 7 EM (ej: "PREC" = "HI", "PERS" = "LO").
--       Estos ratings ('HI', 'LO') se guardan en las columnas `selected_...` de `Estimation`.
--    f. El backend calcula:
--       - Suma los `total_ufp` de `EstimationFunction`.
--       - Calcula `ksloc` usando `Language`.
--       - **Para EAF y SUM_SF**:
--         1. Carga el `ParameterSet` (plantilla) usando `estimation.param_set_id`.
--         2. Mira la selección del usuario, ej: `estimation.selected_em_pers` ("LO").
--         3. Busca el valor correspondiente: `ParameterSet.em_pers_lo`.
--         4. Repite para todos los SF y EM.
--         5. Multiplica los valores de EM para obtener `eaf`.
--         6. Suma los valores de SF para obtener `sum_sf`.
--    g. El backend calcula `E`, `PM`, `TDEV` y guarda todo en `Estimation`.
*/