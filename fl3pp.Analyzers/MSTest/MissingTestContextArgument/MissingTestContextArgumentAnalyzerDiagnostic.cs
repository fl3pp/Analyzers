using Microsoft.CodeAnalysis;

namespace fl3pp.Analyzers.MSTest.MissingTestContextArgument;

public static class MissingTestContextArgumentAnalyzerDiagnostic
{
    public const string NamedTestContextArgumentNameProperty = nameof(NamedTestContextArgumentNameProperty);
 
    public const string AddTestContextArgumentFixTitle = "Add TestContext argument";
    
    public static DiagnosticDescriptor UnsetTestContextParameter { get; } = new(
        id: "FL30005",
        title: "Missing optional TestContext argument.",
        messageFormat: "An optional MSTest 'TestContext' argument is missing.",
        category: "MSTest",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: false,
        helpLinkUri: "https://github.com/fl3pp/Analyzers#fl30005-missing-optional-mstestcontext-argument");
}