-- =============================================
-- DATOS INICIALES (SEED DATA)
-- Valores estándar para métodos de estimación
-- =============================================

USE [EstimacionProyectos]; -- Cambiar por el nombre de tu base de datos
GO

-- =============================================
-- CONFIGURACIÓN DE PARÁMETROS DEFAULT
-- =============================================

-- Configuración Default (sin empresa específica)
INSERT INTO ParameterConfigurations (CompanyId, ConfigurationName, Description, IsDefault, IsActive)
VALUES (NULL, 'Default Configuration', 'Configuración estándar con valores de la literatura', 1, 1);

DECLARE @DefaultConfigId INT = SCOPE_IDENTITY();

-- =============================================
-- COCOMO II - PARÁMETROS DE CALIBRACIÓN
-- =============================================

-- Parámetros de calibración para diferentes etapas
INSERT INTO CocomoCalibrationParameters (ParameterConfigId, Stage, ParameterA, ParameterB, ParameterC, ParameterD)
VALUES 
    (@DefaultConfigId, 'EarlyDesign', 2.94, 0.91, 3.67, 0.28),
    (@DefaultConfigId, 'PostArchitecture', 2.94, 0.91, 3.67, 0.28);

-- =============================================
-- COCOMO II - FACTORES DE ESCALA
-- =============================================

INSERT INTO CocomoScaleFactors (Code, Name, Description)
VALUES 
    ('PREC', 'Precedentedness', 'Familiaridad con proyectos similares'),
    ('FLEX', 'Development Flexibility', 'Flexibilidad en el desarrollo'),
    ('RESL', 'Risk Resolution', 'Gestión de riesgos y arquitectura'),
    ('TEAM', 'Team Cohesion', 'Cohesión del equipo'),
    ('PMAT', 'Process Maturity', 'Madurez del proceso (CMMI)');

-- Valores de Factores de Escala
DECLARE @PREC_ID INT = (SELECT ScaleFactorId FROM CocomoScaleFactors WHERE Code = 'PREC');
DECLARE @FLEX_ID INT = (SELECT ScaleFactorId FROM CocomoScaleFactors WHERE Code = 'FLEX');
DECLARE @RESL_ID INT = (SELECT ScaleFactorId FROM CocomoScaleFactors WHERE Code = 'RESL');
DECLARE @TEAM_ID INT = (SELECT ScaleFactorId FROM CocomoScaleFactors WHERE Code = 'TEAM');
DECLARE @PMAT_ID INT = (SELECT ScaleFactorId FROM CocomoScaleFactors WHERE Code = 'PMAT');

INSERT INTO CocomoScaleFactorValues (ScaleFactorId, ParameterConfigId, RatingLevel, Value)
VALUES 
    -- PREC
    (@PREC_ID, @DefaultConfigId, 'VeryLow', 6.20),
    (@PREC_ID, @DefaultConfigId, 'Low', 4.96),
    (@PREC_ID, @DefaultConfigId, 'Nominal', 3.72),
    (@PREC_ID, @DefaultConfigId, 'High', 2.48),
    (@PREC_ID, @DefaultConfigId, 'VeryHigh', 1.24),
    (@PREC_ID, @DefaultConfigId, 'ExtraHigh', 0.00),
    -- FLEX
    (@FLEX_ID, @DefaultConfigId, 'VeryLow', 5.07),
    (@FLEX_ID, @DefaultConfigId, 'Low', 4.05),
    (@FLEX_ID, @DefaultConfigId, 'Nominal', 3.04),
    (@FLEX_ID, @DefaultConfigId, 'High', 2.03),
    (@FLEX_ID, @DefaultConfigId, 'VeryHigh', 1.01),
    (@FLEX_ID, @DefaultConfigId, 'ExtraHigh', 0.00),
    -- RESL
    (@RESL_ID, @DefaultConfigId, 'VeryLow', 7.07),
    (@RESL_ID, @DefaultConfigId, 'Low', 5.65),
    (@RESL_ID, @DefaultConfigId, 'Nominal', 4.24),
    (@RESL_ID, @DefaultConfigId, 'High', 2.83),
    (@RESL_ID, @DefaultConfigId, 'VeryHigh', 1.41),
    (@RESL_ID, @DefaultConfigId, 'ExtraHigh', 0.00),
    -- TEAM
    (@TEAM_ID, @DefaultConfigId, 'VeryLow', 5.48),
    (@TEAM_ID, @DefaultConfigId, 'Low', 4.38),
    (@TEAM_ID, @DefaultConfigId, 'Nominal', 3.29),
    (@TEAM_ID, @DefaultConfigId, 'High', 2.19),
    (@TEAM_ID, @DefaultConfigId, 'VeryHigh', 1.10),
    (@TEAM_ID, @DefaultConfigId, 'ExtraHigh', 0.00),
    -- PMAT
    (@PMAT_ID, @DefaultConfigId, 'VeryLow', 7.80),
    (@PMAT_ID, @DefaultConfigId, 'Low', 6.24),
    (@PMAT_ID, @DefaultConfigId, 'Nominal', 4.68),
    (@PMAT_ID, @DefaultConfigId, 'High', 3.12),
    (@PMAT_ID, @DefaultConfigId, 'VeryHigh', 1.56),
    (@PMAT_ID, @DefaultConfigId, 'ExtraHigh', 0.00);

