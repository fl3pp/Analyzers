using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;

namespace fl3pp.Analyzers.Helpers;

internal static class TriviaCodeActions
{
    public static async Task<Document> RemoveTriviaInSpan(Document current, TextSpan spanToDelete, CancellationToken ct)
    {
        var editor = await DocumentEditor.CreateAsync(current, ct);
        
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

        return editor.GetChangedDocument();
    }
}