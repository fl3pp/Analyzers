using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace fl3pp.Analyzers.Whitespace.EmptyLineBetweenMatchingBrackets;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class RemoveEmptyLineBetweenMatchingBracketsFixer : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(EmptyLineBetweenMatchingBracketsDiagnostic.Descriptor.Id);
    
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
    
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        context.RegisterCodeFix(CodeAction.Create(
            EmptyLineBetweenMatchingBracketsDiagnostic.RemoveEmptyLineBetweenMatchingBracketsFixTitle,
            TriviaCodeActions.RemoveSpansFromDocument(context.Document, context.Diagnostics
                .Select(diagnostic => diagnostic.Location.SourceSpan)
                .ToImmutableArray()),
            EmptyLineBetweenMatchingBracketsDiagnostic.RemoveEmptyLineBetweenMatchingBracketsFixTitle),
            context.Diagnostics);

        return Task.CompletedTask;
    }
}