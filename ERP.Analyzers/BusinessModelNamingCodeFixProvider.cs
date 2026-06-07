using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace ERP.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BusinessModelNamingCodeFixProvider)), Shared]
    public class BusinessModelNamingCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Constants.BusinessModelDiagnosticId, Constants.DataModelDiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root == null) return;

            var diagnostic = context.Diagnostics[0];
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var classDeclaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            if (classDeclaration == null) return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: GetTitle(diagnostic.Id),
                    createChangedSolution: c => RenameClassAsync(context.Document, classDeclaration, c),
                    equivalenceKey: GetTitle(diagnostic.Id)),
                diagnostic);
        }

        private async Task<Solution> RenameClassAsync(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken cancellationToken)
        {
            var documentIdentifier = classDeclaration.Identifier;
            var oldName = documentIdentifier.Text;
            var newName = oldName + GetSuffixForDocument(document, classDeclaration);

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            if (semanticModel == null) return document.Project.Solution;

            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken);
            if (classSymbol == null) return document.Project.Solution;

            var originalSolution = document.Project.Solution;

            // Renamer.RenameSymbolAsync dans Microsoft.CodeAnalysis.Workspaces prend : Solution, ISymbol, string, OptionSet (optionnel/nullable ou remplacé par SymbolRenameOptions dans les versions récentes)
            var newSolution = await Renamer.RenameSymbolAsync(
                originalSolution,
                classSymbol,
                newName,
                optionSet: null,
                cancellationToken).ConfigureAwait(false);

            return newSolution;
        }

        private static string GetTitle(string diagnosticId)
        {
            return diagnosticId == Constants.DataModelDiagnosticId
                ? Constants.DataModelCodeFixTitle
                : Constants.BusinessModelCodeFixTitle;
        }

        private static string GetSuffixForDocument(Document document, ClassDeclarationSyntax classDeclaration)
        {
            var filePath = classDeclaration.SyntaxTree.FilePath.Replace('/', '\\');
            return filePath.Contains(Constants.DataModelsFolder)
                ? Constants.DataModelSuffix
                : Constants.BusinessModelSuffix;
        }
    }
}
