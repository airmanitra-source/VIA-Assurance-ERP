-- Add franchise (deductible) fields to insurance tables
ALTER TABLE [documentdb].[dbo].[EntrepriseFleet]
ADD FranchiseType NVARCHAR(50) NULL,
    FranchiseAmount DECIMAL(18,2) NULL,
    FranchisePercentage DECIMAL(5,2) NULL;
