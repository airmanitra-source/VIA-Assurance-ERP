IF NOT EXISTS (
    SELECT 1 FROM sys.objects
    WHERE object_id = OBJECT_ID(N'[documentdb].[dbo].[PaySlipModificationRequest]')
    AND type = 'U'
)
BEGIN
    CREATE TABLE [documentdb].[dbo].[PaySlipModificationRequest]
    (
        RequestID       INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeID      BIGINT NOT NULL,
        PeriodID        INT NOT NULL,
        Bonus           DECIMAL(18,2) NULL,
        IndemniteLogement DECIMAL(18,2) NULL,
        IndemniteTransport DECIMAL(18,2) NULL,
        OvertimeHours   DECIMAL(18,2) NULL,
        PrimeScolarite  DECIMAL(18,2) NULL,
        TreiziemeMois   DECIMAL(18,2) NULL,
        Comments        NVARCHAR(500) NULL,
        Status          NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        CreatedDate     DATETIME NOT NULL DEFAULT GETDATE(),
        ReviewedDate    DATETIME NULL
    );
    PRINT 'Table PaySlipModificationRequest créée';
END
