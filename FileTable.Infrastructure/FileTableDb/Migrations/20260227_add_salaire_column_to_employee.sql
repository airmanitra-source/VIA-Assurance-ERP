-- Ajouter le champ Salaire à la table Employee
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[documentdb].[dbo].[Employee]') 
    AND name = 'Salaire'
)
BEGIN
    ALTER TABLE [documentdb].[dbo].[Employee]
    ADD Salaire DECIMAL(18, 2) NULL;
    
    PRINT 'Colonne Salaire ajoutée à la table Employee';
END
ELSE
BEGIN
    PRINT 'La colonne Salaire existe déjà';
END
