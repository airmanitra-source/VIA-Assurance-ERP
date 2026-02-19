USE [documentdb]
GO

ALTER TABLE dbo.entreprise 
    ADD RaisonSocial NVARCHAR(MAX),
        StatutJuridique  NVARCHAR(10),
        Taille  int,
        NombreEmployes  int,
        Adresse NVARCHAR(MAX),
        AnneeCreation datetime,
        NifStat NVARCHAR(256)



