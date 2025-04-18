using System.Collections.Immutable;
using fl3pp.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace fl3pp.Analyzers.ConsecutiveEmptyLines;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class RemoveConsecutiveEmptyLinesFixer : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(ConsecutiveEmptyLinesDiagnostic.Descriptor.Id);
    
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
    
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];
        var spanToDelete = diagnostic.Location.SourceSpan;
        
        context.RegisterCodeFix(CodeAction.Create(
            ConsecutiveEmptyLinesDiagnostic.RemoveConsecutiveEmptyLinesFixTitle,
            async ct =>
            {
                var withoutEmptyLines = await TriviaCodeActions.RemoveTriviaInSpan(context.Document, spanToDelete, ct);
                return withoutEmptyLines;
            },
            ConsecutiveEmptyLinesDiagnostic.RemoveConsecutiveEmptyLinesFixTitle),
            diagnostic);

        return Task.CompletedTask;
    }
}