using Microsoft.CodeAnalysis;

namespace fl3pp.Analyzers.Whitespace.LineLength;

internal static class LineLengthDiagnostic
{
    public const int DefaultMaxLineLength = 120;
    public const string MaxLineLengthConfigKey = "max_line_length";
    public const string GuidelinesConfigKey = "guidelines";
    
    public static DiagnosticDescriptor Descriptor { get; } = new(
        id: "FL30001",
        title: "Maximum line length exceeded",
        messageFormat: "Line {0} is {1} characters longer than allowed.",
        category: "Formatting",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: false,
        description: "Enforces a maximum line length.",
        helpLinkUri: "https://github.com/fl3pp/Analyzers#fl30001-maximum-line-length-exceeded");
}