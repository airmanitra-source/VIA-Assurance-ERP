-- Migration to create 'EntrepriseWarehouseMaterials' table
-- Date: 2026-02-15
use documentdb
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EntrepriseWarehouseMaterials' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [documentdb].[dbo].[EntrepriseWarehouseMaterials] (
        [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
        [WarehouseId] BIGINT NOT NULL,
        [Description] NVARCHAR(250) NOT NULL,
        [ApproximateValue] DECIMAL(18, 2) NOT NULL DEFAULT 0,
        [WantsInsurance] BIT NOT NULL DEFAULT 0,
        [IsInsured] BIT NOT NULL DEFAULT 0,
        CONSTRAINT [FK_EntrepriseWarehouseMaterials_Warehouse] FOREIGN KEY ([WarehouseId]) REFERENCES [documentdb].[dbo].[EntrepriseWarehouse]([Id]) ON DELETE CASCADE
    );
END
GO
