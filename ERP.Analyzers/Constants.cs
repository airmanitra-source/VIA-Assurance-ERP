namespace ERP.Analyzers
{
    public static class Constants
    {
        public const string AnalyzerCategory = "Naming";

        public const string BusinessModelDiagnosticId = "ERP0001";
        public const string DataModelDiagnosticId = "ERP0002";
        public const string DataProviderDiagnosticId = "ERP0003";

        public const string BusinessModelsFolder = @"\Business\Models\";
        public const string DataModelsFolder = @"\Data\Models\";
        public const string DataProvidersFolder = @"\Data\Providers\";

        public const string BusinessModelSuffix = "BusinessModel";
        public const string DataModelSuffix = "DataModel";
        public const string DataProviderSuffix = "DataProvider";

        public const string BusinessModelTitle = "Le nom de la classe du modèle métier doit être suffixé par 'BusinessModel'";
        public const string DataModelTitle = "Le nom de la classe du modèle de données doit être suffixé par 'DataModel'";
        public const string DataProviderTitle = "Le nom de la classe du provider de données doit être suffixé par 'DataProvider'";

        public const string BusinessModelMessageFormat = "La classe '{0}' située dans un dossier 'Business\\Models' doit être nommée avec le suffixe 'BusinessModel'";
        public const string DataModelMessageFormat = "La classe '{0}' située dans un dossier 'Data\\Models' doit être nommée avec le suffixe 'DataModel'";
        public const string DataProviderMessageFormat = "La classe '{0}' située dans un dossier 'Data\\Providers' doit être nommée avec le suffixe 'DataProvider'";
        public const string BusinessModelFileMessageFormat = "Le fichier '{0}' situé dans un dossier 'Business\\Models' doit être nommé avec le suffixe 'BusinessModel'";
        public const string DataModelFileMessageFormat = "Le fichier '{0}' situé dans un dossier 'Data\\Models' doit être nommé avec le suffixe 'DataModel'";
        public const string DataProviderFileMessageFormat = "Le fichier '{0}' situé dans un dossier 'Data\\Providers' doit être nommé avec le suffixe 'DataProvider'";

        public const string BusinessModelDescription = "Toutes les classes se trouvant dans le dossier Business\\Models des modules de l'application doivent obligatoirement être suffixées par 'BusinessModel' conformément aux directives d'architecture.";
        public const string DataModelDescription = "Toutes les classes se trouvant dans le dossier Data\\Models des modules de l'application doivent obligatoirement être suffixées par 'DataModel' conformément aux directives d'architecture.";
        public const string DataProviderDescription = "Toutes les classes se trouvant dans le dossier Data\\Providers des modules de l'application doivent obligatoirement être suffixées par 'DataProvider' conformément aux directives d'architecture.";

        public const string BusinessModelCodeFixTitle = "Suffixer la classe avec 'BusinessModel'";
        public const string DataModelCodeFixTitle = "Suffixer la classe avec 'DataModel'";
        public const string DataProviderCodeFixTitle = "Suffixer la classe avec 'DataProvider'";
    }
}