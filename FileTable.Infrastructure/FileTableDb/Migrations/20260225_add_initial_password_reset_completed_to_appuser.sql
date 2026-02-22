-- Migration: 20260225_add_initial_password_reset_completed_to_appuser.sql
-- Purpose: Add InitialPasswordResetCompleted column to AppUser table for tracking password reset status

BEGIN TRANSACTION;

-- Check if InitialPasswordResetCompleted column already exists
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AppUser' AND COLUMN_NAME = 'InitialPasswordResetCompleted')
BEGIN
    ALTER TABLE [documentdb].[dbo].[AppUser]
    ADD InitialPasswordResetCompleted BIT NOT NULL DEFAULT 0;
    
    PRINT 'InitialPasswordResetCompleted column added to AppUser table with default value 0 (false)';
END
ELSE
BEGIN
    PRINT 'InitialPasswordResetCompleted column already exists in AppUser table';
END;

COMMIT TRANSACTION;
