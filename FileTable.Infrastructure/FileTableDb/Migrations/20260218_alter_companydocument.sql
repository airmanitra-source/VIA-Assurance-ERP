/* SQL Script to update CompanyDocuments table */
ALTER TABLE [documentdb].[dbo].[CompanyDocuments]
ADD [IsSigned] BIT NOT NULL DEFAULT 0,
    [SignedDate] DATETIME2 NULL;