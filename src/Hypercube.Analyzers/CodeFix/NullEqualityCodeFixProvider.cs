using System.Collections.Immutable;
using System.Composition;
using Hypercube.Analyzers.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hypercube.Analyzers.CodeFix;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NullEqualityCodeFixProvider))]
public sealed class NullEqualityCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [NullEqualityAnalyzer.DiagnosticId];

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document
            .GetSyntaxRootAsync(context.CancellationToken)
            .ConfigureAwait(false);

        if (root is null)
            return;

        var diagnostic = context.Diagnostics[0];
        var node = root.FindNode(diagnostic.Location.SourceSpan) as BinaryExpressionSyntax;

        if (node is null)
            return;

        const string title = "Replace with \"is\"";
        context.RegisterCodeFix(
            CodeAction.Create(
                title,
                c => ReplaceWithIsNullAsync(context.Document, node, c),
                equivalenceKey: title),
            diagnostic);
    }

    private static async Task<Document> ReplaceWithIsNullAsync(Document document, BinaryExpressionSyntax binary, CancellationToken cancellationToken)
    {
        if (!binary.IsKind(SyntaxKind.EqualsExpression) && !binary.IsKind(SyntaxKind.NotEqualsExpression))
            return document;
        
        var left = binary.Left;
        var right = binary.Right;
        
        if (!IsNullLiteral(right) && !IsNullLiteral(left))
            return document;
        
        var target = left;
        if (IsNullLiteral(right))
            target = left;
        
        var isNegated = binary.IsKind(SyntaxKind.NotEqualsExpression);
        var newExpr = isNegated
            ? SyntaxFactory.IsPatternExpression(target, SyntaxFactory.UnaryPattern(SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression))))
            : SyntaxFactory.IsPatternExpression(target, SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

        var root = await document.GetSyntaxRootAsync(cancellationToken);
        var newRoot = root!.ReplaceNode(binary, newExpr.WithTriviaFrom(binary));

        return document.WithSyntaxRoot(newRoot);

    }

    private static bool IsNullLiteral(ExpressionSyntax node)
    {
        return node.IsKind(SyntaxKind.NullLiteralExpression);
    }
}