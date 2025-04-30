using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace fl3pp.Analyzers.MSTest.MissingTestContextArgument;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingTestContextArgumentAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(MissingTestContextArgumentAnalyzerDiagnostic.UnsetTestContextParameter);
    
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeArgumentList, SyntaxKind.ArgumentList);
    }

    private void AnalyzeArgumentList(SyntaxNodeAnalysisContext context)
    {
        var node = (ArgumentListSyntax)context.Node;
        var semanticModel = context.SemanticModel;

        var targetSymbol = GetTargetMethodSymbol(node, semanticModel);
        if (targetSymbol is null) return;
 
        var testContextParameters = GetTestContextParametersOfTargetFunction(targetSymbol).ToList();
        var parametersWithArgument = GetArgumentsOfCall(node.Arguments, semanticModel).Select(a => a.Parameter!);
        
        if (testContextParameters.Except(parametersWithArgument).Any())
        {
            var namedTestContextArgumentName = targetSymbol.Parameters
                .Where(s => s.IsOptional).Skip(1).Any()
                ? testContextParameters.First().Name
                : null;

            context.ReportDiagnostic(Diagnostic.Create(
                MissingTestContextArgumentAnalyzerDiagnostic.UnsetTestContextParameter,
                node.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty
                    .Add(MissingTestContextArgumentAnalyzerDiagnostic.NamedTestContextArgumentNameProperty, namedTestContextArgumentName)));
        }
    }

    private IMethodSymbol? GetTargetMethodSymbol(SyntaxNode node, SemanticModel semanticModel)
    {
        if (node.Parent is null) return null;
        var targetFunction = semanticModel.GetSymbolInfo(node.Parent).Symbol;
        if (targetFunction is not IMethodSymbol methodSymbol) return null;
        return methodSymbol;
    }

    private IEnumerable<IParameterSymbol> GetTestContextParametersOfTargetFunction(
        IMethodSymbol methodSymbol)
    {
        var mstestContextParameters = methodSymbol.Parameters
            .Where(p => p.IsOptional)
            .Where(p => p.Type.WithNullableAnnotation(new()).ToDisplayString()
                        == MSTestDefinitions.TestContextQualifiedName);
 
        foreach (var parameter in mstestContextParameters) yield return parameter;
    }
    
    private IEnumerable<IArgumentOperation> GetArgumentsOfCall(
        IEnumerable<ArgumentSyntax> argumentNodes,
        SemanticModel semanticModel)
    {
        foreach (var argument in argumentNodes)
        {
            if (semanticModel.GetOperation(argument) is IArgumentOperation { Parameter: not null } argumentSymbol)
                yield return argumentSymbol;
        }
    }
}
