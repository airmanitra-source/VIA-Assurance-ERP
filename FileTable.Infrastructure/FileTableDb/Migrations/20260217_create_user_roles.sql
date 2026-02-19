USE documentdb;
GO

-- Create UserRole table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserRole')
BEGIN
    CREATE TABLE [dbo].[UserRole](
        [Id] [nvarchar](450) NOT NULL PRIMARY KEY,
        [Name] [nvarchar](256) NOT NULL,
        [NormalizedName] [nvarchar](256) NOT NULL
    );
    PRINT 'UserRole table created.';
END
ELSE
BEGIN
    PRINT 'UserRole table already exists.';
END
GO

-- Create EntrepriseUserRole table if it doesn't exist (Associative table)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EntrepriseUserRole')
BEGIN
    CREATE TABLE [dbo].[EntrepriseUserRole](
        [UserId] BIGINT NOT NULL,
        [RoleId] [nvarchar](450) NOT NULL,
        PRIMARY KEY (UserId, RoleId),
        FOREIGN KEY (UserId) REFERENCES [dbo].[Entreprise](Id) ON DELETE CASCADE,
        FOREIGN KEY (RoleId) REFERENCES [dbo].[UserRole](Id) ON DELETE CASCADE
    );
    PRINT 'EntrepriseUserRole table created.';
END
ELSE
BEGIN
    PRINT 'EntrepriseUserRole table already exists.';
END
GO

-- Seed Roles
MERGE INTO [dbo].[UserRole] AS Target
USING (VALUES 
    ('1', 'directeur', 'DIRECTEUR'),
    ('2', 'RH', 'RH'),
    ('3', 'auditeur', 'AUDITEUR'),
    ('4', 'admin', 'ADMIN'),
    ('5', 'developer', 'DEVELOPER'),
    ('6', 'employee', 'EMPLOYEE')
) AS Source (Id, Name, NormalizedName)
ON Target.Id = Source.Id
WHEN MATCHED THEN
    UPDATE SET Name = Source.Name, NormalizedName = Source.NormalizedName
WHEN NOT MATCHED THEN
    INSERT (Id, Name, NormalizedName) VALUES (Source.Id, Source.Name, Source.NormalizedName);

PRINT 'Roles seeded successfully.';
GO
