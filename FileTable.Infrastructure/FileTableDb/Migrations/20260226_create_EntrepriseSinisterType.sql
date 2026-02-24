-- Migration to create EntrepriseSinisterType junction table
-- Date: 2026-02-26
-- Purpose: Many-to-many relationship between EntrepriseSinister and SinisterType

USE documentdb;
GO

-- Create EntrepriseSinisterType junction table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EntrepriseSinisterType' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [documentdb].[dbo].[EntrepriseSinisterType] (
        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [EntrepriseSinisterId] BIGINT NOT NULL,
        [SinisterTypeId] BIGINT NOT NULL,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_EntrepriseSinisterType_EntrepriseSinister] 
            FOREIGN KEY ([EntrepriseSinisterId]) 
            REFERENCES [documentdb].[dbo].[EntrepriseSinister]([Id]) 
            ON DELETE CASCADE,
        CONSTRAINT [FK_EntrepriseSinisterType_SinisterType] 
            FOREIGN KEY ([SinisterTypeId]) 
            REFERENCES [documentdb].[dbo].[SinisterType]([Id]) 
            ON DELETE CASCADE,
        -- Ensure unique combination of sinister and type
        CONSTRAINT [UQ_EntrepriseSinisterType_Sinister_Type] 
            UNIQUE ([EntrepriseSinisterId], [SinisterTypeId])
    );

    -- Create indexes
    CREATE INDEX [IX_EntrepriseSinisterType_EntrepriseSinisterId] 
        ON [documentdb].[dbo].[EntrepriseSinisterType]([EntrepriseSinisterId]);
    
    CREATE INDEX [IX_EntrepriseSinisterType_SinisterTypeId] 
        ON [documentdb].[dbo].[EntrepriseSinisterType]([SinisterTypeId]);

    PRINT 'Table EntrepriseSinisterType created successfully.';
END
ELSE
BEGIN
    PRINT 'Table EntrepriseSinisterType already exists.';
END
GO
