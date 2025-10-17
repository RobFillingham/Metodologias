-- =============================================
-- Sistema de Estimación de Proyectos de Software
-- Soporta: KLOC, Puntos de Función, Casos de Uso, COCOMO II
-- =============================================

-- =============================================
-- TABLAS DE CONFIGURACIÓN Y PARÁMETROS
-- =============================================

-- Tabla de Empresas/Clientes
CREATE TABLE Companies (
    CompanyId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Tabla de Usuarios
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(200) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- =============================================
-- PARÁMETROS GENERALES
-- =============================================

-- Tabla para almacenar configuraciones de parámetros por empresa
-- Permite usar parámetros default o personalizados por empresa
CREATE TABLE ParameterConfigurations (
    ParameterConfigId INT PRIMARY KEY IDENTITY(1,1),
    CompanyId INT NULL, -- NULL = Configuración Default
    ConfigurationName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    IsDefault BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CompanyId) REFERENCES Companies(CompanyId)
);

-- =============================================
-- PARÁMETROS PARA COCOMO II
-- =============================================

-- Parámetros de calibración COCOMO II (A, B, C, D)
CREATE TABLE CocomoCalibrationParameters (
    CalibrationId INT PRIMARY KEY IDENTITY(1,1),
    ParameterConfigId INT NOT NULL,
    Stage NVARCHAR(50) NOT NULL, -- 'EarlyDesign', 'PostArchitecture', 'Reuse'
    ParameterA DECIMAL(10,4) NOT NULL DEFAULT 2.94,
    ParameterB DECIMAL(10,4) NOT NULL DEFAULT 0.91,
    ParameterC DECIMAL(10,4) NOT NULL DEFAULT 3.67,
    ParameterD DECIMAL(10,4) NOT NULL DEFAULT 0.28,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId)
);

-- Multiplicadores de Esfuerzo COCOMO II (Cost Drivers)
CREATE TABLE CocomoCostDrivers (
    CostDriverId INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(50) NOT NULL, -- RELY, DATA, CPLX, etc.
    Name NVARCHAR(200) NOT NULL,
    Category NVARCHAR(100) NOT NULL, -- Product, Platform, Personnel, Project
    Description NVARCHAR(500),
    ApplicableStage NVARCHAR(50) NOT NULL -- 'EarlyDesign', 'PostArchitecture', 'Both'
);

-- Valores de los Cost Drivers
CREATE TABLE CocomoCostDriverValues (
    CostDriverValueId INT PRIMARY KEY IDENTITY(1,1),
    CostDriverId INT NOT NULL,
    ParameterConfigId INT NOT NULL,
    RatingLevel NVARCHAR(50) NOT NULL, -- VeryLow, Low, Nominal, High, VeryHigh, ExtraHigh
    Multiplier DECIMAL(10,4) NOT NULL,
    FOREIGN KEY (CostDriverId) REFERENCES CocomoCostDrivers(CostDriverId),
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId)
);

-- Factores de Escala COCOMO II
CREATE TABLE CocomoScaleFactors (
    ScaleFactorId INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(50) NOT NULL, -- PREC, FLEX, RESL, TEAM, PMAT
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500)
);

-- Valores de los Factores de Escala
CREATE TABLE CocomoScaleFactorValues (
    ScaleFactorValueId INT PRIMARY KEY IDENTITY(1,1),
    ScaleFactorId INT NOT NULL,
    ParameterConfigId INT NOT NULL,
    RatingLevel NVARCHAR(50) NOT NULL, -- VeryLow, Low, Nominal, High, VeryHigh, ExtraHigh
    Value DECIMAL(10,4) NOT NULL,
    FOREIGN KEY (ScaleFactorId) REFERENCES CocomoScaleFactors(ScaleFactorId),
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId)
);

-- =============================================
-- PARÁMETROS PARA PUNTOS DE FUNCIÓN
-- =============================================

