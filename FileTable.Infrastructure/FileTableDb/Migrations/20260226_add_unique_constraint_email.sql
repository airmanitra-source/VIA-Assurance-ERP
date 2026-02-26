CREATE UNIQUE NONCLUSTERED INDEX UIX_Employee_Email
ON [documentdb].[dbo].[Employee](Email)
WHERE Email IS NOT NULL;
