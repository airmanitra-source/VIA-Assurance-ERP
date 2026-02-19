-- Migration to add 'Frequency' column to 'EntrepriseMerchandiseTransportation' table
-- Date: 2026-02-15
use documentdb

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[documentdb].[dbo].[EntrepriseMerchandiseTransportation]') AND name = 'Frequency')
BEGIN
    ALTER TABLE [documentdb].[dbo].[EntrepriseMerchandiseTransportation]
    ADD [Frequency] NVARCHAR(50) NOT NULL DEFAULT 'OneTime';
    PRINT 'Successfully added Frequency column to EntrepriseMerchandiseTransportation table';
END
ELSE
BEGIN
    PRINT 'Frequency column already exists in EntrepriseMerchandiseTransportation table';
END
GO