-- Complejidades para Archivos Lógicos Internos (ILF)
CREATE TABLE FunctionPointILFComplexity (
    ILFComplexityId INT PRIMARY KEY IDENTITY(1,1),
    ParameterConfigId INT NOT NULL,
    DataElements NVARCHAR(50) NOT NULL, -- '1-19', '20-50', '51+'
    RecordTypes NVARCHAR(50) NOT NULL, -- '1', '2-5', '6+'
    Complexity NVARCHAR(20) NOT NULL, -- Low, Average, High
    Weight INT NOT NULL,
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId)
);

-- Complejidades para Archivos de Interfaz Externa (EIF)
CREATE TABLE FunctionPointEIFComplexity (
    EIFComplexityId INT PRIMARY KEY IDENTITY(1,1),
    ParameterConfigId INT NOT NULL,
    DataElements NVARCHAR(50) NOT NULL,
    RecordTypes NVARCHAR(50) NOT NULL,
    Complexity NVARCHAR(20) NOT NULL,
    Weight INT NOT NULL,
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId)
);

-- Complejidades para Entradas Externas (EI)
CREATE TABLE FunctionPointEIComplexity (
    EIComplexityId INT PRIMARY KEY IDENTITY(1,1),
    ParameterConfigId INT NOT NULL,
    DataElements NVARCHAR(50) NOT NULL,
    FileTypes NVARCHAR(50) NOT NULL,
    Complexity NVARCHAR(20) NOT NULL,
    Weight INT NOT NULL,
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId)
);

-- Complejidades para Salidas Externas (EO)
CREATE TABLE FunctionPointEOComplexity (
    EOComplexityId INT PRIMARY KEY IDENTITY(1,1),
    ParameterConfigId INT NOT NULL,
    DataElements NVARCHAR(50) NOT NULL,
    FileTypes NVARCHAR(50) NOT NULL,
    Complexity NVARCHAR(20) NOT NULL,
    Weight INT NOT NULL,
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId)
);

-- Complejidades para Consultas Externas (EQ)
CREATE TABLE FunctionPointEQComplexity (
    EQComplexityId INT PRIMARY KEY IDENTITY(1,1),
    ParameterConfigId INT NOT NULL,
    DataElements NVARCHAR(50) NOT NULL,
    FileTypes NVARCHAR(50) NOT NULL,
    Complexity NVARCHAR(20) NOT NULL,
    Weight INT NOT NULL,
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId)
);

-- Factores de Ajuste de Complejidad Técnica (TCF)
CREATE TABLE FunctionPointTechnicalFactors (
    TechnicalFactorId INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(10) NOT NULL, -- F1-F14
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500)
);

-- Valores de Factores Técnicos
CREATE TABLE FunctionPointTechnicalFactorValues (
    TechnicalFactorValueId INT PRIMARY KEY IDENTITY(1,1),
    TechnicalFactorId INT NOT NULL,
    ParameterConfigId INT NOT NULL,
    InfluenceLevel INT NOT NULL CHECK (InfluenceLevel BETWEEN 0 AND 5),
    FOREIGN KEY (TechnicalFactorId) REFERENCES FunctionPointTechnicalFactors(TechnicalFactorId),
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId)
);

-- =============================================
-- PARÁMETROS PARA PUNTOS DE CASOS DE USO
-- =============================================

-- Pesos de Actores
CREATE TABLE UseCasePointActorWeights (
    ActorWeightId INT PRIMARY KEY IDENTITY(1,1),
    ParameterConfigId INT NOT NULL,
    ComplexityType NVARCHAR(50) NOT NULL, -- Simple, Average, Complex
    Weight INT NOT NULL,
    Description NVARCHAR(500),
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId)
);

