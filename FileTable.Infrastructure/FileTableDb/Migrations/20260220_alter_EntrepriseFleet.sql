-- Migration to add Power, FiscalPower and Insurance Period to EntrepriseFleet
-- Date: 2026-02-20
use documentdb

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EntrepriseFleet' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'Power' AND Object_ID = Object_ID('dbo.EntrepriseFleet'))
    BEGIN
        ALTER TABLE [documentdb].[dbo].[EntrepriseFleet] ADD [Power] INT NULL;
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'FiscalPower' AND Object_ID = Object_ID('dbo.EntrepriseFleet'))
    BEGIN
        ALTER TABLE [documentdb].[dbo].[EntrepriseFleet] ADD [FiscalPower] INT NULL;
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'InsuranceStartDate' AND Object_ID = Object_ID('dbo.EntrepriseFleet'))
    BEGIN
        ALTER TABLE [documentdb].[dbo].[EntrepriseFleet] ADD [InsuranceStartDate] DATETIME2 NULL;
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'InsuranceEndDate' AND Object_ID = Object_ID('dbo.EntrepriseFleet'))
    BEGIN
        ALTER TABLE [documentdb].[dbo].[EntrepriseFleet] ADD [InsuranceEndDate] DATETIME2 NULL;
    END
END
GO
