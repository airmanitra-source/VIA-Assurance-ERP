-- Migration to add 'PolicyNumber' to 'EntrepriseFleet' table
-- Date: 2026-02-18
USE documentdb
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.EntrepriseFleet') AND name = 'PolicyNumber')
BEGIN
    ALTER TABLE [documentdb].[dbo].[EntrepriseFleet]
    ADD [PolicyNumber] NVARCHAR(50) NULL;
END
GO
