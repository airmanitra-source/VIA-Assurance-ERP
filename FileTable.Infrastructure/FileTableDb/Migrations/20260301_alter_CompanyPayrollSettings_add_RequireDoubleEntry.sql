-- Ajout du champ RequireDoubleEntry à CompanyPayrollSettings
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[documentdb].[dbo].[CompanyPayrollSettings]') AND name = 'RequireDoubleEntry')
BEGIN
    ALTER TABLE [documentdb].[dbo].[CompanyPayrollSettings]
    ADD RequireDoubleEntry BIT NOT NULL DEFAULT 0;
    PRINT 'Colonne RequireDoubleEntry ajoutée à CompanyPayrollSettings';
END
