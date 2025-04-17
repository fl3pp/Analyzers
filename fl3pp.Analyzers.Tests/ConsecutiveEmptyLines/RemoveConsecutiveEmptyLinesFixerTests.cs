using CodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    fl3pp.Analyzers.ConsecutiveEmptyLines.ConsecutiveEmptyLinesAnalyzer,
    fl3pp.Analyzers.ConsecutiveEmptyLines.RemoveConsecutiveEmptyLinesFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using CodeFixVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    fl3pp.Analyzers.ConsecutiveEmptyLines.ConsecutiveEmptyLinesAnalyzer,
    fl3pp.Analyzers.ConsecutiveEmptyLines.RemoveConsecutiveEmptyLinesFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace fl3pp.Analyzers.Tests.ConsecutiveEmptyLines;

public sealed class RemoveConsecutiveEmptyLinesFixerTests
{
    [Fact]
    public async Task CodeFix_TwoEmptyLinesInClassDeclaration_RemovesOneEmptyLine()
    {
        var test = new CodeFixTest();
        test.TestCode =
            """
            class Test
            {
            
            
            }
            """;

        test.ExpectedDiagnostics.Add(CodeFixVerifier.Diagnostic()
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
    public async Task CodeFix_TwoEmptyLinesWithWhitespaceDeclaration_RemovesOneEmptyLine()
    {
        var test = new CodeFixTest();
        test.TestCode =
            """
            class Test
            {
               
               
            }
            """;

        test.ExpectedDiagnostics.Add(CodeFixVerifier.Diagnostic()
            .WithSpan(3, 1, 4, 4).WithArguments("3", "4"));
        test.FixedCode =
            """
            class Test
            {
            
            }
            """;

        await test.RunAsync();
    }
    
    [Fact]
    public async Task CodeFix_EmptyLinesInMultipleLocations_RemovesSuperfluousLinesInAllLocations()
    {
        var test = new CodeFixTest();
        test.TestCode =
            """
            namespace test;
            
            
            class Test
            {
            
            
            }
            
            
            """;

        test.ExpectedDiagnostics.Add(CodeFixVerifier.Diagnostic()
            .WithSpan(2, 1, 3, 1).WithArguments("2", "3"));
        test.ExpectedDiagnostics.Add(CodeFixVerifier.Diagnostic()
            .WithSpan(6, 1, 7, 1).WithArguments("6", "7"));
        test.ExpectedDiagnostics.Add(CodeFixVerifier.Diagnostic()
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