-- =============================================
-- COCOMO II - COST DRIVERS (POST-ARCHITECTURE)
-- =============================================

-- Product Factors
INSERT INTO CocomoCostDrivers (Code, Name, Category, Description, ApplicableStage)
VALUES 
    ('RELY', 'Required Software Reliability', 'Product', 'Confiabilidad requerida', 'Both'),
    ('DATA', 'Database Size', 'Product', 'Tamaño de la base de datos', 'Both'),
    ('CPLX', 'Product Complexity', 'Product', 'Complejidad del producto', 'Both'),
    ('RUSE', 'Required Reusability', 'Product', 'Reusabilidad requerida', 'PostArchitecture'),
    ('DOCU', 'Documentation Match to Lifecycle Needs', 'Product', 'Documentación', 'Both');

-- Platform Factors
INSERT INTO CocomoCostDrivers (Code, Name, Category, Description, ApplicableStage)
VALUES 
    ('TIME', 'Execution Time Constraint', 'Platform', 'Restricción de tiempo de ejecución', 'Both'),
    ('STOR', 'Main Storage Constraint', 'Platform', 'Restricción de memoria', 'Both'),
    ('PVOL', 'Platform Volatility', 'Platform', 'Volatilidad de la plataforma', 'Both');

-- Personnel Factors
INSERT INTO CocomoCostDrivers (Code, Name, Category, Description, ApplicableStage)
VALUES 
    ('ACAP', 'Analyst Capability', 'Personnel', 'Capacidad del analista', 'Both'),
    ('PCAP', 'Programmer Capability', 'Personnel', 'Capacidad del programador', 'PostArchitecture'),
    ('PCON', 'Personnel Continuity', 'Personnel', 'Continuidad del personal', 'Both'),
    ('APEX', 'Applications Experience', 'Personnel', 'Experiencia en aplicaciones', 'Both'),
    ('PLEX', 'Platform Experience', 'Personnel', 'Experiencia en la plataforma', 'Both'),
    ('LTEX', 'Language and Tool Experience', 'Personnel', 'Experiencia en lenguaje y herramientas', 'Both');

-- Project Factors
INSERT INTO CocomoCostDrivers (Code, Name, Category, Description, ApplicableStage)
VALUES 
    ('TOOL', 'Use of Software Tools', 'Project', 'Uso de herramientas de software', 'Both'),
    ('SITE', 'Multisite Development', 'Project', 'Desarrollo multi-sitio', 'Both'),
    ('SCED', 'Required Development Schedule', 'Project', 'Calendario de desarrollo requerido', 'Both');

