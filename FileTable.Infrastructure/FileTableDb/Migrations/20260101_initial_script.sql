USE documentdb;
GO

EXEC sp_configure 'filestream access level', 2;
RECONFIGURE;

-- Step 1: Check current state
SELECT 
    fg.name AS FileGroupName,
    fg.type_desc,
    df.name AS FileName,
    df.physical_name
FROM sys.filegroups fg
LEFT JOIN sys.database_files df ON fg.data_space_id = df.data_space_id
WHERE fg.type_desc = 'FILESTREAM_DATA_FILEGROUP';
GO

-- Step 2: Create FILESTREAM filegroup if it doesn't exist
IF NOT EXISTS (
    SELECT 1 FROM sys.filegroups 
    WHERE type_desc = 'FILESTREAM_DATA_FILEGROUP'
)
BEGIN
    PRINT 'Creating FILESTREAM filegroup...';
    ALTER DATABASE documentdb
    ADD FILEGROUP FileStreamGroup CONTAINS FILESTREAM;
    PRINT 'FILESTREAM filegroup created successfully!';
END
ELSE
BEGIN
    PRINT 'FILESTREAM filegroup already exists';
END
GO

-- Step 3: Add a file to the FILESTREAM filegroup
DECLARE @DataPath NVARCHAR(500);
DECLARE @FileStreamPath NVARCHAR(500);
DECLARE @SQL NVARCHAR(MAX);

-- Get SQL Server default data path
SELECT @DataPath = CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS NVARCHAR(500));
SET @FileStreamPath = @DataPath + 'DocumentDBFileStream';

PRINT 'Adding FILESTREAM file at: ' + @FileStreamPath;

-- Check if file already exists
IF NOT EXISTS (
    SELECT 1 FROM sys.database_files 
    WHERE type = 2  -- FILESTREAM type
)
BEGIN
    SET @SQL = N'ALTER DATABASE documentdb
    ADD FILE (
        NAME = N''DocumentDBFileStream'',
        FILENAME = N''' + @FileStreamPath + '''
    ) TO FILEGROUP FileStreamGroup';
    
    EXEC sp_executesql @SQL;
    PRINT 'FILESTREAM file added successfully!';
END
ELSE
BEGIN
    PRINT 'FILESTREAM file already exists';
END
GO

-- Step 4: Enable non-transactional access and set directory name for FileTable
DECLARE @CurrentDirName NVARCHAR(255);
SELECT @CurrentDirName = non_transacted_access_desc 
FROM sys.database_filestream_options 
WHERE database_id = DB_ID('documentdb');

IF @CurrentDirName IS NULL OR NOT EXISTS (
    SELECT 1 FROM sys.database_filestream_options 
    WHERE database_id = DB_ID('documentdb') 
    AND directory_name IS NOT NULL
)
BEGIN
    PRINT 'Setting FILESTREAM directory name and enabling non-transactional access...';
    
    ALTER DATABASE documentdb
    SET FILESTREAM (
        NON_TRANSACTED_ACCESS = FULL,
        DIRECTORY_NAME = N'DocumentDB_FileStream'
    );
    
    PRINT 'FILESTREAM directory configured successfully!';
END
ELSE
BEGIN
    PRINT 'FILESTREAM directory already configured';
END
GO

-- Step 5: Verify the complete setup
PRINT '=== FILESTREAM Configuration Summary ===';

SELECT 
    fg.name AS FileGroupName,
    fg.type_desc,
    df.name AS FileName,
    df.physical_name,
    df.type_desc AS FileType
FROM sys.filegroups fg
LEFT JOIN sys.database_files df ON fg.data_space_id = df.data_space_id
WHERE fg.type_desc = 'FILESTREAM_DATA_FILEGROUP';

SELECT 
    DB_NAME(database_id) AS DatabaseName,
    directory_name AS DirectoryName,
    non_transacted_access_desc AS NonTransactedAccess
FROM sys.database_filestream_options
WHERE database_id = DB_ID('documentdb');

PRINT 'Configuration complete! You can now create FileTables.';
GO
-- Step 5: Verify
SELECT 
    name AS TableName,
    FileTableRootPath(name) AS UNCPath
FROM sys.tables
WHERE is_filetable = 1;
GO

CREATE TABLE Documents AS FILETABLE
  WITH
  (
    FILETABLE_DIRECTORY = N'Documents',
    FILETABLE_COLLATE_FILENAME = database_default
  )

--  Examples  of  insertion  --
/*INSERT INTO Documents (name, file_stream)
VALUES ('MyDocument.pdf', 
        CAST('Your file content here' AS VARBINARY(MAX)));

-- Insert using OPENROWSET to load from file system
INSERT INTO Documents (name, file_stream)
SELECT 'dump la base.txt', 
       BulkColumn 
FROM OPENROWSET(BULK 'C:\Temp\dump la base.txt', SINGLE_BLOB) AS FileData;
*/