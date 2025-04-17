using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    fl3pp.Analyzers.ConsecutiveEmptyLines.ConsecutiveEmptyLinesAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using AnalyzerVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
    fl3pp.Analyzers.ConsecutiveEmptyLines.ConsecutiveEmptyLinesAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace fl3pp.Analyzers.Tests.ConsecutiveEmptyLines;

public sealed class ConsecutiveEmptyLinesAnalyzerTests
{
    [Fact]
    public async Task Analyze_TwoEmptyLinesInClassBody_ReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestCode =
            """
            class Test
            {
            
            
            }
            """;
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(3, 1, 4, 1).WithArguments("3", "4"));
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_TwoEmptyLinesWithWhitespaceInClassBody_ReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestCode =
            """
            class Test
            {
                
                
            }
            """;
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(3, 1, 4, 5).WithArguments("3", "4"));
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_ThreeEmptyLinesInClassBody_ReportsSingleDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestCode =
            """
            class Test
            {
            
            
            
            }
            """;
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(3, 1, 5, 1).WithArguments("3", "5"));
 
        await test.RunAsync();
    }
}