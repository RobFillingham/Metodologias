-- =====================================================
-- KLOC (Lines of Code) Estimation - Database
-- Script limpio sin conflictos con COCOMO y otros métodos
-- =====================================================

USE MetodologiasDB;

-- Tabla de Estimaciones KLOC
CREATE TABLE IF NOT EXISTS kloc_estimation (
    kloc_estimation_id INT PRIMARY KEY AUTO_INCREMENT,
    project_id INT NOT NULL,
    estimation_name VARCHAR(255) NOT NULL,
    lines_of_code INT NOT NULL COMMENT 'Cantidad total de líneas de código',
    estimated_effort DECIMAL(10,2) COMMENT 'Esfuerzo estimado en persona-mes',
    estimated_cost DECIMAL(15,2) COMMENT 'Costo estimado en moneda base',
    estimated_time DECIMAL(10,2) COMMENT 'Tiempo estimado en meses',
    notes TEXT COMMENT 'Notas adicionales sobre la estimación',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (project_id) REFERENCES project(project_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Índices para optimizar consultas
CREATE INDEX idx_kloc_project ON kloc_estimation(project_id);
CREATE INDEX idx_kloc_created ON kloc_estimation(created_at);