-- Valores de Cost Drivers (Multiplicadores)
DECLARE @RELY_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'RELY');
DECLARE @DATA_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'DATA');
DECLARE @CPLX_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'CPLX');
DECLARE @RUSE_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'RUSE');
DECLARE @DOCU_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'DOCU');
DECLARE @TIME_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'TIME');
DECLARE @STOR_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'STOR');
DECLARE @PVOL_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'PVOL');
DECLARE @ACAP_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'ACAP');
DECLARE @PCAP_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'PCAP');
DECLARE @PCON_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'PCON');
DECLARE @APEX_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'APEX');
DECLARE @PLEX_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'PLEX');
DECLARE @LTEX_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'LTEX');
DECLARE @TOOL_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'TOOL');
DECLARE @SITE_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'SITE');
DECLARE @SCED_ID INT = (SELECT CostDriverId FROM CocomoCostDrivers WHERE Code = 'SCED');

-- RELY
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@RELY_ID, @DefaultConfigId, 'VeryLow', 0.82),
    (@RELY_ID, @DefaultConfigId, 'Low', 0.92),
    (@RELY_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@RELY_ID, @DefaultConfigId, 'High', 1.10),
    (@RELY_ID, @DefaultConfigId, 'VeryHigh', 1.26);

-- DATA
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@DATA_ID, @DefaultConfigId, 'Low', 0.90),
    (@DATA_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@DATA_ID, @DefaultConfigId, 'High', 1.14),
    (@DATA_ID, @DefaultConfigId, 'VeryHigh', 1.28);

-- CPLX
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@CPLX_ID, @DefaultConfigId, 'VeryLow', 0.73),
    (@CPLX_ID, @DefaultConfigId, 'Low', 0.87),
    (@CPLX_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@CPLX_ID, @DefaultConfigId, 'High', 1.17),
    (@CPLX_ID, @DefaultConfigId, 'VeryHigh', 1.34),
    (@CPLX_ID, @DefaultConfigId, 'ExtraHigh', 1.74);

-- RUSE
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@RUSE_ID, @DefaultConfigId, 'Low', 0.95),
    (@RUSE_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@RUSE_ID, @DefaultConfigId, 'High', 1.07),
    (@RUSE_ID, @DefaultConfigId, 'VeryHigh', 1.15),
    (@RUSE_ID, @DefaultConfigId, 'ExtraHigh', 1.24);

-- DOCU
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@DOCU_ID, @DefaultConfigId, 'VeryLow', 0.81),
    (@DOCU_ID, @DefaultConfigId, 'Low', 0.91),
    (@DOCU_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@DOCU_ID, @DefaultConfigId, 'High', 1.11),
    (@DOCU_ID, @DefaultConfigId, 'VeryHigh', 1.23);

-- TIME
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@TIME_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@TIME_ID, @DefaultConfigId, 'High', 1.11),
    (@TIME_ID, @DefaultConfigId, 'VeryHigh', 1.29),
    (@TIME_ID, @DefaultConfigId, 'ExtraHigh', 1.63);

-- STOR
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@STOR_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@STOR_ID, @DefaultConfigId, 'High', 1.05),
    (@STOR_ID, @DefaultConfigId, 'VeryHigh', 1.17),
    (@STOR_ID, @DefaultConfigId, 'ExtraHigh', 1.46);

-- PVOL
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@PVOL_ID, @DefaultConfigId, 'Low', 0.87),
    (@PVOL_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@PVOL_ID, @DefaultConfigId, 'High', 1.15),
    (@PVOL_ID, @DefaultConfigId, 'VeryHigh', 1.30);

-- ACAP
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@ACAP_ID, @DefaultConfigId, 'VeryLow', 1.42),
    (@ACAP_ID, @DefaultConfigId, 'Low', 1.19),
    (@ACAP_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@ACAP_ID, @DefaultConfigId, 'High', 0.85),
    (@ACAP_ID, @DefaultConfigId, 'VeryHigh', 0.71);

-- PCAP
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@PCAP_ID, @DefaultConfigId, 'VeryLow', 1.34),
    (@PCAP_ID, @DefaultConfigId, 'Low', 1.15),
    (@PCAP_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@PCAP_ID, @DefaultConfigId, 'High', 0.88),
    (@PCAP_ID, @DefaultConfigId, 'VeryHigh', 0.76);

