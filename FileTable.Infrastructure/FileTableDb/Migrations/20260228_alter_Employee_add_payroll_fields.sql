-- Ajouter le champ Dependents (personnes à charge) à la table Employee pour le calcul IRSA
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[documentdb].[dbo].[Employee]') 
    AND name = 'Dependents'
)
BEGIN
    ALTER TABLE [documentdb].[dbo].[Employee]
    ADD Dependents INT NOT NULL DEFAULT 0;
    PRINT 'Colonne Dependents ajoutée à Employee';
END

-- Ajouter le champ NumeroCnaps à la table Employee
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[documentdb].[dbo].[Employee]') 
    AND name = 'NumeroCnaps'
)
BEGIN
    ALTER TABLE [documentdb].[dbo].[Employee]
    ADD NumeroCnaps NVARCHAR(50) NULL;
    PRINT 'Colonne NumeroCnaps ajoutée à Employee';
END

-- Ajouter Classification à Employee
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[documentdb].[dbo].[Employee]') 
    AND name = 'Classification'
)
BEGIN
    ALTER TABLE [documentdb].[dbo].[Employee]
    ADD Classification NVARCHAR(50) NULL;
    PRINT 'Colonne Classification ajoutée à Employee';
END

-- Ajouter le numéro de compte bancaire
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[documentdb].[dbo].[Employee]') 
    AND name = 'BankAccountNumber'
)
BEGIN
    ALTER TABLE [documentdb].[dbo].[Employee]
    ADD BankAccountNumber NVARCHAR(50) NULL;
    PRINT 'Colonne BankAccountNumber ajoutée à Employee';
END
