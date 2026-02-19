CREATE TABLE CompanyDocuments (
    EntrepriseID BIGINT NOT NULL,
    FileStreamID UNIQUEIDENTIFIER NOT NULL, -- Correspond au stream_id de la FileTable
    TypeDocument NVARCHAR(50), -- Optionnel : ex. 'Confirmation Police', 'Facture'
    
    CONSTRAINT FK_Company FOREIGN KEY (EntrepriseID) 
        REFERENCES Entreprise(Id),
        
    CONSTRAINT FK_CompanyFileTable FOREIGN KEY (FileStreamID) 
        REFERENCES Documents(stream_id), 
        
    PRIMARY KEY (EntrepriseID, FileStreamID)
);