-- PCON
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@PCON_ID, @DefaultConfigId, 'VeryLow', 1.29),
    (@PCON_ID, @DefaultConfigId, 'Low', 1.12),
    (@PCON_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@PCON_ID, @DefaultConfigId, 'High', 0.90),
    (@PCON_ID, @DefaultConfigId, 'VeryHigh', 0.81);

-- APEX
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@APEX_ID, @DefaultConfigId, 'VeryLow', 1.22),
    (@APEX_ID, @DefaultConfigId, 'Low', 1.10),
    (@APEX_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@APEX_ID, @DefaultConfigId, 'High', 0.88),
    (@APEX_ID, @DefaultConfigId, 'VeryHigh', 0.81);

-- PLEX
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@PLEX_ID, @DefaultConfigId, 'VeryLow', 1.19),
    (@PLEX_ID, @DefaultConfigId, 'Low', 1.09),
    (@PLEX_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@PLEX_ID, @DefaultConfigId, 'High', 0.91),
    (@PLEX_ID, @DefaultConfigId, 'VeryHigh', 0.85);

-- LTEX
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@LTEX_ID, @DefaultConfigId, 'VeryLow', 1.20),
    (@LTEX_ID, @DefaultConfigId, 'Low', 1.09),
    (@LTEX_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@LTEX_ID, @DefaultConfigId, 'High', 0.91),
    (@LTEX_ID, @DefaultConfigId, 'VeryHigh', 0.84);

-- TOOL
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@TOOL_ID, @DefaultConfigId, 'VeryLow', 1.17),
    (@TOOL_ID, @DefaultConfigId, 'Low', 1.09),
    (@TOOL_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@TOOL_ID, @DefaultConfigId, 'High', 0.90),
    (@TOOL_ID, @DefaultConfigId, 'VeryHigh', 0.78);

-- SITE
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@SITE_ID, @DefaultConfigId, 'VeryLow', 1.22),
    (@SITE_ID, @DefaultConfigId, 'Low', 1.09),
    (@SITE_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@SITE_ID, @DefaultConfigId, 'High', 0.93),
    (@SITE_ID, @DefaultConfigId, 'VeryHigh', 0.86),
    (@SITE_ID, @DefaultConfigId, 'ExtraHigh', 0.80);

-- SCED
INSERT INTO CocomoCostDriverValues (CostDriverId, ParameterConfigId, RatingLevel, Multiplier)
VALUES 
    (@SCED_ID, @DefaultConfigId, 'VeryLow', 1.43),
    (@SCED_ID, @DefaultConfigId, 'Low', 1.14),
    (@SCED_ID, @DefaultConfigId, 'Nominal', 1.00),
    (@SCED_ID, @DefaultConfigId, 'High', 1.00),
    (@SCED_ID, @DefaultConfigId, 'VeryHigh', 1.00);

-- =============================================
-- PUNTOS DE FUNCIÓN - FACTORES TÉCNICOS
-- =============================================

INSERT INTO FunctionPointTechnicalFactors (Code, Name, Description)
VALUES 
    ('F1', 'Data Communications', 'Comunicación de datos'),
    ('F2', 'Distributed Processing', 'Procesamiento distribuido'),
    ('F3', 'Performance', 'Rendimiento'),
    ('F4', 'Heavily Used Configuration', 'Configuración muy utilizada'),
    ('F5', 'Transaction Rate', 'Tasa de transacciones'),
    ('F6', 'Online Data Entry', 'Entrada de datos en línea'),
    ('F7', 'End-User Efficiency', 'Eficiencia del usuario final'),
    ('F8', 'Online Update', 'Actualización en línea'),
    ('F9', 'Complex Processing', 'Procesamiento complejo'),
    ('F10', 'Reusability', 'Reusabilidad'),
    ('F11', 'Installation Ease', 'Facilidad de instalación'),
    ('F12', 'Operational Ease', 'Facilidad operacional'),
    ('F13', 'Multiple Sites', 'Múltiples sitios'),
    ('F14', 'Facilitate Change', 'Facilitar el cambio');

