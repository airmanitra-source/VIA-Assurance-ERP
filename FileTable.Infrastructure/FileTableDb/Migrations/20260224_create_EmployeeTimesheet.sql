CREATE TABLE EmployeeTimesheet (
    TimesheetID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID BIGINT NOT NULL,
    ProjectID INT,
    WorkDate DATE NOT NULL,
    HoursWorked DECIMAL(5,2) NOT NULL CHECK (HoursWorked >= 0 AND HoursWorked <= 24),
    OvertimeHours DECIMAL(5,2) DEFAULT 0 CHECK (OvertimeHours >= 0),
    TaskDescription NVARCHAR(500),
    Status NVARCHAR(50) CHECK (Status IN ('Draft', 'Submitted', 'Approved', 'Rejected')) DEFAULT 'Draft',
    SubmittedDate DATETIME2,
    ApprovedByID BIGINT,
    ApprovedDate DATETIME2,
    Comments NVARCHAR(MAX),
    CreatedDate DATETIME2 DEFAULT GETDATE(),
    ModifiedDate DATETIME2,
    CONSTRAINT FK_EmployeeTimesheet_Employee FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID),
    CONSTRAINT FK_EmployeeTimesheet_Project FOREIGN KEY (ProjectID) REFERENCES Project(ProjectID),
    CONSTRAINT FK_EmployeeTimesheet_ApprovedBy FOREIGN KEY (ApprovedByID) REFERENCES Employee(EmployeeID)
);

CREATE INDEX IX_EmployeeTimesheet_EmployeeID_Date ON EmployeeTimesheet(EmployeeID, WorkDate DESC);
CREATE INDEX IX_EmployeeTimesheet_ProjectID_Date ON EmployeeTimesheet(ProjectID, WorkDate DESC);
CREATE INDEX IX_EmployeeTimesheet_Status ON EmployeeTimesheet(Status);
