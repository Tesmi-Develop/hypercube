using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hypercube.Analyzers.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NullEqualityAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = Id.NullEquality;
    
    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Null check using '==' or '!=' is not allowed",
        messageFormat: "Use 'is {0}null' instead of '{1} null'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Use 'is null' or 'is not null' instead of equality operators when checking for null.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeNullEquality, SyntaxKind.EqualsExpression);
        context.RegisterSyntaxNodeAction(AnalyzeNullEquality, SyntaxKind.NotEqualsExpression);
    }

    private static void AnalyzeNullEquality(SyntaxNodeAnalysisContext context)
    {
        var binaryExpression = (BinaryExpressionSyntax)context.Node;

        if (!IsNullLiteral(binaryExpression.Right) && !IsNullLiteral(binaryExpression.Left))
            return;

        var operatorToken = binaryExpression.OperatorToken.Text;
        var isNegated = binaryExpression.IsKind(SyntaxKind.NotEqualsExpression);

        var keyword = isNegated ? "not " : string.Empty;
        var diagnostic = Diagnostic.Create(
            Rule,
            binaryExpression.GetLocation(),
            keyword,
            operatorToken);

        context.ReportDiagnostic(diagnostic);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsNullLiteral(ExpressionSyntax expression)
    {
        return expression.IsKind(SyntaxKind.NullLiteralExpression);
    }
}