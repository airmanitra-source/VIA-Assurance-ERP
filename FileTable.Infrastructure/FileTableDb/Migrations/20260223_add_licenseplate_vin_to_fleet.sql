-- Migration to add LicensePlate and VIN to 'EntrepriseFleet' table
-- Date: 2026-02-23
USE documentdb

-- Add LicensePlate column (Plaque d'immatriculation)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[EntrepriseFleet]') AND name = 'LicensePlate')
BEGIN
    ALTER TABLE [documentdb].[dbo].[EntrepriseFleet]
    ADD [LicensePlate] NVARCHAR(20) NULL;
    
    PRINT 'Column LicensePlate added to EntrepriseFleet table';
END
ELSE
BEGIN
    PRINT 'Column LicensePlate already exists in EntrepriseFleet table';
END
GO

-- Add VIN column (Vehicle Identification Number)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[EntrepriseFleet]') AND name = 'VIN')
BEGIN
    ALTER TABLE [documentdb].[dbo].[EntrepriseFleet]
    ADD [VIN] NVARCHAR(17) NULL;
    
    PRINT 'Column VIN added to EntrepriseFleet table';
END
ELSE
BEGIN
    PRINT 'Column VIN already exists in EntrepriseFleet table';
END
GO
