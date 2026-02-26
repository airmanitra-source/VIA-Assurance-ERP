-- Table de configuration de la paie par entreprise
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[documentdb].[dbo].[CompanyPayrollSettings]') AND type = 'U')
BEGIN
    CREATE TABLE [documentdb].[dbo].[CompanyPayrollSettings] (
        SettingsID INT IDENTITY(1,1) PRIMARY KEY,
        EntrepriseID BIGINT NOT NULL,
        CnapsEmployeeRate DECIMAL(5,4) NOT NULL DEFAULT 0.01,
        CnapsComplementaryRate DECIMAL(5,4) NOT NULL DEFAULT 0.01,
        OstieEmployeeRate DECIMAL(5,4) NOT NULL DEFAULT 0.01,
        CnapsEmployerRate DECIMAL(5,4) NOT NULL DEFAULT 0.13,
        CnapsComplementaryEmployerRate DECIMAL(5,4) NOT NULL DEFAULT 0.035,
        OstieEmployerRate DECIMAL(5,4) NOT NULL DEFAULT 0.05,
        MaxOvertimeHoursPerWeek INT NOT NULL DEFAULT 20,
        OvertimeRateMultiplier DECIMAL(5,2) NOT NULL DEFAULT 1.5,
        IrsaMinimum DECIMAL(18,2) NOT NULL DEFAULT 3000,
        IrsaDependentReduction DECIMAL(18,2) NOT NULL DEFAULT 2000,
        CreatedDate DATETIME2 DEFAULT GETDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_CompanyPayrollSettings_Entreprise FOREIGN KEY (EntrepriseID) REFERENCES Entreprise(Id),
        CONSTRAINT UQ_CompanyPayrollSettings_Entreprise UNIQUE (EntrepriseID)
    );
    PRINT 'Table CompanyPayrollSettings créée';
END
