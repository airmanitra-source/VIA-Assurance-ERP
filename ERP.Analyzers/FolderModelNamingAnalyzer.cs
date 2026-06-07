using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ERP.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FolderModelNamingAnalyzer : DiagnosticAnalyzer
    {
#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor BusinessModelRule = new DiagnosticDescriptor(
            Constants.BusinessModelDiagnosticId,
            Constants.BusinessModelTitle,
            Constants.BusinessModelMessageFormat,
            Constants.AnalyzerCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Constants.BusinessModelDescription);

        private static readonly DiagnosticDescriptor DataModelRule = new DiagnosticDescriptor(
            Constants.DataModelDiagnosticId,
            Constants.DataModelTitle,
            Constants.DataModelMessageFormat,
            Constants.AnalyzerCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Constants.DataModelDescription);

        private static readonly DiagnosticDescriptor DataProviderRule = new DiagnosticDescriptor(
            Constants.DataProviderDiagnosticId,
            Constants.DataProviderTitle,
            Constants.DataProviderMessageFormat,
            Constants.AnalyzerCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Constants.DataProviderDescription);
#pragma warning restore RS2008

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(BusinessModelRule, DataModelRule, DataProviderRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.ClassDeclaration, SyntaxKind.InterfaceDeclaration);
        }

        private void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;
            var filePath = typeDeclaration.SyntaxTree.FilePath;
            if (string.IsNullOrEmpty(filePath))
                return;

            var normalizedPath = filePath.Replace('/', '\\');
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            var typeName = typeDeclaration.Identifier.Text;

            if (normalizedPath.Contains(Constants.BusinessModelsFolder))
            {
                ReportIfTypeNameInvalid(context, typeDeclaration, typeName, Constants.BusinessModelSuffix, BusinessModelRule);
                ReportIfFileNameInvalid(context, typeDeclaration, fileNameWithoutExtension, Constants.BusinessModelSuffix, Constants.BusinessModelFileMessageFormat, Constants.BusinessModelDiagnosticId);
            }

            if (normalizedPath.Contains(Constants.DataModelsFolder))
            {
                ReportIfTypeNameInvalid(context, typeDeclaration, typeName, Constants.DataModelSuffix, DataModelRule);
                ReportIfFileNameInvalid(context, typeDeclaration, fileNameWithoutExtension, Constants.DataModelSuffix, Constants.DataModelFileMessageFormat, Constants.DataModelDiagnosticId);
            }

            if (normalizedPath.Contains(Constants.DataProvidersFolder))
            {
                ReportIfTypeNameInvalid(context, typeDeclaration, typeName, Constants.DataProviderSuffix, DataProviderRule);
                ReportIfFileNameInvalid(context, typeDeclaration, fileNameWithoutExtension, Constants.DataProviderSuffix, Constants.DataProviderFileMessageFormat, Constants.DataProviderDiagnosticId);
            }
        }

        private static void ReportIfTypeNameInvalid(
            SyntaxNodeAnalysisContext context,
            TypeDeclarationSyntax typeDeclaration,
            string typeName,
            string expectedSuffix,
            DiagnosticDescriptor rule)
        {
            if (!typeName.EndsWith(expectedSuffix))
            {
                context.ReportDiagnostic(Diagnostic.Create(rule, typeDeclaration.Identifier.GetLocation(), typeName));
            }
        }

        private static void ReportIfFileNameInvalid(
            SyntaxNodeAnalysisContext context,
            TypeDeclarationSyntax typeDeclaration,
            string fileNameWithoutExtension,
            string expectedSuffix,
            string messageFormat,
            string diagnosticId)
        {
            if (!fileNameWithoutExtension.EndsWith(expectedSuffix))
            {
                var descriptor = new DiagnosticDescriptor(
                    diagnosticId,
                    GetTitle(diagnosticId),
                    messageFormat,
                    Constants.AnalyzerCategory,
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description: GetDescription(diagnosticId));

                context.ReportDiagnostic(Diagnostic.Create(descriptor, typeDeclaration.Identifier.GetLocation(), fileNameWithoutExtension));
            }
        }

        private static string GetTitle(string diagnosticId)
        {
            return diagnosticId switch
            {
                Constants.DataModelDiagnosticId => Constants.DataModelTitle,
                Constants.DataProviderDiagnosticId => Constants.DataProviderTitle,
                _ => Constants.BusinessModelTitle,
            };
        }

        private static string GetDescription(string diagnosticId)
        {
            return diagnosticId switch
            {
                Constants.DataModelDiagnosticId => Constants.DataModelDescription,
                Constants.DataProviderDiagnosticId => Constants.DataProviderDescription,
                _ => Constants.BusinessModelDescription,
            };
        }
    }
}
