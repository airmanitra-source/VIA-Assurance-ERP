-- Migration: 20260224_add_email_to_employee.sql
-- Purpose: Add Email column to Employee table for user account creation

BEGIN TRANSACTION;

-- Check if Email column already exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Employee' AND COLUMN_NAME = 'Email')
BEGIN
    ALTER TABLE [documentdb].[dbo].[Employee]
    ADD Email NVARCHAR(256) NULL;
    
    PRINT 'Email column added to Employee table';
END
ELSE
BEGIN
    PRINT 'Email column already exists in Employee table';
END;

COMMIT TRANSACTION;
