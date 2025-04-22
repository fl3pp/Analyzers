using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace fl3pp.Analyzers.Whitespace.TrailingWhitespace;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class RemoveTrailingWhitespace : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(TrailingWhitespaceDiagnostic.Descriptor.Id);
    
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
 
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        context.RegisterCodeFix(CodeAction.Create(
            TrailingWhitespaceDiagnostic.TrimTrailingWhitespaceFixTitle,
            TriviaCodeActions.RemoveDiagnosticSpansFromDocument(context.Document, context.Diagnostics),
            equivalenceKey: TrailingWhitespaceDiagnostic.TrimTrailingWhitespaceFixTitle),
            context.Diagnostics);
 
        return Task.CompletedTask;
    }
}