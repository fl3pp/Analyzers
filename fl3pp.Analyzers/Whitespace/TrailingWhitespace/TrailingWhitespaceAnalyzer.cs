using System.Collections.Immutable;
using System.Globalization;
using fl3pp.Analyzers.Helpers;
using fl3pp.Analyzers.LineLength;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace fl3pp.Analyzers.TrailingWhitespace;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TrailingWhitespaceAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(TrailingWhitespaceDiagnostic.Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeCompilationUnit, SyntaxKind.CompilationUnit);
    }

    private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
    {
        var node = (CompilationUnitSyntax)context.Node;

        var text = context.Node.SyntaxTree.GetText();

        foreach (var line in text.Lines)
        {
            if (line.Span.Length == 0) continue;
            
            var endOfLine = text.EnumerateTextCharactersReverse(line.Span).First();
            var endOfTrailingWhitespace = endOfLine;
            
            if (!endOfLine.Character.IsWhitespace()) continue;
            
            while (
                endOfTrailingWhitespace.Previous() is { } nextEndOfTrailingWhitespace
                && nextEndOfTrailingWhitespace.Index >= line.Span.Start
                && nextEndOfTrailingWhitespace.Character.IsWhitespace())
            {
                endOfTrailingWhitespace = nextEndOfTrailingWhitespace;
            }
 
            context.ReportDiagnostic(Diagnostic.Create(
                TrailingWhitespaceDiagnostic.Descriptor,
                Location.Create(node.SyntaxTree, TextSpan.FromBounds(endOfTrailingWhitespace.Index, line.Span.End)),
                line.LineNumber + 1));
        }
    }
}