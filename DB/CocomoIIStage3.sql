/*
--------------------------------------------------------------------------------
-- ESQUEMA DE BASE DE DATOS PARA ESTIMACIÓN COCOMO II (Estadio III)
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
CREATE TABLE LanguageCocomoIIStage3 (
    language_id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(100) NOT NULL,        -- Ej: ".NET", "Java", "C++"
    sloc_per_ufp DECIMAL(10, 2) NOT NULL -- Ej: 57
);

-- Tabla de Sets de Parámetros: El núcleo de tu requisito.
-- ACTUARÁ COMO UNA "TABLA DE CONSULTA"
-- El usuario define el valor numérico para cada rating (VLO, LO, NOM, HI, VHI, XHI)
CREATE TABLE ParameterSetCocomoIIStage3 (
    param_set_id INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT, -- NULL para sets default del sistema, modificado para referenciar Users(Id)
    set_name VARCHAR(255) NOT NULL, -- Ej: "Default COCOMO II Stage 3", "Mi Set Web"
    is_default BOOLEAN DEFAULT FALSE,

    -- Constantes del modelo
    const_A DECIMAL(10, 4) NOT NULL DEFAULT 2.94,
    const_B DECIMAL(10, 4) NOT NULL DEFAULT 0.91,
    const_C DECIMAL(10, 4) NOT NULL DEFAULT 3.67,
    const_D DECIMAL(10, 4) NOT NULL DEFAULT 0.28,

    /* -- Factores de Escala (SFs) - 5 factores con 7 ratings cada uno
     -- Basado en el modelo COCOMO (XLO, VLO, LO, NOM, HI, VHI, XHI)
    */
    sf_prec_xlo DECIMAL(10, 4) NULL, sf_prec_vlo DECIMAL(10, 4) NULL, sf_prec_lo DECIMAL(10, 4) NULL, sf_prec_nom DECIMAL(10, 4) NULL, sf_prec_hi DECIMAL(10, 4) NULL, sf_prec_vhi DECIMAL(10, 4) NULL, sf_prec_xhi DECIMAL(10, 4) NULL,
    sf_flex_xlo DECIMAL(10, 4) NULL, sf_flex_vlo DECIMAL(10, 4) NULL, sf_flex_lo DECIMAL(10, 4) NULL, sf_flex_nom DECIMAL(10, 4) NULL, sf_flex_hi DECIMAL(10, 4) NULL, sf_flex_vhi DECIMAL(10, 4) NULL, sf_flex_xhi DECIMAL(10, 4) NULL,
    sf_resl_xlo DECIMAL(10, 4) NULL, sf_resl_vlo DECIMAL(10, 4) NULL, sf_resl_lo DECIMAL(10, 4) NULL, sf_resl_nom DECIMAL(10, 4) NULL, sf_resl_hi DECIMAL(10, 4) NULL, sf_resl_vhi DECIMAL(10, 4) NULL, sf_resl_xhi DECIMAL(10, 4) NULL,
    sf_team_xlo DECIMAL(10, 4) NULL, sf_team_vlo DECIMAL(10, 4) NULL, sf_team_lo DECIMAL(10, 4) NULL, sf_team_nom DECIMAL(10, 4) NULL, sf_team_hi DECIMAL(10, 4) NULL, sf_team_vhi DECIMAL(10, 4) NULL, sf_team_xhi DECIMAL(10, 4) NULL,
    sf_pmat_xlo DECIMAL(10, 4) NULL, sf_pmat_vlo DECIMAL(10, 4) NULL, sf_pmat_lo DECIMAL(10, 4) NULL, sf_pmat_nom DECIMAL(10, 4) NULL, sf_pmat_hi DECIMAL(10, 4) NULL, sf_pmat_vhi DECIMAL(10, 4) NULL, sf_pmat_xhi DECIMAL(10, 4) NULL,

    /* -- Multiplicadores de Esfuerzo (EMs) - 17 factores con 7 ratings cada uno
     -- (XLO, VLO, LO, NOM, HI, VHI, XHI)
     -- Los campos son NULLables porque algunos ratings no aplican
    */
    -- RELY (Required Reliability)
    em_rely_xlo DECIMAL(10, 4) NULL, em_rely_vlo DECIMAL(10, 4) NULL, em_rely_lo DECIMAL(10, 4) NULL, em_rely_nom DECIMAL(10, 4) NULL, em_rely_hi DECIMAL(10, 4) NULL, em_rely_vhi DECIMAL(10, 4) NULL, em_rely_xhi DECIMAL(10, 4) NULL,
    -- DATA (Database Size)
    em_data_xlo DECIMAL(10, 4) NULL, em_data_vlo DECIMAL(10, 4) NULL, em_data_lo DECIMAL(10, 4) NULL, em_data_nom DECIMAL(10, 4) NULL, em_data_hi DECIMAL(10, 4) NULL, em_data_vhi DECIMAL(10, 4) NULL, em_data_xhi DECIMAL(10, 4) NULL,
    -- CPLX (Product Complexity)
    em_cplx_xlo DECIMAL(10, 4) NULL, em_cplx_vlo DECIMAL(10, 4) NULL, em_cplx_lo DECIMAL(10, 4) NULL, em_cplx_nom DECIMAL(10, 4) NULL, em_cplx_hi DECIMAL(10, 4) NULL, em_cplx_vhi DECIMAL(10, 4) NULL, em_cplx_xhi DECIMAL(10, 4) NULL,
    -- RUSE (Required Reusability)
    em_ruse_xlo DECIMAL(10, 4) NULL, em_ruse_vlo DECIMAL(10, 4) NULL, em_ruse_lo DECIMAL(10, 4) NULL, em_ruse_nom DECIMAL(10, 4) NULL, em_ruse_hi DECIMAL(10, 4) NULL, em_ruse_vhi DECIMAL(10, 4) NULL, em_ruse_xhi DECIMAL(10, 4) NULL,
    -- DOCU (Documentation Match to Life-Cycle Needs)
    em_docu_xlo DECIMAL(10, 4) NULL, em_docu_vlo DECIMAL(10, 4) NULL, em_docu_lo DECIMAL(10, 4) NULL, em_docu_nom DECIMAL(10, 4) NULL, em_docu_hi DECIMAL(10, 4) NULL, em_docu_vhi DECIMAL(10, 4) NULL, em_docu_xhi DECIMAL(10, 4) NULL,
    -- TIME (Execution Time Constraint)
    em_time_xlo DECIMAL(10, 4) NULL, em_time_vlo DECIMAL(10, 4) NULL, em_time_lo DECIMAL(10, 4) NULL, em_time_nom DECIMAL(10, 4) NULL, em_time_hi DECIMAL(10, 4) NULL, em_time_vhi DECIMAL(10, 4) NULL, em_time_xhi DECIMAL(10, 4) NULL,
    -- STOR (Main Memory Constraint)
    em_stor_xlo DECIMAL(10, 4) NULL, em_stor_vlo DECIMAL(10, 4) NULL, em_stor_lo DECIMAL(10, 4) NULL, em_stor_nom DECIMAL(10, 4) NULL, em_stor_hi DECIMAL(10, 4) NULL, em_stor_vhi DECIMAL(10, 4) NULL, em_stor_xhi DECIMAL(10, 4) NULL,
    -- PVOL (Platform Volatility)
    em_pvol_xlo DECIMAL(10, 4) NULL, em_pvol_vlo DECIMAL(10, 4) NULL, em_pvol_lo DECIMAL(10, 4) NULL, em_pvol_nom DECIMAL(10, 4) NULL, em_pvol_hi DECIMAL(10, 4) NULL, em_pvol_vhi DECIMAL(10, 4) NULL, em_pvol_xhi DECIMAL(10, 4) NULL,
    -- ACAP (Analyst Capability)
    em_acap_xlo DECIMAL(10, 4) NULL, em_acap_vlo DECIMAL(10, 4) NULL, em_acap_lo DECIMAL(10, 4) NULL, em_acap_nom DECIMAL(10, 4) NULL, em_acap_hi DECIMAL(10, 4) NULL, em_acap_vhi DECIMAL(10, 4) NULL, em_acap_xhi DECIMAL(10, 4) NULL,
    -- PCAP (Programmer Capability)
    em_pcap_xlo DECIMAL(10, 4) NULL, em_pcap_vlo DECIMAL(10, 4) NULL, em_pcap_lo DECIMAL(10, 4) NULL, em_pcap_nom DECIMAL(10, 4) NULL, em_pcap_hi DECIMAL(10, 4) NULL, em_pcap_vhi DECIMAL(10, 4) NULL, em_pcap_xhi DECIMAL(10, 4) NULL,
    -- PCON (Personnel Continuity)
    em_pcon_xlo DECIMAL(10, 4) NULL, em_pcon_vlo DECIMAL(10, 4) NULL, em_pcon_lo DECIMAL(10, 4) NULL, em_pcon_nom DECIMAL(10, 4) NULL, em_pcon_hi DECIMAL(10, 4) NULL, em_pcon_vhi DECIMAL(10, 4) NULL, em_pcon_xhi DECIMAL(10, 4) NULL,
    -- APEX (Applications Experience)
    em_apex_xlo DECIMAL(10, 4) NULL, em_apex_vlo DECIMAL(10, 4) NULL, em_apex_lo DECIMAL(10, 4) NULL, em_apex_nom DECIMAL(10, 4) NULL, em_apex_hi DECIMAL(10, 4) NULL, em_apex_vhi DECIMAL(10, 4) NULL, em_apex_xhi DECIMAL(10, 4) NULL,
    -- PLEX (Platform Experience)
    em_plex_xlo DECIMAL(10, 4) NULL, em_plex_vlo DECIMAL(10, 4) NULL, em_plex_lo DECIMAL(10, 4) NULL, em_plex_nom DECIMAL(10, 4) NULL, em_plex_hi DECIMAL(10, 4) NULL, em_plex_vhi DECIMAL(10, 4) NULL, em_plex_xhi DECIMAL(10, 4) NULL,
    -- LTEX (Language and Tool Experience)
    em_ltex_xlo DECIMAL(10, 4) NULL, em_ltex_vlo DECIMAL(10, 4) NULL, em_ltex_lo DECIMAL(10, 4) NULL, em_ltex_nom DECIMAL(10, 4) NULL, em_ltex_hi DECIMAL(10, 4) NULL, em_ltex_vhi DECIMAL(10, 4) NULL, em_ltex_xhi DECIMAL(10, 4) NULL,
    -- TOOL (Use of Software Tools)
    em_tool_xlo DECIMAL(10, 4) NULL, em_tool_vlo DECIMAL(10, 4) NULL, em_tool_lo DECIMAL(10, 4) NULL, em_tool_nom DECIMAL(10, 4) NULL, em_tool_hi DECIMAL(10, 4) NULL, em_tool_vhi DECIMAL(10, 4) NULL, em_tool_xhi DECIMAL(10, 4) NULL,
    -- SITE (Multisite Development)
    em_site_xlo DECIMAL(10, 4) NULL, em_site_vlo DECIMAL(10, 4) NULL, em_site_lo DECIMAL(10, 4) NULL, em_site_nom DECIMAL(10, 4) NULL, em_site_hi DECIMAL(10, 4) NULL, em_site_vhi DECIMAL(10, 4) NULL, em_site_xhi DECIMAL(10, 4) NULL,
    -- SCED (Schedule Pressure)
    em_sced_xlo DECIMAL(10, 4) NULL, em_sced_vlo DECIMAL(10, 4) NULL, em_sced_lo DECIMAL(10, 4) NULL, em_sced_nom DECIMAL(10, 4) NULL, em_sced_hi DECIMAL(10, 4) NULL, em_sced_vhi DECIMAL(10, 4) NULL, em_sced_xhi DECIMAL(10, 4) NULL,

    FOREIGN KEY (UserId) REFERENCES Users(Id) -- Modificado
);


