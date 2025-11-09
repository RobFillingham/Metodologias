# Instrucciones para Recrear la Base de Datos desde Cero

## ⚠️ ADVERTENCIA
Este proceso **eliminará todos los datos** de la base de datos. Asegúrate de hacer un respaldo si tienes datos importantes.

## Paso 1: Respaldar datos (Opcional)

Si tienes datos que quieres conservar:
```sql
-- En MySQL Workbench o phpMyAdmin
USE metodologias;

-- Exportar usuarios (si quieres conservarlos)
SELECT * FROM Users;

-- Exportar proyectos existentes
SELECT * FROM Project;
```

## Paso 2: Eliminar la base de datos completa

### En MySQL Workbench:
1. Abre MySQL Workbench
2. Conéctate a tu servidor
3. Ejecuta:
```sql
DROP DATABASE IF EXISTS metodologias;
CREATE DATABASE metodologias CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE metodologias;
```

### En phpMyAdmin:
1. Abre phpMyAdmin
2. Selecciona la base de datos `metodologias`
3. Clic en "Operaciones" → "Eliminar base de datos"
4. Confirma la eliminación
5. Crea una nueva base de datos con el nombre `metodologias`

## Paso 3: Ejecutar los scripts en orden

**IMPORTANTE**: Los scripts deben ejecutarse en este orden exacto:

### 1. Script principal (Usuarios, Auth, Project compartido)
```
DB/script.sql
```
Este script crea:
- Tabla `Users`
- Tabla `Project` (compartida por todos los métodos)
- Otras tablas base del sistema

### 2. KLOC
```
DB/kloc_clean.sql
```

### 3. Function Points
```
DB/function_points_clean.sql
```

### 4. Use Case Points
```
DB/use_case_points_clean.sql
```

### 5. COCOMO 1
```
DB/cocomo1_clean.sql
```

### 6. COCOMO III (COCOMO 2 Post-Architecture)
```
DB/Cocomo3.sql
```
Este script crea:
- Tabla `Language` (compartida)
- Tablas de COCOMO III

### 7. COCOMO II Stage 1 (Composition Model)
```
DB/cocomo2_stage1.sql
```
Este script:
- Reutiliza la tabla `Project` del script principal
- Reutiliza la tabla `Language` de COCOMO III
- Crea tablas específicas: `ParameterSetCocomo2Stage1`, `EstimationCocomo2Stage1`, `EstimationComponentCocomo2Stage1`

### 8. COCOMO II Stage 3 (Post-Architecture with Function Points)
```
DB/CocomoIIStage3.sql
```
Este script:
- Reutiliza la tabla `Project` del script principal
- Crea tablas específicas: `LanguageCocomoIIStage3`, `ParameterSetCocomoIIStage3`, `EstimationCocomoIIStage3`, `EstimationFunctionCocomoIIStage3`
- Incluye datos iniciales con valores por defecto de COCOMO II

## Paso 4: Verificar la estructura

Ejecuta este query para verificar que todas las tablas se crearon correctamente:

```sql
USE metodologias;

-- Ver todas las tablas
SHOW TABLES;

-- Verificar que EstimationCocomo2Stage1 use la tabla Project correcta
SHOW CREATE TABLE EstimationCocomo2Stage1;

-- Ver las foreign keys de EstimationCocomo2Stage1
SELECT 
    CONSTRAINT_NAME,
    TABLE_NAME,
    COLUMN_NAME,
    REFERENCED_TABLE_NAME,
    REFERENCED_COLUMN_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = 'metodologias'
  AND TABLE_NAME = 'EstimationCocomo2Stage1'
  AND REFERENCED_TABLE_NAME IS NOT NULL;
```

Deberías ver que `EstimationCocomo2Stage1` tiene una FK `project_id` apuntando a `Project(project_id)`.

## Paso 5: Reiniciar el Backend

Después de recrear la BD:

1. Detén el backend si está corriendo (Ctrl+C en la terminal)
2. Vuelve a iniciar el backend:
```bash
cd Backend
dotnet run
```

3. El backend debería iniciar sin errores

## Paso 6: Probar en el Frontend

1. Inicia sesión en la aplicación
2. Crea un proyecto nuevo
3. Ve a "COCOMO II Stage 1"
4. Intenta crear una nueva estimación
5. Debería funcionar correctamente

## Tablas Compartidas

Estas tablas son compartidas por múltiples métodos:

| Tabla | Usada por |
|-------|-----------|
| `Users` | Todos los métodos |
| `Project` | COCOMO II Stage 1, COCOMO II Stage 3, COCOMO III, KLOC, Function Point, Use Case Point |
| `Language` | COCOMO II Stage 1, COCOMO III |

**Nota**: `LanguageCocomoIIStage3` es una tabla separada específica para COCOMO II Stage 3.

## Solución de Problemas

### Error: "Cannot add foreign key constraint"
- Asegúrate de ejecutar `Cocomo3.sql` ANTES de `cocomo2_stage1.sql`
- La tabla `Language` debe existir antes de crear `EstimationCocomo2Stage1`

### Error: "Table 'Project' doesn't exist"
- Asegúrate de ejecutar `script.sql` primero
- La tabla `Project` debe existir antes de `cocomo2_stage1.sql`

### Error al crear estimación en el frontend
- Verifica que la FK de `EstimationCocomo2Stage1` apunte a `Project` y no a `ProjectCocomo2Stage1`
- Revisa los logs del backend para ver el error específico
