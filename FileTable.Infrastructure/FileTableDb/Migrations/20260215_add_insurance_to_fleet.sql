-- Migration to add missing insurance columns to 'EntrepriseFleet' table
-- Date: 2026-02-15
USE documentdb;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EntrepriseFleet' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[documentdb].[dbo].[EntrepriseFleet]') AND name = 'WantsInsurance')
    BEGIN
        ALTER TABLE [documentdb].[dbo].[EntrepriseFleet] ADD [WantsInsurance] BIT NOT NULL DEFAULT 0;
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[documentdb].[dbo].[EntrepriseFleet]') AND name = 'IsInsured')
    BEGIN
        ALTER TABLE [documentdb].[dbo].[EntrepriseFleet] ADD [IsInsured] BIT NOT NULL DEFAULT 0;
    END
END
GO
