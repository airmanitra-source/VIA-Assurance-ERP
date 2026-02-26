use documentdb;
BEGIN TRANSACTION;

BEGIN TRY
    -- 1. Créer la table temporaire avec le bon type
    CREATE TABLE Employee_New (
        EmployeeID BIGINT IDENTITY(1,1) NOT NULL,
        Age INT CHECK (Age >= 18),
        DateEmbauche DATE NULL,
        DateFinContrat DATETIME NULL,
        Email NVARCHAR(255),
        EntrepriseID BIGINT NOT NULL,
        Fonctions NVARCHAR(MAX),
        IsActive BIT Default 1,
        Nom NVARCHAR(100) NOT NULL,
        NombreMoisPoste INT DEFAULT 0,
        NomPoste NVARCHAR(100),
        NumeroMatricule NVARCHAR(50),
        Prenom NVARCHAR(100) NOT NULL,
        Salaire DECIMAL(18, 2) NULL,
        Sexe CHAR(1) CHECK (Sexe IN ('M', 'F')),
        StatutEmploye NVARCHAR(3) CHECK (StatutEmploye IN ('CDI', 'CDD')),
        VouloirSouscrire BIT NOT NULL DEFAULT 0,
        PRIMARY KEY (EmployeeID),
        CONSTRAINT FK_Entreprise FOREIGN KEY (EntrepriseID) 
            REFERENCES Entreprise(Id)
    );

    -- 2. Permettre l'insertion manuelle dans la colonne IDENTITY
    SET IDENTITY_INSERT Employee_New ON;

    -- 3. Transférer les données (listez toutes vos colonnes ici aussi)
    INSERT INTO Employee_New (EmployeeID, Age, DateEmbauche, DateFinContrat, Email, EntrepriseID, Fonctions, IsActive, Nom, NombreMoisPoste, NomPoste, NumeroMatricule, Prenom, Salaire, Sexe, StatutEmploye, VouloirSouscrire)
    SELECT EmployeeID, Age, ISNULL(DateEmbauche, NULL), DateFinContrat, ISNULL(Email, NULL), EntrepriseID, ISNULL(Fonctions, NULL), IsActive, Nom, NombreMoisPoste, ISNULL(NomPoste, NULL), ISNULL(NumeroMatricule, NULL), Prenom, ISNULL(Salaire, NULL), Sexe, StatutEmploye, VouloirSouscrire
    FROM Employee;

    SET IDENTITY_INSERT Employee_New OFF;

    -- 4. Supprimer l'ancienne table et ses contraintes
    ALTER TABLE EmployeeDocuments DROP CONSTRAINT IF EXISTS FK_Employee;
    ALTER TABLE Souscription DROP CONSTRAINT IF EXISTS FK_Souscription_Employee;
    ALTER TABLE EmployeeProject DROP CONSTRAINT IF EXISTS FK_EmployeeProject_Employee;

    ALTER TABLE EmployeeDocuments ALTER COLUMN EmployeeID BIGINT NOT NULL;
    ALTER TABLE Souscription ALTER COLUMN EmployeeId BIGINT NOT NULL;
    
    DROP TABLE Employee;

    -- 5. Renommer la nouvelle table
    EXEC sp_rename 'Employee_New', 'Employee';
    
    -- 6. Rajouter les clés étrangères avec la nouvelle table
    ALTER TABLE EmployeeDocuments ADD CONSTRAINT FK_Employee FOREIGN KEY (EmployeeID) 
        REFERENCES Employee(EmployeeID);
    
    ALTER TABLE Souscription ADD CONSTRAINT FK_Souscription_Employee FOREIGN KEY (EmployeeId) 
        REFERENCES Employee(EmployeeID);
    
    ALTER TABLE EmployeeProject ADD CONSTRAINT FK_EmployeeProject_Employee FOREIGN KEY (EmployeeID) 
        REFERENCES Employee(EmployeeID);

    COMMIT TRANSACTION;
    PRINT 'Migration Employee complétée avec succès';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Erreur lors de la migration: ' + ERROR_MESSAGE();
    THROW;
END CATCH
