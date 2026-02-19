CREATE TABLE Employee (
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
    Nom NVARCHAR(100) NOT NULL,
    Prenom NVARCHAR(100) NOT NULL,
    Age INT CHECK (Age >= 18),
    Sexe CHAR(1) CHECK (Sexe IN ('M', 'F')),
    NomPoste NVARCHAR(100),
    Fonctions NVARCHAR(MAX),
    NombreMoisPoste INT DEFAULT 0,
    StatutEmploye NVARCHAR(3) CHECK (StatutEmploye IN ('CDI', 'CDD'))
);