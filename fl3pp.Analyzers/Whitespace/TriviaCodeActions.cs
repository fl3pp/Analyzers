using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace fl3pp.Analyzers.Whitespace;

internal static class TriviaCodeActionsPropertyKeys
{
    public const string SpanStartOverride = "SpanStartOverride";
    public const string SpanEndOverride = "SpanEndOverride";
}

internal static class TriviaCodeActions
{
    public static Func<CancellationToken, Task<Document>> RemoveDiagnosticSpansFromDocument(
        Document document,
        ImmutableArray<Diagnostic> diagnostics)
    {
        TextSpan GetSpanFromDiagnostic(Diagnostic diagnostic)
        {
            var start = diagnostic.Properties.TryGetValue(
                TriviaCodeActionsPropertyKeys.SpanStartOverride, out var startValue)
                ? int.Parse(startValue!) : diagnostic.Location.SourceSpan.Start;
            var end = diagnostic.Properties.TryGetValue(
                TriviaCodeActionsPropertyKeys.SpanEndOverride, out var endValue)
                ? int.Parse(endValue!) : diagnostic.Location.SourceSpan.End;
            return TextSpan.FromBounds(start, end);
        }
 
        return RemoveSpansFromDocument(document, diagnostics.Select(GetSpanFromDiagnostic).ToImmutableArray());
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