ALTER TABLE documentdb.dbo.EmployeeDocuments DROP CONSTRAINT PK__Employee__B8453B6150AC9025;

ALTER TABLE EmployeeDocuments 
     ADD Id BIGINT Identity(1,1) PRIMARY KEY

