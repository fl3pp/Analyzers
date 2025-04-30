using Microsoft.CodeAnalysis;

namespace fl3pp.Analyzers.MSTest;

internal static class MSTestDefinitions
{
    public const string TestClassAttributeQualifiedName = "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute";
    public const string TestContextName = "TestContext";
    public const string TestContextQualifiedName = "Microsoft.VisualStudio.TestTools.UnitTesting." + TestContextName;
    
    public static bool IsTestContext(ITypeSymbol type)
    {
        return type.WithNullableAnnotation(new()).ToDisplayString() == TestContextQualifiedName;
    }
}