-- Pesos de Casos de Uso
CREATE TABLE UseCasePointUseCaseWeights (
    UseCaseWeightId INT PRIMARY KEY IDENTITY(1,1),
    ParameterConfigId INT NOT NULL,
    ComplexityType NVARCHAR(50) NOT NULL, -- Simple, Average, Complex
    Weight INT NOT NULL,
    TransactionsMin INT NOT NULL,
    TransactionsMax INT NULL,
    Description NVARCHAR(500),
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId)
);

-- Factores Técnicos UCP
CREATE TABLE UseCasePointTechnicalFactors (
    TechnicalFactorId INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(10) NOT NULL, -- T1-T13
    Name NVARCHAR(200) NOT NULL,
    Weight DECIMAL(10,4) NOT NULL,
    Description NVARCHAR(500)
);

-- Valores de Factores Técnicos UCP
CREATE TABLE UseCasePointTechnicalFactorValues (
    TechnicalFactorValueId INT PRIMARY KEY IDENTITY(1,1),
    TechnicalFactorId INT NOT NULL,
    ParameterConfigId INT NOT NULL,
    AssignedValue INT NOT NULL CHECK (AssignedValue BETWEEN 0 AND 5),
    FOREIGN KEY (TechnicalFactorId) REFERENCES UseCasePointTechnicalFactors(TechnicalFactorId),
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId)
);

-- Factores Ambientales UCP
CREATE TABLE UseCasePointEnvironmentalFactors (
    EnvironmentalFactorId INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(10) NOT NULL, -- E1-E8
    Name NVARCHAR(200) NOT NULL,
    Weight DECIMAL(10,4) NOT NULL,
    Description NVARCHAR(500)
);

-- Valores de Factores Ambientales UCP
CREATE TABLE UseCasePointEnvironmentalFactorValues (
    EnvironmentalFactorValueId INT PRIMARY KEY IDENTITY(1,1),
    EnvironmentalFactorId INT NOT NULL,
    ParameterConfigId INT NOT NULL,
    AssignedValue INT NOT NULL CHECK (AssignedValue BETWEEN 0 AND 5),
    FOREIGN KEY (EnvironmentalFactorId) REFERENCES UseCasePointEnvironmentalFactors(EnvironmentalFactorId),
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId)
);

-- =============================================
-- PARÁMETROS PARA KLOC
-- =============================================

-- Productividad por Lenguaje de Programación
CREATE TABLE KLOCLanguageProductivity (
    LanguageProductivityId INT PRIMARY KEY IDENTITY(1,1),
    ParameterConfigId INT NOT NULL,
    LanguageName NVARCHAR(100) NOT NULL,
    LinesPerPersonMonth DECIMAL(10,2) NOT NULL, -- Líneas de código por persona-mes
    AverageDefectsPerKLOC DECIMAL(10,4) NULL,
    Description NVARCHAR(500),
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId)
);

-- Factores de Ajuste KLOC
CREATE TABLE KLOCAdjustmentFactors (
    AdjustmentFactorId INT PRIMARY KEY IDENTITY(1,1),
    ParameterConfigId INT NOT NULL,
    FactorName NVARCHAR(200) NOT NULL,
    FactorValue DECIMAL(10,4) NOT NULL,
    Description NVARCHAR(500),
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId)
);

-- =============================================
-- TABLAS DE PROYECTOS Y ESTIMACIONES
-- =============================================

