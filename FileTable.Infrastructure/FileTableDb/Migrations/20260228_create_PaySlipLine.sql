-- Table des lignes du bulletin de paie
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[documentdb].[dbo].[PaySlipLine]') AND type = 'U')
BEGIN
    CREATE TABLE [documentdb].[dbo].[PaySlipLine] (
        LineID BIGINT IDENTITY(1,1) PRIMARY KEY,
        PayrollID INT NOT NULL,
        PeriodID INT NOT NULL,
        EmployeeID BIGINT NOT NULL,
        Rubrique NVARCHAR(10) NOT NULL,
        Libelle NVARCHAR(200) NOT NULL,
        LineType NVARCHAR(20) NOT NULL CHECK (LineType IN ('Gain', 'Cotisation', 'Impot')),
        Nombre DECIMAL(10,2) NULL,
        Base DECIMAL(18,2) NULL,
        Taux DECIMAL(10,4) NULL,
        GainAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        EmployeeDeduction DECIMAL(18,2) NOT NULL DEFAULT 0,
        EmployerContribution DECIMAL(18,2) NOT NULL DEFAULT 0,
        SortOrder INT NOT NULL DEFAULT 0,
        CreatedDate DATETIME2 DEFAULT GETDATE(),
        CONSTRAINT FK_PaySlipLine_Payroll FOREIGN KEY (PayrollID) REFERENCES EmployeePayroll(PayrollID),
        CONSTRAINT FK_PaySlipLine_Period FOREIGN KEY (PeriodID) REFERENCES PayrollPeriod(PeriodID),
        CONSTRAINT FK_PaySlipLine_Employee FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID)
    );

    CREATE INDEX IX_PaySlipLine_PayrollID ON [documentdb].[dbo].[PaySlipLine](PayrollID);
    CREATE INDEX IX_PaySlipLine_PeriodEmployee ON [documentdb].[dbo].[PaySlipLine](PeriodID, EmployeeID);
    PRINT 'Table PaySlipLine créée';
END
