-- =====================================================
-- Function Points (FP) Estimation - Database
-- Script limpio sin conflictos con otros métodos
-- =====================================================

USE MetodologiasDB;

-- Tabla principal de estimaciones por Puntos de Función
CREATE TABLE IF NOT EXISTS function_point_estimation (
    fp_estimation_id INT PRIMARY KEY AUTO_INCREMENT,
    project_id INT NOT NULL,
    estimation_name VARCHAR(255) NOT NULL,
    external_inputs INT COMMENT 'Cantidad de entradas externas',
    external_outputs INT COMMENT 'Cantidad de salidas externas',
    external_inquiries INT COMMENT 'Cantidad de consultas externas',
    internal_logical_files INT COMMENT 'Cantidad de archivos lógicos internos',
    external_interface_files INT COMMENT 'Cantidad de archivos de interfaz externa',
    complexity_level ENUM('LOW', 'AVERAGE', 'HIGH') DEFAULT 'AVERAGE' COMMENT 'Nivel de complejidad general',
    unadjusted_fp DECIMAL(10,2) COMMENT 'Puntos de función sin ajustar',
    value_adjustment_factor DECIMAL(10,4) COMMENT 'Factor de ajuste de valor',
    adjusted_fp DECIMAL(10,2) COMMENT 'Puntos de función ajustados',
    estimated_effort DECIMAL(10,2) COMMENT 'Esfuerzo estimado en persona-mes',
    estimated_cost DECIMAL(15,2) COMMENT 'Costo estimado en moneda base',
    estimated_time DECIMAL(10,2) COMMENT 'Tiempo estimado en meses',
    notes TEXT COMMENT 'Notas adicionales sobre la estimación',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (project_id) REFERENCES project(project_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabla para características técnicas (Technical Characteristics)
CREATE TABLE IF NOT EXISTS function_point_characteristics (
    fp_char_id INT PRIMARY KEY AUTO_INCREMENT,
    fp_estimation_id INT NOT NULL,
    characteristic_name VARCHAR(100) NOT NULL COMMENT 'Nombre de la característica (ej: Performance, Security, etc)',
    influence_level ENUM('NONE', 'INCIDENTAL', 'MODERATE', 'AVERAGE', 'SIGNIFICANT', 'ESSENTIAL') DEFAULT 'AVERAGE' COMMENT 'Nivel de influencia',
    score INT COMMENT 'Puntuación (0-5)',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (fp_estimation_id) REFERENCES function_point_estimation(fp_estimation_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Índices para optimizar consultas
CREATE INDEX idx_fp_project ON function_point_estimation(project_id);
CREATE INDEX idx_fp_created ON function_point_estimation(created_at);
CREATE INDEX idx_fp_char_estimation ON function_point_characteristics(fp_estimation_id);