-- Valores default (Nominal = 3 para todos)
DECLARE @TF_ID INT;
DECLARE TF_CURSOR CURSOR FOR 
    SELECT TechnicalFactorId FROM FunctionPointTechnicalFactors;

OPEN TF_CURSOR;
FETCH NEXT FROM TF_CURSOR INTO @TF_ID;

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO FunctionPointTechnicalFactorValues (TechnicalFactorId, ParameterConfigId, InfluenceLevel)
    VALUES (@TF_ID, @DefaultConfigId, 3);
    
    FETCH NEXT FROM TF_CURSOR INTO @TF_ID;
END

CLOSE TF_CURSOR;
DEALLOCATE TF_CURSOR;

-- =============================================
-- PUNTOS DE FUNCIÓN - TABLAS DE COMPLEJIDAD
-- =============================================

-- ILF (Internal Logical Files)
INSERT INTO FunctionPointILFComplexity (ParameterConfigId, DataElements, RecordTypes, Complexity, Weight)
VALUES 
    (@DefaultConfigId, '1-19', '1', 'Low', 7),
    (@DefaultConfigId, '1-19', '2-5', 'Low', 7),
    (@DefaultConfigId, '1-19', '6+', 'Average', 10),
    (@DefaultConfigId, '20-50', '1', 'Low', 7),
    (@DefaultConfigId, '20-50', '2-5', 'Average', 10),
    (@DefaultConfigId, '20-50', '6+', 'High', 15),
    (@DefaultConfigId, '51+', '1', 'Average', 10),
    (@DefaultConfigId, '51+', '2-5', 'High', 15),
    (@DefaultConfigId, '51+', '6+', 'High', 15);

-- EIF (External Interface Files)
INSERT INTO FunctionPointEIFComplexity (ParameterConfigId, DataElements, RecordTypes, Complexity, Weight)
VALUES 
    (@DefaultConfigId, '1-19', '1', 'Low', 5),
    (@DefaultConfigId, '1-19', '2-5', 'Low', 5),
    (@DefaultConfigId, '1-19', '6+', 'Average', 7),
    (@DefaultConfigId, '20-50', '1', 'Low', 5),
    (@DefaultConfigId, '20-50', '2-5', 'Average', 7),
    (@DefaultConfigId, '20-50', '6+', 'High', 10),
    (@DefaultConfigId, '51+', '1', 'Average', 7),
    (@DefaultConfigId, '51+', '2-5', 'High', 10),
    (@DefaultConfigId, '51+', '6+', 'High', 10);

-- EI (External Inputs)
INSERT INTO FunctionPointEIComplexity (ParameterConfigId, DataElements, FileTypes, Complexity, Weight)
VALUES 
    (@DefaultConfigId, '1-4', '0-1', 'Low', 3),
    (@DefaultConfigId, '1-4', '2', 'Low', 3),
    (@DefaultConfigId, '1-4', '3+', 'Average', 4),
    (@DefaultConfigId, '5-15', '0-1', 'Low', 3),
    (@DefaultConfigId, '5-15', '2', 'Average', 4),
    (@DefaultConfigId, '5-15', '3+', 'High', 6),
    (@DefaultConfigId, '16+', '0-1', 'Average', 4),
    (@DefaultConfigId, '16+', '2', 'High', 6),
    (@DefaultConfigId, '16+', '3+', 'High', 6);

-- EO (External Outputs)
INSERT INTO FunctionPointEOComplexity (ParameterConfigId, DataElements, FileTypes, Complexity, Weight)
VALUES 
    (@DefaultConfigId, '1-5', '0-1', 'Low', 4),
    (@DefaultConfigId, '1-5', '2-3', 'Low', 4),
    (@DefaultConfigId, '1-5', '4+', 'Average', 5),
    (@DefaultConfigId, '6-19', '0-1', 'Low', 4),
    (@DefaultConfigId, '6-19', '2-3', 'Average', 5),
    (@DefaultConfigId, '6-19', '4+', 'High', 7),
    (@DefaultConfigId, '20+', '0-1', 'Average', 5),
    (@DefaultConfigId, '20+', '2-3', 'High', 7),
    (@DefaultConfigId, '20+', '4+', 'High', 7);

