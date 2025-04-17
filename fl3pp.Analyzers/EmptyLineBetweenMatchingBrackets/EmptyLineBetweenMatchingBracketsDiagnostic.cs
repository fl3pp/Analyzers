using Microsoft.CodeAnalysis;

namespace fl3pp.Analyzers.EmptyLineBetweenMatchingBrackets;

internal static class EmptyLineBetweenMatchingBracketsDiagnostic
{
    public static DiagnosticDescriptor Descriptor { get; } = new(
        id: "FL30004",
        title: "Empty lines between matching consecutive braces",
        messageFormat: "Line {0} is empty between matching consecutive '{1}' braces.",
        category: "Formatting",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: false,
        description: "Ensure there is an empty line between matching braces.",
        helpLinkUri: "https://github.com/fl3pp/Analyzers#fl30004-empty-line-between-matching-consecutive-braces");
}