using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;

namespace fl3pp.Analyzers.TrailingWhitespace;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class TrailingWhitespaceFixer : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(TrailingWhitespaceDiagnostic.Descriptor.Id);
    
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
    
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];
        var spanToDelete = diagnostic.Location.SourceSpan;

        var editor = await DocumentEditor.CreateAsync(context.Document, context.CancellationToken);
        
        var triviaGroups = editor.OriginalRoot
            .DescendantTrivia(spanToDelete)
            .Select(trivia => trivia.Token)
            .Distinct()
            .GroupBy(token => token.Parent)
            .ToArray();

        foreach (var triviaGroup in triviaGroups)
        {
            var node = triviaGroup.Key!;
            
            SyntaxTriviaList ClearAffectedSpan(SyntaxTriviaList original)
            {
                return SyntaxFactory.TriviaList(original.Select(trivia =>
                {
                    var overlapWithDiagnostic = trivia.Span.Overlap(spanToDelete) ?? new TextSpan();
                    
                    var newSpanStart = trivia.Span.Start;
                    var newSpanEnd = trivia.Span.End;

                    if (overlapWithDiagnostic.Start == trivia.Span.Start)
                    {
                        newSpanStart = overlapWithDiagnostic.End;
                    }
                    else if (overlapWithDiagnostic.End == trivia.Span.End)
                    {
                        newSpanEnd = overlapWithDiagnostic.Start;
                    }

                    var newSpan = TextSpan.FromBounds(newSpanStart, newSpanEnd);
                    
                    return SyntaxFactory.SyntaxTrivia(
                        trivia.Kind(),
                        text: editor.OriginalRoot.SyntaxTree.GetText().ToString(newSpan));
                }));
            }

            var replacements = triviaGroup
                .Select(original => original
                    .WithLeadingTrivia(ClearAffectedSpan(original.LeadingTrivia))
                    .WithTrailingTrivia(ClearAffectedSpan(original.TrailingTrivia)))
                .Zip(triviaGroup, (newToken, oldToken) => new { Old = oldToken, New = newToken });
 
            editor.ReplaceNode(node, replacements.Aggregate(node, (cur, replacement) =>
                cur.ReplaceToken(replacement.Old, replacement.New)));
        }
        
        context.RegisterCodeFix(CodeAction.Create(
            "Remove trailing whitespace",
            _ => Task.FromResult(editor.GetChangedDocument())),
            diagnostic);
    }
}