-- Migration to add SinisterId FK to '[EntrepriseSinister]' table
-- Date: 2026-02-16
-- Purpose: Link company claim records to insurance policy metadata stored in [Sinister]

USE documentdb;
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EntrepriseSinister' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'SinisterId' AND Object_ID = Object_ID(N'[documentdb].[dbo].EntrepriseSinister'))
    BEGIN
        ALTER TABLE [documentdb].[dbo].[EntrepriseSinister]
        ADD [SinisterId] BIGINT NULL;

        -- Add foreign key to Sinister table (policy metadata)
        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Sinister' AND schema_id = SCHEMA_ID('dbo'))
        BEGIN
            ALTER TABLE [documentdb].[dbo].[EntrepriseSinister]
            ADD CONSTRAINT [FK_EntrepriseSinister_SinisterPolicy] FOREIGN KEY ([SinisterId]) REFERENCES [documentdb].[dbo].[Sinister]([Id]) ON DELETE SET NULL;
        END

        CREATE INDEX [IX_EntrepriseSinister_SinisterId] ON [documentdb].[dbo].EntrepriseSinister([SinisterId]);
    END
END
GO
