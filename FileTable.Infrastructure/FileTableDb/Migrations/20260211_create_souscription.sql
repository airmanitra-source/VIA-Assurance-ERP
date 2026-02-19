CREATE TABLE Souscription (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    EntrepriseId BIGINT NOT NULL,
    MoisDeCotisation INT NOT NULL,
    MontantCotisation DECIMAL(18, 2) NOT NULL,    
    CONSTRAINT CK_MoisDeCotisation CHECK (MoisDeCotisation BETWEEN 1 AND 12),
    CONSTRAINT FK_Souscription_Employee FOREIGN KEY (EmployeeId) 
        REFERENCES Employee(EmployeeID),
    CONSTRAINT FK_Souscription_Entreprise FOREIGN KEY (EntrepriseId) 
        REFERENCES entreprise(Id)
);


CREATE NONCLUSTERED INDEX IX_Souscription_IdentifiantEmploye 
    ON Souscription(EmployeeId);

CREATE NONCLUSTERED INDEX IX_Souscription_IdentifiantEntreprise 
    ON Souscription(EntrepriseId);