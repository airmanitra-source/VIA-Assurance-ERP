-- Migration to create 'EntrepriseMerchandiseTransportation' table
-- Date: 2026-02-15
use documentdb
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EntrepriseMerchandiseTransportation' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [documentdb].[dbo].[EntrepriseMerchandiseTransportation] (
        [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
        [EntrepriseId] BIGINT NOT NULL,
        [Description] NVARCHAR(250) NOT NULL,
        [Value] DECIMAL(18, 2) NOT NULL,
        [DepartureDate] DATETIME2 NOT NULL,
        [ArrivalDate] DATETIME2 NOT NULL,
        [Origin] NVARCHAR(100) NOT NULL,
        [Destination] NVARCHAR(100) NOT NULL,
        [WantsInsurance] BIT NOT NULL DEFAULT 0,
        [IsInsured] BIT NOT NULL DEFAULT 0,
        CONSTRAINT [FK_EntrepriseMerchandiseTransportation_Entreprise] FOREIGN KEY ([EntrepriseId]) REFERENCES [documentdb].[dbo].[Entreprise]([Id]) ON DELETE CASCADE
    );
END
