-- Migration to add Insurance Period to EntrepriseWarehouse
-- Date: 2026-02-20
use documentdb

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EntrepriseWarehouse' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'InsuranceStartDate' AND Object_ID = Object_ID('dbo.EntrepriseWarehouse'))
    BEGIN
        ALTER TABLE [documentdb].[dbo].[EntrepriseWarehouse] ADD [InsuranceStartDate] DATETIME2 NULL;
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'InsuranceEndDate' AND Object_ID = Object_ID('dbo.EntrepriseWarehouse'))
    BEGIN
        ALTER TABLE [documentdb].[dbo].[EntrepriseWarehouse] ADD [InsuranceEndDate] DATETIME2 NULL;
    END
END
GO
