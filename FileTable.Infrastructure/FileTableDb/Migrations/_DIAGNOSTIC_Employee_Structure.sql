-- Script de DIAGNOSTIC pour afficher l'état complet de la table Employee
USE [documentdb];

PRINT '=== STRUCTURE ACTUELLE DE LA TABLE EMPLOYEE ===';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    ORDINAL_POSITION
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'dbo' 
  AND TABLE_NAME = 'Employee'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '=== CONTRAINTES ET CLÉS ÉTRANGÈRES ===';
SELECT 
    tc.CONSTRAINT_TYPE,
    tc.CONSTRAINT_NAME,
    kcu.COLUMN_NAME,
    kcu.TABLE_NAME,
    rc.MATCH_OPTION
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu 
    ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
LEFT JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc 
    ON tc.CONSTRAINT_NAME = rc.CONSTRAINT_NAME
WHERE tc.TABLE_NAME = 'Employee' AND tc.TABLE_SCHEMA = 'dbo'
ORDER BY tc.CONSTRAINT_TYPE, tc.CONSTRAINT_NAME;

PRINT '';
PRINT '=== VÉRIFIER LES COLONNES REQUISES ===';
DECLARE @requiredColumns TABLE (ColumnName NVARCHAR(128));

INSERT INTO @requiredColumns VALUES 
    ('EmployeeID'),
    ('Nom'),
    ('Prenom'),
    ('Age'),
    ('Sexe'),
    ('NomPoste'),
    ('Fonctions'),
    ('NombreMoisPoste'),
    ('StatutEmploye'),
    ('EntrepriseID'),
    ('IsActive'),
    ('NumeroMatricule'),
    ('DateEmbauche'),
    ('DateFinContrat'),
    ('Email'),
    ('VouloirSouscrire'),
    ('Salaire');

SELECT 
    rc.ColumnName,
    CASE 
        WHEN c.COLUMN_NAME IS NOT NULL THEN 'EXISTE'
        ELSE 'MANQUANTE'
    END AS Status
FROM @requiredColumns rc
LEFT JOIN INFORMATION_SCHEMA.COLUMNS c 
    ON c.TABLE_SCHEMA = 'dbo' 
    AND c.TABLE_NAME = 'Employee' 
    AND c.COLUMN_NAME = rc.ColumnName
ORDER BY rc.ColumnName;

PRINT '';
PRINT '=== PREMIER ENREGISTREMENT (si existe) ===';
IF EXISTS (SELECT 1 FROM [documentdb].[dbo].[Employee] LIMIT 1)
BEGIN
    SELECT TOP 1 * FROM [documentdb].[dbo].[Employee];
END
ELSE
BEGIN
    PRINT 'Aucun enregistrement Employee trouvé';
END