-- EQ (External Queries)
INSERT INTO FunctionPointEQComplexity (ParameterConfigId, DataElements, FileTypes, Complexity, Weight)
VALUES 
    (@DefaultConfigId, '1-5', '0-1', 'Low', 3),
    (@DefaultConfigId, '1-5', '2-3', 'Low', 3),
    (@DefaultConfigId, '1-5', '4+', 'Average', 4),
    (@DefaultConfigId, '6-19', '0-1', 'Low', 3),
    (@DefaultConfigId, '6-19', '2-3', 'Average', 4),
    (@DefaultConfigId, '6-19', '4+', 'High', 6),
    (@DefaultConfigId, '20+', '0-1', 'Average', 4),
    (@DefaultConfigId, '20+', '2-3', 'High', 6),
    (@DefaultConfigId, '20+', '4+', 'High', 6);

-- =============================================
-- PUNTOS DE CASOS DE USO - PESOS
-- =============================================

-- Pesos de Actores
INSERT INTO UseCasePointActorWeights (ParameterConfigId, ComplexityType, Weight, Description)
VALUES 
    (@DefaultConfigId, 'Simple', 1, 'API de otro sistema'),
    (@DefaultConfigId, 'Average', 2, 'Interfaz de protocolo o línea de comandos'),
    (@DefaultConfigId, 'Complex', 3, 'Interfaz gráfica de usuario');

-- Pesos de Casos de Uso
INSERT INTO UseCasePointUseCaseWeights (ParameterConfigId, ComplexityType, Weight, TransactionsMin, TransactionsMax, Description)
VALUES 
    (@DefaultConfigId, 'Simple', 5, 1, 3, '1 a 3 transacciones'),
    (@DefaultConfigId, 'Average', 10, 4, 7, '4 a 7 transacciones'),
    (@DefaultConfigId, 'Complex', 15, 8, NULL, 'Más de 7 transacciones');

-- Factores Técnicos UCP
INSERT INTO UseCasePointTechnicalFactors (Code, Name, Weight, Description)
VALUES 
    ('T1', 'Distributed System', 2.0, 'Sistema distribuido'),
    ('T2', 'Response Time', 1.0, 'Tiempo de respuesta'),
    ('T3', 'End-user Efficiency', 1.0, 'Eficiencia del usuario'),
    ('T4', 'Complex Processing', 1.0, 'Procesamiento complejo'),
    ('T5', 'Reusable Code', 1.0, 'Código reusable'),
    ('T6', 'Easy to Install', 0.5, 'Fácil de instalar'),
    ('T7', 'Easy to Use', 0.5, 'Fácil de usar'),
    ('T8', 'Portable', 2.0, 'Portátil'),
    ('T9', 'Easy to Change', 1.0, 'Fácil de cambiar'),
    ('T10', 'Concurrent', 1.0, 'Concurrente'),
    ('T11', 'Security Features', 1.0, 'Características de seguridad'),
    ('T12', 'Direct Access for Third Parties', 1.0, 'Acceso directo para terceros'),
    ('T13', 'Special Training', 1.0, 'Entrenamiento especial');

-- Valores default (Nominal = 3 para todos)
DECLARE @UCPTechnicalId INT;
DECLARE UCPTech_CURSOR CURSOR FOR 
    SELECT TechnicalFactorId FROM UseCasePointTechnicalFactors;

OPEN UCPTech_CURSOR;
FETCH NEXT FROM UCPTech_CURSOR INTO @UCPTechnicalId;

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO UseCasePointTechnicalFactorValues (TechnicalFactorId, ParameterConfigId, AssignedValue)
    VALUES (@UCPTechnicalId, @DefaultConfigId, 3);
    
    FETCH NEXT FROM UCPTech_CURSOR INTO @UCPTechnicalId;
END

