-- =====================================================
-- COCOMO Estadio 1 (COCOMO Básico) - Base de Datos
-- Script limpio sin conflictos con COCOMO 3
-- =====================================================

USE MetodologiasDB;

-- Tabla de Estimaciones COCOMO 1 Básicas
CREATE TABLE IF NOT EXISTS cocomo1_estimation (
    cocomo1_estimation_id INT PRIMARY KEY AUTO_INCREMENT,
    project_id INT NOT NULL,
    estimation_name VARCHAR(255) NOT NULL,
    kloc DECIMAL(10,2) NOT NULL COMMENT 'Tamaño en miles de líneas de código',
    mode ENUM('ORGANIC', 'SEMI_DETACHED', 'EMBEDDED') NOT NULL DEFAULT 'ORGANIC' COMMENT 'Modo de proyecto',
    effort_pm DECIMAL(10,2) COMMENT 'Esfuerzo en persona-mes',
    tdev_months DECIMAL(10,2) COMMENT 'Tiempo de desarrollo en meses',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (project_id) REFERENCES project(project_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Índices para optimizar consultas
CREATE INDEX idx_cocomo1_project ON cocomo1_estimation(project_id);
CREATE INDEX idx_cocomo1_created ON cocomo1_estimation(created_at);