-- Tabla de Estimaciones: Es una "foto" (snapshot) de un cálculo específico.
-- Un proyecto puede tener muchas estimaciones (versiones).
CREATE TABLE EstimationCocomoIIStage3 (
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

    selected_em_rely VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_data VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_cplx VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_ruse VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_docu VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_time VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_stor VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_pvol VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_acap VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_pcap VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_pcon VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_apex VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_plex VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_ltex VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_tool VARCHAR(10) NOT NULL DEFAULT 'NOM',
    selected_em_site VARCHAR(10) NOT NULL DEFAULT 'NOM',
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
    FOREIGN KEY (param_set_id) REFERENCES ParameterSetCocomoIIStage3(param_set_id),
    FOREIGN KEY (language_id) REFERENCES LanguageCocomoIIStage3(language_id)
);


-- Tabla de Funciones: Almacena los inputs del conteo de Puntos de Función.
-- Cada fila es una de las funciones (EI, EO, EQ, ILF, EIF) que el usuario ingresó.
CREATE TABLE EstimationFunctionCocomoIIStage3 (
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

    FOREIGN KEY (estimation_id) REFERENCES EstimationCocomoIIStage3(estimation_id) ON DELETE CASCADE
);


-- -- DATOS INICIALES (VALORES DEFAULT)
-- -----------------------------------------------------------------------------

