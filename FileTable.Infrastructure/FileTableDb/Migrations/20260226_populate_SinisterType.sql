-- Migration to populate SinisterType with reference data
-- Date: 2026-02-26
-- Purpose: Insert standard sinister types

USE documentdb;
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'SinisterType' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    -- Insert sinister types if not already present
    INSERT INTO [documentdb].[dbo].[SinisterType] ([TypeName])
    SELECT TypeName FROM (VALUES
        (N'Accident'),
        (N'Incendie'),
        (N'Inondation'),
        (N'Vol'),
        (N'Vandalisme'),
        (N'Catastrophe naturelle'),
        (N'Dégât des eaux'),
        (N'Bris de glace'),
        (N'Collision'),
        (N'Tempête'),
        (N'Grêle'),
        (N'Explosion')
    ) AS Types(TypeName)
    WHERE TypeName NOT IN (SELECT TypeName FROM [documentdb].[dbo].[SinisterType]);

    PRINT 'SinisterType table populated successfully.';
END
ELSE
BEGIN
    PRINT 'Table SinisterType does not exist.';
END
GO
