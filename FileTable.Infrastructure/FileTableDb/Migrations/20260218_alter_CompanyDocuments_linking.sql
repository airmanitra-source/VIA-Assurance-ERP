-- Migration to add specific item linking to CompanyDocuments
-- Adding foreign keys for Fleet, Warehouse, and Transportation items.

ALTER TABLE CompanyDocuments
ADD EntrepriseFleetID BIGINT NULL,
    EntrepriseWarehouseID BIGINT NULL,
    EntrepriseMerchandiseTransportationID BIGINT NULL;

-- Adding Foreign Key constraints
ALTER TABLE CompanyDocuments
ADD CONSTRAINT FK_CompanyDocuments_Fleet FOREIGN KEY (EntrepriseFleetID) 
    REFERENCES EntrepriseFleet(Id);

ALTER TABLE CompanyDocuments
ADD CONSTRAINT FK_CompanyDocuments_Warehouse FOREIGN KEY (EntrepriseWarehouseID) 
    REFERENCES EntrepriseWarehouse(Id);

ALTER TABLE CompanyDocuments
ADD CONSTRAINT FK_CompanyDocuments_Transportation FOREIGN KEY (EntrepriseMerchandiseTransportationID) 
    REFERENCES EntrepriseMerchandiseTransportation(Id);

-- Optional: Indices for performance
CREATE INDEX IX_CompanyDocuments_Fleet ON CompanyDocuments(EntrepriseFleetID);
CREATE INDEX IX_CompanyDocuments_Warehouse ON CompanyDocuments(EntrepriseWarehouseID);
CREATE INDEX IX_CompanyDocuments_Transportation ON CompanyDocuments(EntrepriseMerchandiseTransportationID);
