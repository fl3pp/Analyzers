using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    fl3pp.Analyzers.EmptyLineBetweenMatchingBrackets.EmptyLineBetweenMatchingBracketsAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using AnalyzerVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
    fl3pp.Analyzers.EmptyLineBetweenMatchingBrackets.EmptyLineBetweenMatchingBracketsAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace fl3pp.Analyzers.Tests.EmptyLineBetweenMatchingBrackets;

public sealed class EmptyLineBetweenMatchingBracketsAnalyzerTests
{
    [Fact]
    public async Task Analyze_EmptyLineBetweenClassOpeningAndClosingBrace_NotReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestCode =
            """
            class Test
            {
            
            }
            """;
        
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_EmptyLineBetweenNamespaceAndClassClosingBraces_ReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestCode =
            """
            namespace TestNamespace
            {
                class TestClass
                {
                
                }
                
            }
            """;
        
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(6, 6, 8, 1).WithArguments("7", "}"));
        
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_TwoEmptyLineBetweenNamespaceAndClassClosingBraces_ReportsSingleDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestCode =
            """
            namespace TestNamespace
            {
                class TestClass
                {
                
                }
                
                
            }
            """;
        
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(6, 6, 9, 1).WithArguments("7", "}"));
        
        await test.RunAsync();
    }
}