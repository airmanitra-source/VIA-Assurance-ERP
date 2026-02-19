-- Migration to create '[Sinister]' table
-- Date: 2026-02-16
-- Purpose: Store insurance policy metadata contracted by an entreprise.
-- IMPORTANT: This table stores metadata and references for insurance policies
-- (policy number, insurer, coverage dates, amounts, applicability) but MUST
-- NOT contain full contract text, sensitive terms, or personally-identifying
-- payment credentials. Store only summary / reference identifiers and links
-- to external contract storage if needed.

USE documentdb;
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Sinister' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
	CREATE TABLE [documentdb].[dbo].[Sinister] (
		[Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
		[PolicyNumber] NVARCHAR(200) NOT NULL,
		[EntrepriseId] BIGINT NOT NULL,
		[InsurerName] NVARCHAR(200) NULL,
		[InsurerContact] NVARCHAR(200) NULL,
		[CoverageStartDate] DATETIME2 NOT NULL,
		[CoverageEndDate] DATETIME2 NULL,
		[CoverageType] NVARCHAR(100) NULL, -- e.g. Comprehensive, ThirdParty, Cargo
		[CoveredAssets] NVARCHAR(MAX) NULL, -- JSON or comma-separated identifiers describing applicable assets/categories
		[PremiumAmount] DECIMAL(18,2) NULL,
		[DeductibleAmount] DECIMAL(18,2) NULL,
		[PolicyLimits] NVARCHAR(MAX) NULL,
		[IsActive] BIT NOT NULL DEFAULT 1,
		[PolicyReferenceId] NVARCHAR(200) NULL, -- external/internal reference to contract storage
		[Notes] NVARCHAR(MAX) NULL,

		[CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
		[LastModifiedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

		CONSTRAINT [UQ_Sinister_PolicyNumber] UNIQUE ([PolicyNumber]),

		CONSTRAINT [FK_Sinister_Entreprise] FOREIGN KEY ([EntrepriseId]) REFERENCES [documentdb].[dbo].[Entreprise]([Id]) ON DELETE CASCADE
	);

	CREATE INDEX [IX_Sinister_EntrepriseId] ON [documentdb].[dbo].Sinister([EntrepriseId]);
	CREATE INDEX [IX_Sinister_PolicyNumber] ON [documentdb].[dbo].Sinister([PolicyNumber]);
	CREATE INDEX [IX_Sinister_IsActive] ON [documentdb].[dbo].Sinister([IsActive]);
	CREATE INDEX [IX_Sinister_CoverageDates] ON [documentdb].[dbo].Sinister([CoverageStartDate], [CoverageEndDate]);
END
GO