-- 1. Set de Parámetros Default (Basado en COCOMO II Stage 3)
--    UserId es NULL porque es un set del sistema.
INSERT INTO ParameterSetCocomoIIStage3 (
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

    -- Multiplicadores de Esfuerzo (EM) - Valores para COCOMO II Stage 3
    -- RELY (Required Reliability)
    em_rely_xlo, em_rely_vlo, em_rely_lo, em_rely_nom, em_rely_hi, em_rely_vhi, em_rely_xhi,
    -- DATA (Database Size)
    em_data_xlo, em_data_vlo, em_data_lo, em_data_nom, em_data_hi, em_data_vhi, em_data_xhi,
    -- CPLX (Product Complexity)
    em_cplx_xlo, em_cplx_vlo, em_cplx_lo, em_cplx_nom, em_cplx_hi, em_cplx_vhi, em_cplx_xhi,
    -- RUSE (Required Reusability)
    em_ruse_xlo, em_ruse_vlo, em_ruse_lo, em_ruse_nom, em_ruse_hi, em_ruse_vhi, em_ruse_xhi,
    -- DOCU (Documentation Match to Life-Cycle Needs)
    em_docu_xlo, em_docu_vlo, em_docu_lo, em_docu_nom, em_docu_hi, em_docu_vhi, em_docu_xhi,
    -- TIME (Execution Time Constraint)
    em_time_xlo, em_time_vlo, em_time_lo, em_time_nom, em_time_hi, em_time_vhi, em_time_xhi,
    -- STOR (Main Memory Constraint)
    em_stor_xlo, em_stor_vlo, em_stor_lo, em_stor_nom, em_stor_hi, em_stor_vhi, em_stor_xhi,
    -- PVOL (Platform Volatility)
    em_pvol_xlo, em_pvol_vlo, em_pvol_lo, em_pvol_nom, em_pvol_hi, em_pvol_vhi, em_pvol_xhi,
    -- ACAP (Analyst Capability)
    em_acap_xlo, em_acap_vlo, em_acap_lo, em_acap_nom, em_acap_hi, em_acap_vhi, em_acap_xhi,
    -- PCAP (Programmer Capability)
    em_pcap_xlo, em_pcap_vlo, em_pcap_lo, em_pcap_nom, em_pcap_hi, em_pcap_vhi, em_pcap_xhi,
    -- PCON (Personnel Continuity)
    em_pcon_xlo, em_pcon_vlo, em_pcon_lo, em_pcon_nom, em_pcon_hi, em_pcon_vhi, em_pcon_xhi,
    -- APEX (Applications Experience)
    em_apex_xlo, em_apex_vlo, em_apex_lo, em_apex_nom, em_apex_hi, em_apex_vhi, em_apex_xhi,
    -- PLEX (Platform Experience)
    em_plex_xlo, em_plex_vlo, em_plex_lo, em_plex_nom, em_plex_hi, em_plex_vhi, em_plex_xhi,
    -- LTEX (Language and Tool Experience)
    em_ltex_xlo, em_ltex_vlo, em_ltex_lo, em_ltex_nom, em_ltex_hi, em_ltex_vhi, em_ltex_xhi,
    -- TOOL (Use of Software Tools)
    em_tool_xlo, em_tool_vlo, em_tool_lo, em_tool_nom, em_tool_hi, em_tool_vhi, em_tool_xhi,
    -- SITE (Multisite Development)
    em_site_xlo, em_site_vlo, em_site_lo, em_site_nom, em_site_hi, em_site_vhi, em_site_xhi,
    -- SCED (Schedule Pressure)
    em_sced_xlo, em_sced_vlo, em_sced_lo, em_sced_nom, em_sced_hi, em_sced_vhi, em_sced_xhi
) VALUES (
    NULL, 'Default COCOMO II Stage 3', TRUE,
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

    -- EM: RELY
    NULL, 0.82, 0.92, 1.00, 1.10, 1.26, NULL,
    -- EM: DATA
    NULL, NULL, 0.90, 1.00, 1.14, 1.28, NULL,
    -- EM: CPLX
    NULL, 0.73, 0.87, 1.00, 1.17, 1.34, 1.74,
    -- EM: RUSE
    NULL, NULL, 0.95, 1.00, 1.07, 1.15, 1.24,
    -- EM: DOCU
    NULL, 0.81, 0.91, 1.00, 1.11, 1.23, NULL,
    -- EM: TIME
    NULL, NULL, NULL, 1.00, 1.11, 1.29, 1.63,
    -- EM: STOR
    NULL, NULL, NULL, 1.00, 1.05, 1.17, 1.46,
    -- EM: PVOL
    NULL, NULL, 0.87, 1.00, 1.15, 1.30, NULL,
    -- EM: ACAP
    NULL, 1.42, 1.19, 1.00, 0.85, 0.71, NULL,
    -- EM: PCAP
    NULL, 1.34, 1.15, 1.00, 0.88, 0.76, NULL,
    -- EM: PCON
    NULL, 1.29, 1.12, 1.00, 0.90, 0.81, NULL,
    -- EM: APEX
    NULL, 1.22, 1.10, 1.00, 0.88, 0.81, NULL,
    -- EM: PLEX
    NULL, 1.19, 1.09, 1.00, 0.91, 0.85, NULL,
    -- EM: LTEX
    NULL, 1.20, 1.09, 1.00, 0.91, 0.84, NULL,
    -- EM: TOOL
    NULL, 1.17, 1.09, 1.00, 0.90, 0.78, NULL,
    -- EM: SITE
    NULL, 1.22, 1.09, 1.00, 0.93, 0.86, 0.80,
    -- EM: SCED
    NULL, 1.43, 1.14, 1.00, 1.00, 1.00, NULL
);

