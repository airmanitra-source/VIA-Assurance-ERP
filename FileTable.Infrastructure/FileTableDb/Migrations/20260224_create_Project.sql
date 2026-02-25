CREATE TABLE Project (
    ProjectID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    StartDate DATE NOT NULL,
    EndDate DATE,
    Status NVARCHAR(50) CHECK (Status IN ('Active', 'Completed', 'OnHold', 'Cancelled')) DEFAULT 'Active',
    Budget DECIMAL(18,2),
    CreatedDate DATETIME2 DEFAULT GETDATE(),
    ModifiedDate DATETIME2
);
