-- =====================================================
-- MIGRACIÓN: Arreglar constraint UNIQUE en UsoDigital
-- =====================================================
-- PROBLEMA: El constraint UNIQUE estaba solo sobre fechaRegistro,
--           lo que impedía que múltiples usuarios registraran datos el mismo día.
-- SOLUCIÓN: Cambiar a UNIQUE sobre (idUsuario, fechaRegistro)
--
-- CUÁNDO EJECUTAR: UNA sola vez en la BD de Azure antes de usar la app en producción
-- CÓMO EJECUTAR:   Azure Data Studio, SQL Server Management Studio, o portal de Azure
-- =====================================================

USE Kairos;
GO

-- 1. Eliminar el constraint UNIQUE incorrecto
--    (El nombre exacto puede variar si regeneraste la BD)
--    Si te da error "does not exist", significa que ya está arreglado.
IF EXISTS (
    SELECT * FROM sys.objects 
    WHERE type = 'UQ' 
    AND name = 'UQ__UsoDigit__D7F4BC9993E617A9'
)
BEGIN
    PRINT 'Eliminando constraint UNIQUE incorrecto...';
    ALTER TABLE UsoDigital DROP CONSTRAINT UQ__UsoDigit__D7F4BC9993E617A9;
    PRINT '✓ Constraint eliminado';
END
ELSE
BEGIN
    PRINT 'Constraint incorrecto ya no existe (OK)';
END
GO

-- 2. Crear el constraint UNIQUE correcto
--    Permite que cada usuario tenga UN registro por día
IF NOT EXISTS (
    SELECT * FROM sys.objects 
    WHERE type = 'UQ' 
    AND name = 'UQ_UsoDigital_Usuario_Fecha'
)
BEGIN
    PRINT 'Creando constraint UNIQUE correcto sobre (idUsuario, fechaRegistro)...';
    ALTER TABLE UsoDigital 
    ADD CONSTRAINT UQ_UsoDigital_Usuario_Fecha UNIQUE (idUsuario, fechaRegistro);
    PRINT '✓ Constraint correcto creado';
END
ELSE
BEGIN
    PRINT 'Constraint correcto ya existe (OK)';
END
GO

-- 3. Verificación
PRINT '';
PRINT '=== VERIFICACIÓN ===';
SELECT 
    i.name AS 'Constraint Name',
    COL_NAME(ic.object_id, ic.column_id) AS 'Column Name'
FROM sys.index_columns ic
INNER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id
WHERE i.type_desc = 'UNIQUE' 
  AND ic.object_id = OBJECT_ID('UsoDigital')
ORDER BY i.name, ic.key_ordinal;

PRINT '';
PRINT '✓ Migración completada exitosamente';
PRINT '  Si ves "UQ_UsoDigital_Usuario_Fecha" con columnas "idUsuario" y "fechaRegistro", está correcto.';
GO
