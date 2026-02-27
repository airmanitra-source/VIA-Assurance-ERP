IF NOT EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID('[documentdb].[dbo].[Employee]')
      AND name = 'NombreEnfants'
)
BEGIN
    ALTER TABLE [documentdb].[dbo].[Employee]
    ADD [NombreEnfants] INT NOT NULL DEFAULT 0;
END
