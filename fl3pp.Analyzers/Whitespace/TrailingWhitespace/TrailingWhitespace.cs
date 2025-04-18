using System.Collections.Immutable;
using fl3pp.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace fl3pp.Analyzers.TrailingWhitespace;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class TrailingWhitespace : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(TrailingWhitespaceDiagnostic.Descriptor.Id);
    
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
    
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];
        var spanToDelete = diagnostic.Location.SourceSpan;

        context.RegisterCodeFix(CodeAction.Create(
            TrailingWhitespaceDiagnostic.TrimTrailingWhitespaceFixTitle,
            ct => TriviaCodeActions.RemoveTriviaInSpan(context.Document, spanToDelete, ct),
            equivalenceKey: TrailingWhitespaceDiagnostic.TrimTrailingWhitespaceFixTitle),
            diagnostic);
 
        return Task.CompletedTask;
    }
}