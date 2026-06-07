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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FolderModelNamingCodeFixProvider)), Shared]
    public class FolderModelNamingCodeFixProvider : CodeFixProvider
    {
        private readonly INamingHelper _namingHelper;

        public FolderModelNamingCodeFixProvider() : this(new NamingHelper())
        {
        }

        public FolderModelNamingCodeFixProvider(INamingHelper namingHelper)
        {
            _namingHelper = namingHelper;
        }

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Constants.BusinessModelDiagnosticId, Constants.DataModelDiagnosticId, Constants.DataProviderDiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root == null) return;

            var diagnostic = context.Diagnostics[0];
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var typeDeclaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().FirstOrDefault();
            if (typeDeclaration == null) return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: _namingHelper.GetCodeFixTitle(diagnostic.Id),
                    createChangedSolution: c => RenameTypeAsync(context.Document, typeDeclaration, c),
                    equivalenceKey: _namingHelper.GetCodeFixTitle(diagnostic.Id)),
                diagnostic);
        }

        private async Task<Solution> RenameTypeAsync(Document document, TypeDeclarationSyntax typeDeclaration, CancellationToken cancellationToken)
        {
            var documentIdentifier = typeDeclaration.Identifier;
            var oldName = documentIdentifier.Text;
            var newName = oldName + _namingHelper.GetSuffixForDocumentFilePath(typeDeclaration.SyntaxTree.FilePath);

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            if (semanticModel == null) return document.Project.Solution;

            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration, cancellationToken);
            if (typeSymbol == null) return document.Project.Solution;

            var originalSolution = document.Project.Solution;

            // Renamer.RenameSymbolAsync dans Microsoft.CodeAnalysis.Workspaces prend : Solution, ISymbol, string, OptionSet (optionnel/nullable ou remplacé par SymbolRenameOptions dans les versions récentes)
            var newSolution = await Renamer.RenameSymbolAsync(
                originalSolution,
                typeSymbol,
                newName,
                optionSet: null,
                cancellationToken).ConfigureAwait(false);

            return newSolution;
        }
    }
}
