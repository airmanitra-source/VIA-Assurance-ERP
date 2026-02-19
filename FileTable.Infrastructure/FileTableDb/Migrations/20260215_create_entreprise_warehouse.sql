-- Migration to create 'EntrepriseWarehouse' table
-- Date: 2026-02-15
use documentdb
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EntrepriseWarehouse' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [documentdb].[dbo].[EntrepriseWarehouse] (
        [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
        [EntrepriseId] BIGINT NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [SizeM2] DECIMAL(18, 2) NOT NULL,
        [ContentsDescription] NVARCHAR(MAX) NULL,
        [Address] NVARCHAR(250) NULL,
        [WantsInsurance] BIT NOT NULL DEFAULT 0,
        [IsInsured] BIT NOT NULL DEFAULT 0,
        CONSTRAINT [FK_EntrepriseWarehouse_Entreprise] FOREIGN KEY ([EntrepriseId]) REFERENCES [documentdb].[dbo].[Entreprise]([Id]) ON DELETE CASCADE
    );
END
GO
