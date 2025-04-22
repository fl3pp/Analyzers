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
        var diagnostic = context.Diagnostics[0];
        var spanToDelete = diagnostic.Location.SourceSpan;
        
        context.RegisterCodeFix(CodeAction.Create(
            EmptyLineBetweenMatchingBracketsDiagnostic.RemoveEmptyLineBetweenMatchingBracketsFixTitle,
            async ct =>
            {
                var withoutEmptyLines = await TriviaCodeActions.RemoveTriviaInSpan(context.Document, spanToDelete, ct);
                return withoutEmptyLines;
            },
            EmptyLineBetweenMatchingBracketsDiagnostic.RemoveEmptyLineBetweenMatchingBracketsFixTitle),
            diagnostic);

        return Task.CompletedTask;
    }
    
    public const string RemoveConsecutiveEmptyLinesFixTitle = "Remove empty lines between matching brackets";
}