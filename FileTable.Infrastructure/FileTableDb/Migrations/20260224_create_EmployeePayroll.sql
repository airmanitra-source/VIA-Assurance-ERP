CREATE TABLE EmployeePayroll (
    PayrollID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID BIGINT NOT NULL,
    PayPeriodMonth INT NOT NULL CHECK (PayPeriodMonth BETWEEN 1 AND 12),
    PayPeriodYear INT NOT NULL CHECK (PayPeriodYear >= 2000),
    BaseSalary DECIMAL(18,2) NOT NULL,
    Bonus DECIMAL(18,2) DEFAULT 0,
    Deductions DECIMAL(18,2) DEFAULT 0,
    NetSalary AS (BaseSalary + Bonus - Deductions) PERSISTED,
    PaymentDate DATE NOT NULL,
    PaymentMethod NVARCHAR(50) CHECK (PaymentMethod IN ('BankTransfer', 'Check', 'Cash')) DEFAULT 'BankTransfer',
    Notes NVARCHAR(MAX),
    CreatedDate DATETIME2 DEFAULT GETDATE(),
    ModifiedDate DATETIME2,
    CONSTRAINT FK_EmployeePayroll_Employee FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID),
    CONSTRAINT UQ_EmployeePayroll UNIQUE (EmployeeID, PayPeriodMonth, PayPeriodYear)
);

CREATE INDEX IX_EmployeePayroll_EmployeeID_Date ON EmployeePayroll(EmployeeID, PayPeriodYear DESC, PayPeriodMonth DESC);
