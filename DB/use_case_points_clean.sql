-- =====================================================
-- Use Case Points (UCP) Estimation - Database
-- Script limpio sin conflictos con otros métodos
-- =====================================================

USE MetodologiasDB;

-- Tabla principal de estimaciones por Puntos de Casos de Uso
CREATE TABLE IF NOT EXISTS use_case_point_estimation (
    ucp_estimation_id INT PRIMARY KEY AUTO_INCREMENT,
    project_id INT NOT NULL,
    estimation_name VARCHAR(255) NOT NULL,
    simple_ucc_count INT COMMENT 'Cantidad de casos de uso simples',
    average_ucc_count INT COMMENT 'Cantidad de casos de uso promedio',
    complex_ucc_count INT COMMENT 'Cantidad de casos de uso complejos',
    simple_actor_count INT COMMENT 'Cantidad de actores simples',
    average_actor_count INT COMMENT 'Cantidad de actores promedio',
    complex_actor_count INT COMMENT 'Cantidad de actores complejos',
    unadjusted_ucp DECIMAL(10,2) COMMENT 'Puntos de casos de uso sin ajustar',
    technical_complexity_factor DECIMAL(10,4) COMMENT 'Factor de complejidad técnica (TCF)',
    environment_factor DECIMAL(10,4) COMMENT 'Factor de ambiente (EF)',
    adjusted_ucp DECIMAL(10,2) COMMENT 'Puntos de casos de uso ajustados',
    estimated_effort DECIMAL(10,2) COMMENT 'Esfuerzo estimado en persona-hora',
    estimated_effort_pm DECIMAL(10,2) COMMENT 'Esfuerzo estimado en persona-mes',
    estimated_cost DECIMAL(15,2) COMMENT 'Costo estimado en moneda base',
    estimated_time DECIMAL(10,2) COMMENT 'Tiempo estimado en meses',
    notes TEXT COMMENT 'Notas adicionales sobre la estimación',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (project_id) REFERENCES project(project_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabla para factores técnicos (Technical Factors)
CREATE TABLE IF NOT EXISTS use_case_technical_factors (
    ucp_tech_factor_id INT PRIMARY KEY AUTO_INCREMENT,
    ucp_estimation_id INT NOT NULL,
    factor_name VARCHAR(100) NOT NULL COMMENT 'Nombre del factor técnico (ej: Distributed system, Performance, Security, etc)',
    factor_weight INT COMMENT 'Peso del factor (0-5)',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (ucp_estimation_id) REFERENCES use_case_point_estimation(ucp_estimation_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabla para factores de ambiente (Environment Factors)
CREATE TABLE IF NOT EXISTS use_case_environment_factors (
    ucp_env_factor_id INT PRIMARY KEY AUTO_INCREMENT,
    ucp_estimation_id INT NOT NULL,
    factor_name VARCHAR(100) NOT NULL COMMENT 'Nombre del factor de ambiente (ej: Familiar with UML, Team Continuity, etc)',
    factor_weight INT COMMENT 'Peso del factor (0-5)',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (ucp_estimation_id) REFERENCES use_case_point_estimation(ucp_estimation_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Índices para optimizar consultas
CREATE INDEX idx_ucp_project ON use_case_point_estimation(project_id);
CREATE INDEX idx_ucp_created ON use_case_point_estimation(created_at);
CREATE INDEX idx_ucp_tech_factor_estimation ON use_case_technical_factors(ucp_estimation_id);
CREATE INDEX idx_ucp_env_factor_estimation ON use_case_environment_factors(ucp_estimation_id);