-- 2. Datos iniciales para Lenguajes (ejemplo)
INSERT INTO LanguageCocomoIIStage3 (name, sloc_per_ufp) VALUES
('.NET', 57),
('Java', 53),
('C++', 50),
('JavaScript', 47),
('Python', 38),
('SQL', 13);

/*
-- Notas de Diseño (v3):
--
-- 1. ¿Por qué `ParameterSetCocomoIIStage3` es tan "ancha"?
--    - Para cumplir el requisito de que el usuario *defina* los valores.
--    - `ParameterSetCocomoIIStage3` ya no almacena la *selección* (ej: "NOM"), sino que
--      almacena la *tabla de valores* completa (ej: `em_rely_nom` = 1.00,
--      `em_rely_hi` = 1.10, etc.).
--    - Es una "plantilla de cálculo".
--
-- 2. ¿Dónde se guarda la selección del usuario?
--    - Se guarda en la tabla `EstimationCocomoIIStage3`.
--    - Se añadieron las columnas `selected_em_...` para los 17 EM.
--
-- 3. Flujo de Trabajo (Actualizado):
--    a. Usuario (o Admin) crea un `ParameterSetCocomoIIStage3` y rellena los campos.
--    b. Usuario crea un `Project` e inicia una `EstimationCocomoIIStage3`.
--    c. La App le pregunta:
--       - Nombre de la estimación (ej: "v1").
--       - Qué `ParameterSetCocomoIIStage3` usar (esto carga la "plantilla").
--       - Qué `LanguageCocomoIIStage3` usar.
--    d. El usuario ingresa todas las `EstimationFunctionCocomoIIStage3` (Puntos de Función).
--    e. El usuario *selecciona los ratings* para los 5 SF y 17 EM.
--    f. El backend calcula EAF multiplicando los valores de los 17 EM.
--    g. El backend calcula `E`, `PM`, `TDEV` y guarda todo en `EstimationCocomoIIStage3`.
*/