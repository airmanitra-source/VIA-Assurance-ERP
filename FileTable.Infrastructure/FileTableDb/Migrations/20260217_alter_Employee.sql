use documentdb;
BEGIN TRANSACTION;

BEGIN TRY
    -- 1. Créer la table temporaire avec le bon type
    CREATE TABLE Employee_New (
        EmployeeID BIGINT IDENTITY(1,1) NOT NULL,
        Nom NVARCHAR(100) NOT NULL,
        Prenom NVARCHAR(100) NOT NULL,
        Age INT CHECK (Age >= 18),
        Sexe CHAR(1) CHECK (Sexe IN ('M', 'F')),
        NomPoste NVARCHAR(100),
        Fonctions NVARCHAR(MAX),
        DateFinContrat DATETIME NULL,
        NombreMoisPoste INT DEFAULT 0,
        NumeroMatricule NVARCHAR(50),
        StatutEmploye NVARCHAR(3) CHECK (StatutEmploye IN ('CDI', 'CDD')),
        IsActive BIT Default 1,
        VouloirSouscrire BIT NOT NULL DEFAULT 0,
        EntrepriseID bigint not null
     CONSTRAINT FK_Entreprise FOREIGN KEY (EntrepriseID) 
        REFERENCES Entreprise(Id)
    );

    -- 2. Permettre l'insertion manuelle dans la colonne IDENTITY
    SET IDENTITY_INSERT Employee_New ON;

    -- 3. Transférer les données (listez toutes vos colonnes ici aussi)
    INSERT INTO Employee_New (EmployeeID, Nom, Prenom, Age, Sexe, NomPoste, Fonctions, NombreMoisPoste, StatutEmploye, EntrepriseID, IsActive, NumeroMatricule, DateFinContrat, VouloirSouscrire)
    SELECT EmployeeID, Nom, Prenom, Age, Sexe, NomPoste, Fonctions, NombreMoisPoste, StatutEmploye, EntrepriseID, IsActive, NumeroMatricule, DateFinContrat, VouloirSouscrire
    FROM Employee;

    SET IDENTITY_INSERT Employee_New OFF;

    -- 4. Supprimer l'ancienne table
    -- Note: Si vous avez des clés étrangères, il faut les DROP avant cette ligne
    ALTER TABLE EmployeeDocuments DROP CONSTRAINT FK_Employee;
    ALTER TABLE Souscription DROP CONSTRAINT FK_Souscription_Employee;
    ALTER TABLE EmployeeDocuments DROP CONSTRAINT PK__Employee__3214EC07B6A1C02D;

    -- MODIFIER le type des colonnes au lieu de DROP/ADD
    ALTER TABLE EmployeeDocuments ALTER COLUMN EmployeeID BIGINT NOT NULL;
    ALTER TABLE Souscription ALTER COLUMN EmployeeId BIGINT NOT NULL;
    DROP TABLE Employee;

    -- 5. Renommer la nouvelle table
    EXEC sp_rename 'Employee_New', 'Employee';
    --6. Rajouter les FK avec la nouvelle table:
    ALTER TABLE EmployeeDocuments ADD CONSTRAINT FK_Employee FOREIGN KEY (EmployeeID) 
     REFERENCES Employee(EmployeeID);
    ALTER TABLE Souscription ADD CONSTRAINT FK_Souscription_Employee FOREIGN KEY (EmployeeId) 
    REFERENCES Employee(EmployeeID);
    -- Si tout est OK, on valide
    COMMIT TRANSACTION;
    PRINT 'Migration vers BIGINT réussie !';
END TRY
BEGIN CATCH
    -- En cas d'erreur, on annule tout
    ROLLBACK TRANSACTION;
    PRINT 'Erreur détectée, modifications annulées.';
    PRINT ERROR_MESSAGE();
END CATCH
