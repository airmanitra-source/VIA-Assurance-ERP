-- Migration to add Insurance Period to EntrepriseMerchandiseTransportation
-- Date: 2026-02-20
use documentdb

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EntrepriseMerchandiseTransportation' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'InsuranceStartDate' AND Object_ID = Object_ID('dbo.EntrepriseMerchandiseTransportation'))
    BEGIN
        ALTER TABLE [documentdb].[dbo].[EntrepriseMerchandiseTransportation] ADD [InsuranceStartDate] DATETIME2 NULL;
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'InsuranceEndDate' AND Object_ID = Object_ID('dbo.EntrepriseMerchandiseTransportation'))
    BEGIN
        ALTER TABLE [documentdb].[dbo].[EntrepriseMerchandiseTransportation] ADD [InsuranceEndDate] DATETIME2 NULL;
    END
END
GO
