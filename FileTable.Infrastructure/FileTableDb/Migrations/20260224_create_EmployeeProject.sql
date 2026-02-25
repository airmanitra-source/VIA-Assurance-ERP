CREATE TABLE EmployeeProject (
    EmployeeProjectID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID BIGINT NOT NULL,
    ProjectID INT NOT NULL,
    Role NVARCHAR(100),
    AssignedDate DATE NOT NULL DEFAULT GETDATE(),
    EndDate DATE,
    IsActive BIT DEFAULT 1,
    HourlyRate DECIMAL(18,2),
    CreatedDate DATETIME2 DEFAULT GETDATE(),
    ModifiedDate DATETIME2,
    CONSTRAINT FK_EmployeeProject_Employee FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID),
    CONSTRAINT FK_EmployeeProject_Project FOREIGN KEY (ProjectID) REFERENCES Project(ProjectID),
    CONSTRAINT UQ_EmployeeProject UNIQUE (EmployeeID, ProjectID, AssignedDate)
);
