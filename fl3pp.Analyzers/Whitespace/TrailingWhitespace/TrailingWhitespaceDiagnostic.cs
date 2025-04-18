using Microsoft.CodeAnalysis;

namespace fl3pp.Analyzers.TrailingWhitespace;

internal static class TrailingWhitespaceDiagnostic
{
    public static DiagnosticDescriptor Descriptor { get; } = new(
        id: "FL30002",
        title: "Trailing whitespace",
        messageFormat: "Line {0} contains trailing whitespace.",
        category: "Formatting",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: false,
        description: "Ensures that lines do not have trailing whitespace.",
        helpLinkUri: "https://github.com/fl3pp/Analyzers#fl30002-trailing-whitespace");
    
    public const string TrimTrailingWhitespaceFixTitle = "Trim trailing whitespace";
}