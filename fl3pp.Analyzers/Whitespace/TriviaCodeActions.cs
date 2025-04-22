using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace fl3pp.Analyzers.Whitespace;

internal static class TriviaCodeActions
{
    public static Func<CancellationToken, Task<Document>> RemoveDiagnosticSpansFromDocument(
        Document document,
        ImmutableArray<Diagnostic> diagnostics)
    {
        return RemoveSpansFromDocument(document, diagnostics.Select(d => d.Location.SourceSpan).ToImmutableArray());
    }
    
    public static Func<CancellationToken, Task<Document>> RemoveSpansFromDocument(
        Document document,
        ImmutableArray<TextSpan> spans) => async ct =>
    {
        var text = await document.GetTextAsync(ct).ConfigureAwait(false);
        var newText = text.WithChanges(spans.Select(s => new TextChange(s, string.Empty)));
        return document.WithText(newText);
    };
}