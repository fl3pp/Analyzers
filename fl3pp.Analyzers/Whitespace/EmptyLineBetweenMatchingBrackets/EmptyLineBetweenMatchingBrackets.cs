using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace fl3pp.Analyzers.Whitespace.EmptyLineBetweenMatchingBrackets;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EmptyLineBetweenMatchingBrackets : DiagnosticAnalyzer
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
                LeadingBracket = GetLeadingBracketOrNull(text.EnumerateTextCharacters(line.Span)),
                ClosingBracket = GetLeadingBracketOrNull(text.EnumerateTextCharactersReverse(line.Span)),
            })
            .ToArray();
        
        TextCharacter? GetLeadingBracketOrNull(IEnumerable<TextCharacter> characters) => characters
            .SkipWhile(textCharacter => textCharacter.Character.IsWhitespace())
            .Take(1)
            .Where(textCharacter => s_bracketChars.Contains(textCharacter.Character))
            .Cast<TextCharacter?>()
            .FirstOrDefault();
 
        foreach (var lineToCheck in bracketLines)
        {
            if (lineToCheck.ClosingBracket is null) continue;

            var nextNonEmptyLine = bracketLines
                .Skip(lineToCheck.TextLine.LineNumber + 1)
                .SkipWhile(line => text.EnumerateTextCharacters(line.TextLine.Span)
                    .All(c => c.Character.IsWhitespace()))
                .FirstOrDefault();
            
            if (nextNonEmptyLine?.LeadingBracket is null) continue;
            
            if (nextNonEmptyLine.TextLine.LineNumber - lineToCheck.TextLine.LineNumber == 1) continue;
 
            if (nextNonEmptyLine.LeadingBracket.Value.Character != lineToCheck.ClosingBracket.Value.Character) continue;
            
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor:EmptyLineBetweenMatchingBracketsDiagnostic.Descriptor,
                location:Location.Create(
                    context.Node.SyntaxTree,
                    TextSpan.FromBounds(lineToCheck.TextLine.Span.End, nextNonEmptyLine.TextLine.Span.Start)),
                properties: ImmutableDictionary<string, string?>.Empty,
                messageArgs: [lineToCheck.TextLine.LineNumber + 2, lineToCheck.ClosingBracket.Value.Character]));
        }
    }
}