-- Proyectos
CREATE TABLE Projects (
    ProjectId INT PRIMARY KEY IDENTITY(1,1),
    CompanyId INT NOT NULL,
    UserId INT NOT NULL, -- Usuario que crea el proyecto
    ProjectName NVARCHAR(200) NOT NULL,
    ProjectDescription NVARCHAR(1000),
    ProjectType NVARCHAR(100), -- Web, Desktop, Mobile, etc.
    ProgrammingLanguage NVARCHAR(100),
    Status NVARCHAR(50) NOT NULL DEFAULT 'Active', -- Active, Completed, Archived
    StartDate DATE,
    EndDate DATE,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CompanyId) REFERENCES Companies(CompanyId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Estimaciones (Control de Versiones)
CREATE TABLE Estimations (
    EstimationId INT PRIMARY KEY IDENTITY(1,1),
    ProjectId INT NOT NULL,
    EstimationMethod NVARCHAR(50) NOT NULL, -- KLOC, FunctionPoints, UseCasePoints, COCOMO_Early, COCOMO_Post
    VersionNumber INT NOT NULL,
    ParameterConfigId INT NOT NULL, -- Configuración de parámetros usada
    EstimationName NVARCHAR(200),
    EstimationDescription NVARCHAR(1000),
    EstimatedEffort DECIMAL(18,2), -- Persona-Mes
    EstimatedTime DECIMAL(18,2), -- Meses
    EstimatedCost DECIMAL(18,2), -- Costo en moneda
    Currency NVARCHAR(10) DEFAULT 'USD',
    CostPerPersonMonth DECIMAL(18,2), -- Costo por persona-mes
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    Notes NVARCHAR(MAX),
    FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId),
    FOREIGN KEY (ParameterConfigId) REFERENCES ParameterConfigurations(ParameterConfigId),
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT UQ_Project_Method_Version UNIQUE (ProjectId, EstimationMethod, VersionNumber)
);

-- =============================================
-- DATOS ESPECÍFICOS DE ESTIMACIÓN KLOC
-- =============================================

CREATE TABLE EstimationKLOC (
    EstimationKLOCId INT PRIMARY KEY IDENTITY(1,1),
    EstimationId INT NOT NULL,
    OptimisticKLOC DECIMAL(18,2) NOT NULL,
    MostLikelyKLOC DECIMAL(18,2) NOT NULL,
    PessimisticKLOC DECIMAL(18,2) NOT NULL,
    ExpectedKLOC DECIMAL(18,2) NOT NULL, -- Calculado: (O + 4M + P) / 6
    LanguageProductivityId INT NOT NULL,
    AdjustmentFactor DECIMAL(10,4) DEFAULT 1.0,
    FOREIGN KEY (EstimationId) REFERENCES Estimations(EstimationId),
    FOREIGN KEY (LanguageProductivityId) REFERENCES KLOCLanguageProductivity(LanguageProductivityId)
);

-- =============================================
-- DATOS ESPECÍFICOS DE PUNTOS DE FUNCIÓN
-- =============================================

CREATE TABLE EstimationFunctionPoints (
    EstimationFPId INT PRIMARY KEY IDENTITY(1,1),
    EstimationId INT NOT NULL,
    -- Conteos sin ajustar
    ILF_Low INT DEFAULT 0,
    ILF_Average INT DEFAULT 0,
    ILF_High INT DEFAULT 0,
    EIF_Low INT DEFAULT 0,
    EIF_Average INT DEFAULT 0,
    EIF_High INT DEFAULT 0,
    EI_Low INT DEFAULT 0,
    EI_Average INT DEFAULT 0,
    EI_High INT DEFAULT 0,
    EO_Low INT DEFAULT 0,
    EO_Average INT DEFAULT 0,
    EO_High INT DEFAULT 0,
    EQ_Low INT DEFAULT 0,
    EQ_Average INT DEFAULT 0,
    EQ_High INT DEFAULT 0,
    -- Resultados
    UnadjustedFunctionPoints INT NOT NULL,
    TechnicalComplexityFactor DECIMAL(10,4) NOT NULL,
    AdjustedFunctionPoints DECIMAL(18,2) NOT NULL,
    LanguageLOCPerFP INT NOT NULL, -- LOC por punto de función según lenguaje
    EstimatedLOC DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (EstimationId) REFERENCES Estimations(EstimationId)
);

