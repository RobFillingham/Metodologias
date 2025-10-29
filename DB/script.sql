-- Database Creation
CREATE DATABASE IF NOT EXISTS MetodologiasDB;

USE MetodologiasDB;

-- Users Table
CREATE TABLE IF NOT EXISTS Users (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(500) NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);


-- Tabla de Proyectos: Contenedor principal para las estimaciones.
CREATE TABLE Project (
    project_id INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT NOT NULL, -- Modificado para referenciar Users(Id)
    project_name VARCHAR(255) NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) -- Modificado
);

