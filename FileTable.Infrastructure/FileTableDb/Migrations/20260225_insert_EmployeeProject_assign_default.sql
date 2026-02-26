-- Affectation du projet 'On Boarding' par défaut à tous les employés existants
DECLARE @ProjectID INT;

-- Récupération de l'ID du projet (Notez les guillemets simples autour du nom)
SELECT @ProjectID = ProjectID 
FROM [documentdb].[dbo].[Project] 
WHERE ProjectName = 'On Boarding'; 

-- Vérification que le projet existe
IF @ProjectID IS NOT NULL
BEGIN
    -- Affectation du projet à tous les employés qui ne l'ont pas déjà
    INSERT INTO [documentdb].[dbo].[EmployeeProject] (EmployeeID, ProjectID, AssignedDate, IsActive, CreatedDate)
    SELECT e.EmployeeID, @ProjectID, GETDATE(), 1, GETDATE()
    FROM [documentdb].[dbo].[Employee] e
    WHERE NOT EXISTS (
        SELECT 1 FROM [documentdb].[dbo].[EmployeeProject] ep 
        WHERE ep.EmployeeID = e.EmployeeID AND ep.ProjectID = @ProjectID
    );

    PRINT 'Affectation du projet On Boarding complétée avec succès.';
END
ELSE 
BEGIN
    PRINT 'Erreur : Le projet On Boarding n existe pas. Veuillez d abord exécuter le script de création du projet.';
END