-- Detalles de Factores Técnicos aplicados en la estimación
CREATE TABLE EstimationFPTechnicalFactors (
    EstimationFPTechnicalId INT PRIMARY KEY IDENTITY(1,1),
    EstimationFPId INT NOT NULL,
    TechnicalFactorId INT NOT NULL,
    InfluenceLevel INT NOT NULL CHECK (InfluenceLevel BETWEEN 0 AND 5),
    FOREIGN KEY (EstimationFPId) REFERENCES EstimationFunctionPoints(EstimationFPId),
    FOREIGN KEY (TechnicalFactorId) REFERENCES FunctionPointTechnicalFactors(TechnicalFactorId)
);

-- =============================================
-- DATOS ESPECÍFICOS DE PUNTOS DE CASOS DE USO
-- =============================================

CREATE TABLE EstimationUseCasePoints (
    EstimationUCPId INT PRIMARY KEY IDENTITY(1,1),
    EstimationId INT NOT NULL,
    -- Actores
    SimpleActors INT DEFAULT 0,
    AverageActors INT DEFAULT 0,
    ComplexActors INT DEFAULT 0,
    UnadjustedActorWeight INT NOT NULL,
    -- Casos de Uso
    SimpleUseCases INT DEFAULT 0,
    AverageUseCases INT DEFAULT 0,
    ComplexUseCases INT DEFAULT 0,
    UnadjustedUseCaseWeight INT NOT NULL,
    -- Factores
    UnadjustedUseCasePoints INT NOT NULL,
    TechnicalComplexityFactor DECIMAL(10,4) NOT NULL,
    EnvironmentalComplexityFactor DECIMAL(10,4) NOT NULL,
    UseCasePoints DECIMAL(18,2) NOT NULL,
    HoursPerUCP DECIMAL(10,2) NOT NULL, -- Horas por UCP (típicamente 20)
    EstimatedHours DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (EstimationId) REFERENCES Estimations(EstimationId)
);

-- Factores Técnicos aplicados en UCP
CREATE TABLE EstimationUCPTechnicalFactors (
    EstimationUCPTechnicalId INT PRIMARY KEY IDENTITY(1,1),
    EstimationUCPId INT NOT NULL,
    TechnicalFactorId INT NOT NULL,
    AssignedValue INT NOT NULL CHECK (AssignedValue BETWEEN 0 AND 5),
    FOREIGN KEY (EstimationUCPId) REFERENCES EstimationUseCasePoints(EstimationUCPId),
    FOREIGN KEY (TechnicalFactorId) REFERENCES UseCasePointTechnicalFactors(TechnicalFactorId)
);

-- Factores Ambientales aplicados en UCP
CREATE TABLE EstimationUCPEnvironmentalFactors (
    EstimationUCPEnvironmentalId INT PRIMARY KEY IDENTITY(1,1),
    EstimationUCPId INT NOT NULL,
    EnvironmentalFactorId INT NOT NULL,
    AssignedValue INT NOT NULL CHECK (AssignedValue BETWEEN 0 AND 5),
    FOREIGN KEY (EstimationUCPId) REFERENCES EstimationUseCasePoints(EstimationUCPId),
    FOREIGN KEY (EnvironmentalFactorId) REFERENCES UseCasePointEnvironmentalFactors(EnvironmentalFactorId)
);

-- =============================================
-- DATOS ESPECÍFICOS DE COCOMO II
-- =============================================

CREATE TABLE EstimationCOCOMO (
    EstimationCOCOMOId INT PRIMARY KEY IDENTITY(1,1),
    EstimationId INT NOT NULL,
    Stage NVARCHAR(50) NOT NULL, -- EarlyDesign, PostArchitecture
    SizeInKLOC DECIMAL(18,2) NOT NULL,
    CalibrationId INT NOT NULL,
    -- Exponente de Escala
    ScaleFactorSum DECIMAL(10,4) NOT NULL,
    ExponentB DECIMAL(10,4) NOT NULL,
    -- Multiplicador de Esfuerzo
    EffortMultiplier DECIMAL(10,4) NOT NULL,
    -- Resultados
    NominalEffort DECIMAL(18,2) NOT NULL, -- PM sin ajuste
    AdjustedEffort DECIMAL(18,2) NOT NULL, -- PM con ajuste
    FOREIGN KEY (EstimationId) REFERENCES Estimations(EstimationId),
    FOREIGN KEY (CalibrationId) REFERENCES CocomoCalibrationParameters(CalibrationId)
);