CLOSE UCPTech_CURSOR;
DEALLOCATE UCPTech_CURSOR;

-- Factores Ambientales UCP
INSERT INTO UseCasePointEnvironmentalFactors (Code, Name, Weight, Description)
VALUES 
    ('E1', 'Familiar with Development Process', 1.5, 'Familiaridad con el proceso de desarrollo'),
    ('E2', 'Application Experience', 0.5, 'Experiencia en la aplicación'),
    ('E3', 'Object-Oriented Experience', 1.0, 'Experiencia orientada a objetos'),
    ('E4', 'Lead Analyst Capability', 0.5, 'Capacidad del analista líder'),
    ('E5', 'Motivation', 1.0, 'Motivación'),
    ('E6', 'Stable Requirements', 2.0, 'Requisitos estables'),
    ('E7', 'Part-time Workers', -1.0, 'Trabajadores de medio tiempo'),
    ('E8', 'Difficult Programming Language', -1.0, 'Lenguaje de programación difícil');

-- Valores default (Nominal = 3 para todos)
DECLARE @UCPEnvId INT;
DECLARE UCPEnv_CURSOR CURSOR FOR 
    SELECT EnvironmentalFactorId FROM UseCasePointEnvironmentalFactors;

OPEN UCPEnv_CURSOR;
FETCH NEXT FROM UCPEnv_CURSOR INTO @UCPEnvId;

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO UseCasePointEnvironmentalFactorValues (EnvironmentalFactorId, ParameterConfigId, AssignedValue)
    VALUES (@UCPEnvId, @DefaultConfigId, 3);
    
    FETCH NEXT FROM UCPEnv_CURSOR INTO @UCPEnvId;
END

CLOSE UCPEnv_CURSOR;
DEALLOCATE UCPEnv_CURSOR;

-- =============================================
-- KLOC - PRODUCTIVIDAD POR LENGUAJE
-- =============================================

INSERT INTO KLOCLanguageProductivity (ParameterConfigId, LanguageName, LinesPerPersonMonth, AverageDefectsPerKLOC, Description)
VALUES 
    (@DefaultConfigId, 'Assembly', 320, 10.0, 'Lenguaje ensamblador'),
    (@DefaultConfigId, 'C', 150, 7.5, 'Lenguaje C'),
    (@DefaultConfigId, 'C++', 50, 5.0, 'C++ orientado a objetos'),
    (@DefaultConfigId, 'C#', 55, 4.5, 'C# .NET'),
    (@DefaultConfigId, 'Java', 50, 4.5, 'Java'),
    (@DefaultConfigId, 'JavaScript', 60, 5.0, 'JavaScript/TypeScript'),
    (@DefaultConfigId, 'Python', 60, 4.0, 'Python'),
    (@DefaultConfigId, 'Ruby', 60, 4.0, 'Ruby'),
    (@DefaultConfigId, 'PHP', 55, 5.0, 'PHP'),
    (@DefaultConfigId, 'Visual Basic', 45, 5.0, 'Visual Basic .NET'),
    (@DefaultConfigId, 'SQL', 100, 3.0, 'SQL y stored procedures'),
    (@DefaultConfigId, 'HTML/CSS', 150, 2.0, 'Markup y estilos'),
    (@DefaultConfigId, 'Swift', 50, 4.5, 'Swift (iOS)'),
    (@DefaultConfigId, 'Kotlin', 50, 4.5, 'Kotlin (Android)'),
    (@DefaultConfigId, 'Go', 70, 4.0, 'Go/Golang');

-- Factores de ajuste KLOC
INSERT INTO KLOCAdjustmentFactors (ParameterConfigId, FactorName, FactorValue, Description)
VALUES 
    (@DefaultConfigId, 'Team Experience Factor', 1.0, 'Factor de experiencia del equipo'),
    (@DefaultConfigId, 'Complexity Factor', 1.0, 'Factor de complejidad del proyecto'),
    (@DefaultConfigId, 'Tool Support Factor', 1.0, 'Factor de soporte de herramientas');

GO

PRINT 'Datos iniciales cargados exitosamente';
