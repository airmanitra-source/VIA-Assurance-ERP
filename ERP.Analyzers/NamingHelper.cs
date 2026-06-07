namespace ERP.Analyzers
{
    public interface INamingHelper
    {
        string GetTitle(string diagnosticId);
        string GetDescription(string diagnosticId);
        string GetCodeFixTitle(string diagnosticId);
        string GetSuffixForDocumentFilePath(string filePath);
    }

    public class NamingHelper : INamingHelper
    {
        public string GetTitle(string diagnosticId)
        {
            return diagnosticId switch
            {
                Constants.DataModelDiagnosticId => Constants.DataModelTitle,
                Constants.DataProviderDiagnosticId => Constants.DataProviderTitle,
                _ => Constants.BusinessModelTitle,
            };
        }

        public string GetDescription(string diagnosticId)
        {
            return diagnosticId switch
            {
                Constants.DataModelDiagnosticId => Constants.DataModelDescription,
                Constants.DataProviderDiagnosticId => Constants.DataProviderDescription,
                _ => Constants.BusinessModelDescription,
            };
        }

        public string GetCodeFixTitle(string diagnosticId)
        {
            return diagnosticId switch
            {
                Constants.DataModelDiagnosticId => Constants.DataModelCodeFixTitle,
                Constants.DataProviderDiagnosticId => Constants.DataProviderCodeFixTitle,
                _ => Constants.BusinessModelCodeFixTitle,
            };
        }

        public string GetSuffixForDocumentFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return Constants.BusinessModelSuffix;

            var normalizedPath = filePath.Replace('/', '\\');
            if (normalizedPath.Contains(Constants.DataModelsFolder))
                return Constants.DataModelSuffix;

            if (normalizedPath.Contains(Constants.DataProvidersFolder))
                return Constants.DataProviderSuffix;

            return Constants.BusinessModelSuffix;
        }
    }
}
