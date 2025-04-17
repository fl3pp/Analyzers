using System.Collections.Immutable;
using System.Globalization;
using fl3pp.Analyzers.LineLength;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

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

        foreach (var line in context.Node.SyntaxTree.GetText().Lines)
        {
            int trailingWhitespaceLength = 0;
            
            while (line.End != line.Start
                && TrailingWhitespaceDiagnostic.WhitespaceChars.Contains(
                    text.ToString(new(line.End - trailingWhitespaceLength - 1, 1))[0]))
            {
                trailingWhitespaceLength++;
            }

            if (trailingWhitespaceLength == 0) continue;
 
            context.ReportDiagnostic(Diagnostic.Create(
                TrailingWhitespaceDiagnostic.Descriptor,
                Location.Create(node.SyntaxTree, new(line.End - trailingWhitespaceLength, trailingWhitespaceLength)),
                line.LineNumber + 1));
        }
    }
}