-- Migration to create '[EntrepriseSinister]' table
-- Date: 2026-02-15
-- Purpose: Track sinister (insurance claims/incidents) for assets (fleets, transportation, and warehouses)

use documentdb
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '[EntrepriseSinister]' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [documentdb].[dbo].[EntrepriseSinister] (
        [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
        [EntrepriseId] BIGINT NOT NULL,
        [SinisterDate] DATETIME2 NOT NULL,
        [Description] NVARCHAR(MAX) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- 'Pending', 'InProgress', 'Resolved', 'Rejected'
        [EstimatedAmount] DECIMAL(18, 2) NOT NULL,
        [ResolvedAmount] DECIMAL(18, 2) NULL,
        [AssetType] NVARCHAR(50) NOT NULL, -- 'Fleet', 'Transportation', 'Warehouse'
        [EntrepriseFleetId] BIGINT NULL,
        [EntrepriseMerchandiseTransportationId] BIGINT NULL,
        [EntrepriseWarehouseId] BIGINT NULL,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [LastModifiedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_CompanySinister_Entreprise] FOREIGN KEY ([EntrepriseId]) REFERENCES [documentdb].[dbo].[Entreprise]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CompanySinister_EntrepriseFleet] FOREIGN KEY ([EntrepriseFleetId]) REFERENCES [documentdb].[dbo].[EntrepriseFleet]([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_CompanySinister_EntrepriseMerchandiseTransportation] FOREIGN KEY ([EntrepriseMerchandiseTransportationId]) REFERENCES [documentdb].[dbo].[EntrepriseMerchandiseTransportation]([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_CompanySinister_EntrepriseWarehouse] FOREIGN KEY ([EntrepriseWarehouseId]) REFERENCES [documentdb].[dbo].[EntrepriseWarehouse]([Id]) ON DELETE SET NULL,
        -- Constraint to ensure only one asset is referenced per sinister
        CONSTRAINT [CK_CompanySinister_SingleAsset] CHECK (
            (CASE WHEN [EntrepriseFleetId] IS NOT NULL THEN 1 ELSE 0 END) +
            (CASE WHEN [EntrepriseMerchandiseTransportationId] IS NOT NULL THEN 1 ELSE 0 END) +
            (CASE WHEN [EntrepriseWarehouseId] IS NOT NULL THEN 1 ELSE 0 END) = 1
        )
    );
    
    -- Create indexes for better query performance
    CREATE INDEX [IX_EntrepriseSinister_EntrepriseId] ON [documentdb].[dbo].EntrepriseSinister([EntrepriseId]);
    CREATE INDEX [IX_EntrepriseSinister_SinisterDate] ON [documentdb].[dbo].EntrepriseSinister([SinisterDate]);
    CREATE INDEX [IX_EntrepriseSinister_Status] ON [documentdb].[dbo].EntrepriseSinister([Status]);
    CREATE INDEX [IX_EntrepriseSinister_AssetType] ON [documentdb].[dbo].EntrepriseSinister([AssetType]);
END
GO
