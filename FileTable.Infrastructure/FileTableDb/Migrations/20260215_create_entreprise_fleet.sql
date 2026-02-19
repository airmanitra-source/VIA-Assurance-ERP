-- Migration to create 'EntrepriseFleet' table
-- Date: 2026-02-15
use  documentdb
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EntrepriseFleet' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [documentdb].[dbo].[EntrepriseFleet] (
        [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
        [EntrepriseId] BIGINT NOT NULL,
        [Type] NVARCHAR(50) NOT NULL, -- 'Auto' or 'Moto'
        [Year] INT NOT NULL,
        [IsWorking] BIT NOT NULL DEFAULT 1,
        [Mileage] INT NOT NULL DEFAULT 0,
        [Make] NVARCHAR(100) NOT NULL,
        [Model] NVARCHAR(100) NOT NULL,
        [WantsInsurance] BIT NOT NULL DEFAULT 0,
        [IsInsured] BIT NOT NULL DEFAULT 0,
        CONSTRAINT [FK_EntrepriseFleet_Entreprise] FOREIGN KEY ([EntrepriseId]) REFERENCES documentdb.dbo.Entreprise(Id) ON DELETE CASCADE
    );
END

