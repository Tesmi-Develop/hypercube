using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hypercube.Analyzers.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EngineInternalUsageAnalyzer : DiagnosticAnalyzer
{
    private const string AttributeName = "EngineInternalAttribute";
    
    private static DiagnosticDescriptor Rule => new(
        Id.EngineCoreUsageNotAllowed,
        "EngineCore usage without permission",
        "{0} {1} is internal engine {2}, usage is not allowed",
        "Usage",
        DiagnosticSeverity.Error,
        true,
        "Usage of internal engine members is not allowed unless explicitly permitted."
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterSyntaxNodeAction(AnalyzeMethodInvocation, SyntaxKind.InvocationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeFieldOrPropertyAccess, SyntaxKind.SimpleMemberAccessExpression);
        context.RegisterSyntaxNodeAction(AnalyzeConstructorConstraint, SyntaxKind.ConstructorConstraint);
    }

    private static void AnalyzeMethodInvocation(SyntaxNodeAnalysisContext context)
    {
        if (AllowedAssembly(context))
            return;
        
        var invocationExpression = (InvocationExpressionSyntax) context.Node;
        var location = invocationExpression.GetLocation();
        var symbol = ModelExtensions.GetSymbolInfo(context.SemanticModel, invocationExpression).Symbol;
        if (symbol is not IMethodSymbol methodSymbol)
            return;

        AllowedUsage(context, methodSymbol, methodSymbol, location);
        AllowedUsage(context, methodSymbol.ContainingType, methodSymbol, location);
        AllowedUsage(context, methodSymbol.ReturnType, methodSymbol, location);
        
        foreach (var parameter in methodSymbol.Parameters)
            AllowedUsage(context, parameter.Type, parameter, location);
    }

    private static void AnalyzeFieldOrPropertyAccess(SyntaxNodeAnalysisContext context)
    {
        if (AllowedAssembly(context))
            return;
        
        var memberAccess = (MemberAccessExpressionSyntax) context.Node;
        var location = memberAccess.GetLocation();
        var symbol = ModelExtensions.GetSymbolInfo(context.SemanticModel, memberAccess).Symbol;
        switch (symbol)
        {
            case IFieldSymbol fieldSymbol:
                AllowedUsage(context, fieldSymbol, fieldSymbol, location);
                AllowedUsage(context, fieldSymbol.Type, fieldSymbol, location);
                break;
            
            case IPropertySymbol propertySymbol:
                AllowedUsage(context, propertySymbol, propertySymbol, location);
                AllowedUsage(context, propertySymbol.Type, propertySymbol, location);
                AllowedUsage(context, propertySymbol.ContainingType, propertySymbol, location);
                break;
        }
    }

    private static void AnalyzeConstructorConstraint(SyntaxNodeAnalysisContext context)
    {
        if (AllowedAssembly(context))
            return;
        
        var constructorConstraint = (ConstructorConstraintSyntax) context.Node;
        var location = constructorConstraint.GetLocation();
        
        if (constructorConstraint.Parent is not TypeParameterConstraintClauseSyntax typeSyntax || typeSyntax.Parent is null)
            return;
        
        var typeSymbol = context.SemanticModel.GetDeclaredSymbol((TypeParameterSyntax) typeSyntax.Parent);
        if (typeSymbol == null)
            return;
        
        AllowedUsage(context, typeSymbol, typeSymbol, location);

        var constructors = typeSymbol
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Where(method => method.MethodKind == MethodKind.Constructor);
        
        foreach (var constructor in constructors)
        {
            AllowedUsage(context, constructor, constructor, location);

            foreach (var parameter in constructor.Parameters)
            {
                AllowedUsage(context, parameter, parameter, location);
                AllowedUsage(context, parameter.Type, parameter, location);
            }
        }
    }

    private static void AllowedUsage(SyntaxNodeAnalysisContext context, ISymbol? symbol, ISymbol? symbolMessage, Location location)
    {
        if (symbol is null || !HasAttribute(symbol))
            return;

        var kind = Enum.GetName(typeof(SymbolKind), symbolMessage?.Kind ?? symbol.Kind); 
        var diagnostic = Diagnostic.Create(Rule, location, kind, symbolMessage?.Name ?? symbol.Name, kind?.ToLower() ?? string.Empty);
        context.ReportDiagnostic(diagnostic);
    }

    private static bool AllowedAssembly(SyntaxNodeAnalysisContext context)
    {
        var attributes = context.ContainingSymbol?.ContainingAssembly.GetAttributes();
        if (attributes is null)
            return false;
        
        foreach(var data in attributes)
        {
            if (data.AttributeClass is not { } attribute)
                continue;
            
            if (attribute.Name != nameof(AssemblyMetadataAttribute))
                continue;

            var arguments = data.ConstructorArguments;
            if (arguments.Length != 2)
                continue;

            var key = (string?) arguments[0].Value;
            var value = (string?) arguments[1].Value;
            
            if (key is null || value is null)
                continue;
            
            if (key != "AllowEngineCoreUsage")
                continue;
            
            if (!bool.TryParse(value, out var result))
                continue;

            return result;
        }
        
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasAttribute(ISymbol? symbol)
    {
        return symbol?
            .GetAttributes()
            .Any(attribute => attribute.AttributeClass?.Name == AttributeName) ?? false;
    }
}