USE documentdb;
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CompanySinisterDocuments' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [documentdb].[dbo].[CompanySinisterDocuments] (
        [CompanySinisterID] BIGINT NOT NULL,
        [EntrepriseID] BIGINT NOT NULL,
        [FileStreamID] UNIQUEIDENTIFIER NOT NULL,
        [TypeDocument] NVARCHAR(50) NULL,

        CONSTRAINT [FK_CompanySinisterDocuments_Sinister] FOREIGN KEY ([CompanySinisterID])
            REFERENCES [documentdb].[dbo].[EntrepriseSinister]([Id]) ON DELETE CASCADE,

        CONSTRAINT [FK_CompanySinisterDocuments_Entreprise] FOREIGN KEY ([EntrepriseID])
            REFERENCES [documentdb].[dbo].[Entreprise]([Id]) ON DELETE NO ACTION,

        CONSTRAINT [FK_CompanySinisterDocuments_FileTable] FOREIGN KEY ([FileStreamID])
            REFERENCES [documentdb].[dbo].[Documents]([stream_id]),

        CONSTRAINT [PK_CompanySinisterDocuments] PRIMARY KEY ([CompanySinisterID], [FileStreamID])
    );

    CREATE INDEX [IX_CompanySinisterDocuments_EntrepriseId] ON [documentdb].[dbo].[CompanySinisterDocuments]([EntrepriseID]);
END
GO
