ALTER TABLE Employee 
    ADD EntrepriseID bigint not null
     CONSTRAINT FK_Entreprise FOREIGN KEY (EntrepriseID) 
        REFERENCES Entreprise(Id)