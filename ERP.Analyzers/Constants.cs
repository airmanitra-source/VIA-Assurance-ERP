namespace ERP.Analyzers
{
    internal static class Constants
    {
        internal const string AnalyzerCategory = "Naming";

        internal const string BusinessModelDiagnosticId = "ERP0001";
        internal const string DataModelDiagnosticId = "ERP0002";

        internal const string BusinessModelsFolder = @"\Business\Models\";
        internal const string DataModelsFolder = @"\Data\Models\";

        internal const string BusinessModelSuffix = "BusinessModel";
        internal const string DataModelSuffix = "DataModel";

        internal const string BusinessModelTitle = "Le nom de la classe du modèle métier doit être suffixé par 'BusinessModel'";
        internal const string DataModelTitle = "Le nom de la classe du modèle de données doit être suffixé par 'DataModel'";

        internal const string BusinessModelMessageFormat = "La classe '{0}' située dans un dossier 'Business\\Models' doit être nommée avec le suffixe 'BusinessModel'";
        internal const string DataModelMessageFormat = "La classe '{0}' située dans un dossier 'Data\\Models' doit être nommée avec le suffixe 'DataModel'";

        internal const string BusinessModelDescription = "Toutes les classes se trouvant dans le dossier Business\\Models des modules de l'application doivent obligatoirement être suffixées par 'BusinessModel' conformément aux directives d'architecture.";
        internal const string DataModelDescription = "Toutes les classes se trouvant dans le dossier Data\\Models des modules de l'application doivent obligatoirement être suffixées par 'DataModel' conformément aux directives d'architecture.";

        internal const string BusinessModelCodeFixTitle = "Suffixer la classe avec 'BusinessModel'";
        internal const string DataModelCodeFixTitle = "Suffixer la classe avec 'DataModel'";
    }
}