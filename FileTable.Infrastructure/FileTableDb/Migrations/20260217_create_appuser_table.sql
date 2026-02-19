USE documentdb;
GO

-- =============================================
-- Migration: Separate AppUser from Entreprise
-- =============================================

-- 1. Create AppUser table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AppUser')
BEGIN
    CREATE TABLE [documentdb].[dbo].[AppUser](
        [Id]                 BIGINT IDENTITY(1,1) PRIMARY KEY,
        [UserName]           NVARCHAR(256) NOT NULL,
        [NormalizedUserName] NVARCHAR(256) NOT NULL,
        [Email]              NVARCHAR(256) NOT NULL,
        [NormalizedEmail]    NVARCHAR(256) NOT NULL,
        [PasswordHash]       NVARCHAR(MAX),
        [SecurityStamp]      NVARCHAR(MAX),
        [EntrepriseId]       BIGINT NOT NULL
    );
    PRINT 'AppUser table created.';
END
ELSE
BEGIN
    PRINT 'AppUser table already exists.';
END
GO

-- 2. Migrate existing user data from Entreprise to AppUser
-- Only migrate rows that have Identity data (UserName is not null)
SET IDENTITY_INSERT [documentdb].[dbo].[AppUser] ON;

INSERT INTO [documentdb].[dbo].[AppUser] (Id, UserName, NormalizedUserName, Email, NormalizedEmail, PasswordHash, SecurityStamp, EntrepriseId)
SELECT Id, UserName, NormalizedUserName, Email, NormalizedEmail, PasswordHash, SecurityStamp, Id
FROM [documentdb].[dbo].[Entreprise]
WHERE UserName IS NOT NULL;

SET IDENTITY_INSERT [documentdb].[dbo].[AppUser] OFF;

PRINT 'Migrated existing users from Entreprise to AppUser.';
GO

-- 3. Update EntrepriseUserRole FK to reference AppUser instead of Entreprise
-- First drop the old FK constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.EntrepriseUserRole') AND referenced_object_id = OBJECT_ID('dbo.Entreprise'))
BEGIN
    DECLARE @fkName NVARCHAR(256);
    SELECT @fkName = name FROM sys.foreign_keys 
    WHERE parent_object_id = OBJECT_ID('dbo.EntrepriseUserRole') 
    AND referenced_object_id = OBJECT_ID('dbo.Entreprise');
    
    IF @fkName IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE [documentdb].[dbo].[EntrepriseUserRole] DROP CONSTRAINT [' + @fkName + ']');
        PRINT 'Dropped old FK constraint on EntrepriseUserRole -> Entreprise.';
    END
END
GO

-- Add new FK to AppUser
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.EntrepriseUserRole') AND referenced_object_id = OBJECT_ID('dbo.AppUser'))
BEGIN
    ALTER TABLE [documentdb].[dbo].[EntrepriseUserRole]
    ADD CONSTRAINT FK_EntrepriseUserRole_AppUser
    FOREIGN KEY (UserId) REFERENCES [documentdb].[dbo].[AppUser](Id) ON DELETE CASCADE;
    PRINT 'Added new FK constraint on EntrepriseUserRole -> AppUser.';
END
GO

-- 4. Add FK from AppUser to Entreprise
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.AppUser') AND referenced_object_id = OBJECT_ID('dbo.Entreprise'))
BEGIN
    ALTER TABLE [documentdb].[dbo].[AppUser]
    ADD CONSTRAINT FK_AppUser_Entreprise
    FOREIGN KEY (EntrepriseId) REFERENCES [documentdb].[dbo].[Entreprise](Id);
    PRINT 'Added FK constraint on AppUser -> Entreprise.';
END
GO

-- 5. Remove Identity columns from Entreprise (optional - can be done later)
-- Uncomment when ready:
-- ALTER TABLE [documentdb].[dbo].[Entreprise] DROP COLUMN UserName, NormalizedUserName, NormalizedEmail, PasswordHash, SecurityStamp;

PRINT 'Migration complete. AppUser table is ready.';
GO
