using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace fl3pp.Analyzers.Whitespace.ConsecutiveEmptyLines;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class RemoveConsecutiveEmptyLinesFixer : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(ConsecutiveEmptyLinesDiagnostic.Descriptor.Id);
    
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
    
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        context.RegisterCodeFix(CodeAction.Create(
            ConsecutiveEmptyLinesDiagnostic.RemoveConsecutiveEmptyLinesFixTitle,
            TriviaCodeActions.RemoveDiagnosticSpansFromDocument(context.Document, context.Diagnostics),
            ConsecutiveEmptyLinesDiagnostic.RemoveConsecutiveEmptyLinesFixTitle),
            context.Diagnostics);

        return Task.CompletedTask;
    }
}