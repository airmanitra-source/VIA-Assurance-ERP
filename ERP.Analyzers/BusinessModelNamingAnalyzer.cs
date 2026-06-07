using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ERP.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BusinessModelNamingAnalyzer : DiagnosticAnalyzer
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
#pragma warning restore RS2008

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(BusinessModelRule, DataModelRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;
            var filePath = classDeclaration.SyntaxTree.FilePath;
            if (string.IsNullOrEmpty(filePath))
                return;

            var normalizedPath = filePath.Replace('/', '\\');
            var className = classDeclaration.Identifier.Text;

            if (normalizedPath.Contains(Constants.BusinessModelsFolder) && !className.EndsWith(Constants.BusinessModelSuffix))
            {
                context.ReportDiagnostic(Diagnostic.Create(BusinessModelRule, classDeclaration.Identifier.GetLocation(), className));
            }

            if (normalizedPath.Contains(Constants.DataModelsFolder) && !className.EndsWith(Constants.DataModelSuffix))
            {
                context.ReportDiagnostic(Diagnostic.Create(DataModelRule, classDeclaration.Identifier.GetLocation(), className));
            }
        }
    }
}
