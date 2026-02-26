-- Ajouter une contrainte UNIQUE sur le numéro de matricule
IF NOT EXISTS (
    SELECT 1 FROM sys.objects 
    WHERE object_id = OBJECT_ID(N'[documentdb].[dbo].[UQ_Employee_NumeroMatricule]')
    AND type = 'UQ'
)
BEGIN
    ALTER TABLE [documentdb].[dbo].[Employee]
    ADD CONSTRAINT [UQ_Employee_NumeroMatricule] UNIQUE (NumeroMatricule);
    
    PRINT 'Contrainte UNIQUE ajoutée au NumeroMatricule';
END
ELSE
BEGIN
    PRINT 'La contrainte UNIQUE sur NumeroMatricule existe déjà';
END
