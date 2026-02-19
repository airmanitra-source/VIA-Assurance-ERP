-- Migration to rename 'entreprise' table to 'Entreprise'
-- Date: 2026-02-15

EXEC sp_rename '[documentdb].[dbo].[entreprise]', 'Entreprise';
GO

ALTER TABLE documentdb.dbo.Entreprise ADD CONSTRAINT Entreprise_PK PRIMARY KEY (Id);

ALTER TABLE Souscription
ADD CONSTRAINT FK_Souscription_Entreprise
FOREIGN KEY (EntrepriseId) REFERENCES Entreprise(Id);

ALTER TABLE Employee
ADD CONSTRAINT FK_Employee_Entreprise
FOREIGN KEY (EntrepriseID) REFERENCES Entreprise(Id);
