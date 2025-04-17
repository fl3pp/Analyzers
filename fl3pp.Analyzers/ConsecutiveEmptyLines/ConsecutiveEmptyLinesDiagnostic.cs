using Microsoft.CodeAnalysis;

namespace fl3pp.Analyzers.ConsecutiveEmptyLines;

internal static class ConsecutiveEmptyLinesDiagnostic
{
    public static DiagnosticDescriptor Descriptor { get; } = new(
        id: "FL30003",
        title: "Consecutive empty lines",
        messageFormat: "Lines {0} to {1} are empty.",
        category: "Formatting",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: false,
        description: "Enforces that no two consecutive lines are empty.",
        helpLinkUri: "https://github.com/fl3pp/Analyzers#fl30003-consecutive-empty-lines");
    
    public const string RemoveConsecutiveEmptyLinesFixTitle = "Remove consecutive empty lines";
}