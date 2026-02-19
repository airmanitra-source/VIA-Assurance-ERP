CREATE TABLE EmployeeDocuments (
    EmployeeID INT NOT NULL,
    FileStreamID UNIQUEIDENTIFIER NOT NULL, -- Correspond au stream_id de la FileTable
    TypeDocument NVARCHAR(50), -- Optionnel : ex. 'Contrat', 'CV'
    
    CONSTRAINT FK_Employee FOREIGN KEY (EmployeeID) 
        REFERENCES Employee(EmployeeID),
        
    CONSTRAINT FK_FileTable FOREIGN KEY (FileStreamID) 
        REFERENCES Documents(stream_id), 
        
    PRIMARY KEY (EmployeeID, FileStreamID)
);