-- Factores de Escala aplicados
CREATE TABLE EstimationCOCOMOScaleFactors (
    EstimationCOCOMOScaleId INT PRIMARY KEY IDENTITY(1,1),
    EstimationCOCOMOId INT NOT NULL,
    ScaleFactorId INT NOT NULL,
    RatingLevel NVARCHAR(50) NOT NULL,
    Value DECIMAL(10,4) NOT NULL,
    FOREIGN KEY (EstimationCOCOMOId) REFERENCES EstimationCOCOMO(EstimationCOCOMOId),
    FOREIGN KEY (ScaleFactorId) REFERENCES CocomoScaleFactors(ScaleFactorId)
);

-- Cost Drivers aplicados
CREATE TABLE EstimationCOCOMOCostDrivers (
    EstimationCOCOMOCostId INT PRIMARY KEY IDENTITY(1,1),
    EstimationCOCOMOId INT NOT NULL,
    CostDriverId INT NOT NULL,
    RatingLevel NVARCHAR(50) NOT NULL,
    Multiplier DECIMAL(10,4) NOT NULL,
    FOREIGN KEY (EstimationCOCOMOId) REFERENCES EstimationCOCOMO(EstimationCOCOMOId),
    FOREIGN KEY (CostDriverId) REFERENCES CocomoCostDrivers(CostDriverId)
);

-- =============================================
-- TABLAS DE AUDITORÍA Y REGISTRO
-- =============================================

-- Log de cambios en estimaciones
CREATE TABLE EstimationHistory (
    HistoryId INT PRIMARY KEY IDENTITY(1,1),
    EstimationId INT NOT NULL,
    ChangeType NVARCHAR(50) NOT NULL, -- Created, Modified, Archived
    ChangedBy INT NOT NULL,
    ChangeDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    OldValues NVARCHAR(MAX), -- JSON con valores anteriores
    NewValues NVARCHAR(MAX), -- JSON con valores nuevos
    Comments NVARCHAR(1000),
    FOREIGN KEY (EstimationId) REFERENCES Estimations(EstimationId),
    FOREIGN KEY (ChangedBy) REFERENCES Users(UserId)
);

-- Log de uso del sistema
CREATE TABLE AuditLog (
    AuditId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT,
    Action NVARCHAR(100) NOT NULL,
    EntityType NVARCHAR(100),
    EntityId INT,
    Details NVARCHAR(MAX),
    IPAddress NVARCHAR(50),
    Timestamp DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- =============================================
-- ÍNDICES PARA MEJORAR RENDIMIENTO
-- =============================================

CREATE INDEX IX_Projects_CompanyId ON Projects(CompanyId);
CREATE INDEX IX_Projects_UserId ON Projects(UserId);
CREATE INDEX IX_Projects_Status ON Projects(Status);
CREATE INDEX IX_Estimations_ProjectId ON Estimations(ProjectId);
CREATE INDEX IX_Estimations_Method ON Estimations(EstimationMethod);
CREATE INDEX IX_Estimations_CreatedAt ON Estimations(CreatedAt);
CREATE INDEX IX_ParameterConfigurations_CompanyId ON ParameterConfigurations(CompanyId);
CREATE INDEX IX_ParameterConfigurations_IsDefault ON ParameterConfigurations(IsDefault);
CREATE INDEX IX_EstimationHistory_EstimationId ON EstimationHistory(EstimationId);
CREATE INDEX IX_AuditLog_UserId ON AuditLog(UserId);
CREATE INDEX IX_AuditLog_Timestamp ON AuditLog(Timestamp);

GO
