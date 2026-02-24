-- Migration to create SinisterType reference table
-- Date: 2026-02-26
-- Purpose: Store sinister type reference data (accident, incendie, inondation, etc.)

USE documentdb;
GO

-- Create SinisterType table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SinisterType' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [documentdb].[dbo].[SinisterType] (
        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [TypeName] NVARCHAR(100) NOT NULL UNIQUE
    );

    PRINT 'Table SinisterType created successfully.';
END
ELSE
BEGIN
    PRINT 'Table SinisterType already exists.';
END
GO
