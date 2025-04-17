using System.Collections.Immutable;
using fl3pp.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace fl3pp.Analyzers.EmptyLineBetweenMatchingBrackets;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EmptyLineBetweenMatchingBracketsAnalyzer : DiagnosticAnalyzer
{
    private static readonly char[] s_bracketChars = [ '(', ')', '{', '}', '[', ']', '<', '>' ];
 
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(EmptyLineBetweenMatchingBracketsDiagnostic.Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeCompilationUnit, SyntaxKind.CompilationUnit);
    }

    private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
    {
        var text = context.Node.SyntaxTree.GetText();

        var bracketLines = text.Lines
            .Select(line => new
            {
                TextLine = line,
                StartingBracket = GetFirstBracket(text.EnumerateTextCharacters(line.Span)),
                EndingBracket = GetFirstBracket(text.EnumerateTextCharactersReverse(line.Span)),
            })
            .ToArray();
        
        TextCharacter? GetFirstBracket(IEnumerable<TextCharacter> characters) => characters
            .SkipWhile(textCharacter => textCharacter.Character.IsWhitespace())
            .Where(textCharacter => s_bracketChars.Contains(textCharacter.Character))
            .Cast<TextCharacter?>()
            .FirstOrDefault();
 
        foreach (var lineToCheck in bracketLines)
        {
            if (lineToCheck.EndingBracket is null) continue;

            var nextNonEmptyLine = bracketLines
                .Skip(lineToCheck.TextLine.LineNumber + 1)
                .SkipWhile(line => text.EnumerateTextCharacters(line.TextLine.Span)
                    .All(c => c.Character.IsWhitespace()))
                .FirstOrDefault();
            
            if (nextNonEmptyLine?.StartingBracket is null) continue;
 
            if (nextNonEmptyLine.StartingBracket.Value.Character != lineToCheck.EndingBracket.Value.Character) continue;
            
            context.ReportDiagnostic(Diagnostic.Create(
                EmptyLineBetweenMatchingBracketsDiagnostic.Descriptor,
                Location.Create(
                    context.Node.SyntaxTree,
                    TextSpan.FromBounds(
                        lineToCheck.TextLine.Span.End,
                        nextNonEmptyLine.TextLine.Span.Start)),
                lineToCheck.TextLine.LineNumber + 2,
                lineToCheck.EndingBracket.Value.Character));
        }
    }
}