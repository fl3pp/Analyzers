using TestScenario = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    fl3pp.Analyzers.Whitespace.ConsecutiveEmptyLines.ConsecutiveEmptyLines,
    fl3pp.Analyzers.Whitespace.ConsecutiveEmptyLines.RemoveConsecutiveEmptyLinesFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    fl3pp.Analyzers.Whitespace.ConsecutiveEmptyLines.ConsecutiveEmptyLines,
    fl3pp.Analyzers.Whitespace.ConsecutiveEmptyLines.RemoveConsecutiveEmptyLinesFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace fl3pp.Analyzers.Tests.Whitespace;

public sealed class ConsecutiveEmptyLinesTests
{
    [Fact]
    public async Task AnalyzeAndFix_TwoEmptyLinesInClassDeclaration_ReportsDiagnosticAndRemovesOneEmptyLine()
    {
        var test = new TestScenario();
        test.TestCode =
            """
            class Test
            {
            
            
            }
            """;

        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(3, 1, 4, 1).WithArguments("3", "4"));
        test.FixedCode =
            """
            class Test
            {
            
            }
            """;

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_ThreeEmptyLinesInClassBody_ReportsSingleDiagnosticAndRemovesTwoLines()
    {
        var test = new TestScenario();
        test.TestCode =
            """
            class Test
            {
            
            
            
            }
            """;
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(3, 1, 5, 1).WithArguments("3", "5"));
        test.FixedCode =
            """
            class Test
            {
            
            }
            """;
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_TwoEmptyLinesInBeginningOfCompilationUnit_ReportsDiagnosticAndRemovesOneLine()
    {
        var test = new TestScenario();
        test.TestCode =
            """
            
            
            class Test { }
            """;
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(1, 1, 2, 1).WithArguments("1", "2"));
        test.FixedCode =
            """
            
            class Test { }
            """;
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_EmptyLinesInMultipleLocations_RemovesSuperfluousLinesInAllLocations()
    {
        var test = new TestScenario();
        test.TestCode =
            """
            namespace test;
            
            
            class Test
            {
            
            
            }
            
            
            """;

        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(2, 1, 3, 1).WithArguments("2", "3"));
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(6, 1, 7, 1).WithArguments("6", "7"));
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(9, 1, 10, 1).WithArguments("9", "10"));
        test.FixedCode =
            """
            namespace test;
            
            class Test
            {
            
            }
            
            """;

        await test.RunAsync();
    }
}