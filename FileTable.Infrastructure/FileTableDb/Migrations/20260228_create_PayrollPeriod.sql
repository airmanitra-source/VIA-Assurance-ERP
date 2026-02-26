-- Table des périodes de paie
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[documentdb].[dbo].[PayrollPeriod]') AND type = 'U')
BEGIN
    CREATE TABLE [documentdb].[dbo].[PayrollPeriod] (
        PeriodID INT IDENTITY(1,1) PRIMARY KEY,
        EntrepriseID BIGINT NOT NULL,
        PeriodStart DATE NOT NULL,
        PeriodEnd DATE NOT NULL,
        PaymentDate DATE NULL,
        Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Draft', 'InProgress', 'Validated', 'Paid')) DEFAULT 'Draft',
        CreatedDate DATETIME2 DEFAULT GETDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_PayrollPeriod_Entreprise FOREIGN KEY (EntrepriseID) REFERENCES Entreprise(Id),
        CONSTRAINT UQ_PayrollPeriod UNIQUE (EntrepriseID, PeriodStart, PeriodEnd)
    );
    PRINT 'Table PayrollPeriod créée';
END
