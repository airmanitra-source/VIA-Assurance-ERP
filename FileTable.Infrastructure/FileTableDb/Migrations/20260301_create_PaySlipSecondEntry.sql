-- Table de stockage de la deuxième saisie des éléments de paie (double saisie)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[documentdb].[dbo].[PaySlipSecondEntry]') AND type = 'U')
BEGIN
    CREATE TABLE [documentdb].[dbo].[PaySlipSecondEntry] (
        SecondEntryID INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeID BIGINT NOT NULL,
        PeriodID INT NOT NULL,
        Bonus DECIMAL(18,2),
        CreatedDate DATETIME2 DEFAULT GETDATE(),
        IndemniteLogement DECIMAL(18,2),
        IndemniteTransport DECIMAL(18,2),
        OvertimeHours DECIMAL(18,2),
        PrimeScolarite DECIMAL(18,2),
        TreiziemeMois DECIMAL(18,2),
        CONSTRAINT UQ_PaySlipSecondEntry_Employee_Period UNIQUE (EmployeeID, PeriodID)
    );
    PRINT 'Table PaySlipSecondEntry créée';
END
