
ALTER TABLE [documentdb].[dbo].[EntrepriseWarehouse]
ADD FranchiseType NVARCHAR(50) NULL,
    FranchiseAmount DECIMAL(18,2) NULL,
    FranchisePercentage DECIMAL(5,2) NULL;
