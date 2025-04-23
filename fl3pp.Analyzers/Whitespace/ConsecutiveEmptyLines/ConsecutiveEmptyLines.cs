using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace fl3pp.Analyzers.Whitespace.ConsecutiveEmptyLines;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ConsecutiveEmptyLines : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(ConsecutiveEmptyLinesDiagnostic.Descriptor);

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
 
        var emptyLines = text.Lines
            .Where(line => text.EnumerateTextCharacters(line.Span).All(t => t.Character.IsWhitespace()))
            .ToArray();

        if (emptyLines.Length == 0) return;

        Stack<List<TextLine>> possibleDiagnostics = new();
        possibleDiagnostics.Push(new([emptyLines.First()]));

        foreach (var line in emptyLines.Skip(1))
        {
            if (possibleDiagnostics.Peek().Last().LineNumber + 1 == line.LineNumber)
                possibleDiagnostics.Peek().Add(line);
            else
                possibleDiagnostics.Push(new([line]));
        }

        foreach (var diagnostic in possibleDiagnostics.Where(lines => lines.Count > 1))
        {
            var firstLine = diagnostic.First();
            var lastLine = diagnostic.Last();

            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: ConsecutiveEmptyLinesDiagnostic.Descriptor,
                location: Location.Create(
                    node.SyntaxTree,
                    TextSpan.FromBounds(
                        text.Lines[Math.Max(0, firstLine.LineNumber - 1)].Span.End,
                        lastLine.Span.End)),
                messageArgs: [firstLine.LineNumber + 1, lastLine.LineNumber + 1],
                properties: ImmutableDictionary<string, string?>.Empty
                    .Add(TriviaCodeActions.SpanStartOverrideKey, firstLine.Span.Start.ToString())
                    .Add(TriviaCodeActions.SpanEndOverrideKey, lastLine.Span.End.ToString())));
        }
    }
}