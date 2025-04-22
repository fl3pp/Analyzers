using TestScenario = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    fl3pp.Analyzers.Whitespace.TrailingWhitespace.TrailingWhitespaceAnalyzer,
    fl3pp.Analyzers.Whitespace.TrailingWhitespace.TrailingWhitespace,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    fl3pp.Analyzers.Whitespace.TrailingWhitespace.TrailingWhitespaceAnalyzer,
    fl3pp.Analyzers.Whitespace.TrailingWhitespace.TrailingWhitespace,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace fl3pp.Analyzers.Tests.Whitespace;

public sealed class TrailingWhitespaceTests
{
    [Fact]
    public async Task AnalyzeAndFix_NoSpaceAfterLine_NotReportsDiagnostic()
    {
        var test = new TestScenario();
        test.TestCode = "class Test { }";
 
        await test.RunAsync();
    }
    
    [Theory]
    [InlineData("\r")]
    [InlineData("\r\n")]
    [InlineData("\n")]
    public async Task AnalyzeAndFix_NoSpaceAfterLineWithSecondLineAndVariousNewlineVariants_NotReportsDiagnostic(
        string newline)
    {
        var test = new TestScenario();
        test.TestCode = $"class Test {{ }}{newline}class Test2 {{ }}";
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_WhitespaceAfterSingleLineClassDeclaration_ReportsDiagnosticAndRemovesWhitespace()
    {
        var test = new TestScenario();
        test.TestCode = "class Test { } ";
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(1, 15, 1, 16).WithArguments("1"));
        test.FixedCode = "class Test { }";

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_TwoWhitespacesAfterSingleLineClassDeclaration_ReportsDiagnosticAndRemovesWhitespace()
    {
        var test = new TestScenario();
        test.TestCode = "class Test { }  ";
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(1, 15, 1, 17).WithArguments("1"));
        test.FixedCode = "class Test { }";
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_TabAfterSingleLineClassDeclaration_ReportsDiagnosticAndRemovesTab()
    {
        var test = new TestScenario();
        test.TestCode = "class Test { }\t";
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(1, 15, 1, 16).WithArguments("1"));
        test.FixedCode = "class Test { }";

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_SpaceOnEmptyLine_ReportsDiagnosticAndRemovesWhitespace()
    {
        var test = new TestScenario();
        test.TestCode = "class Test\n{ \n}";
 
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(2, 2, 2, 3).WithArguments("2"));
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_WhitespaceAfterMultiLineClassDeclaration_ReportsDiagnosticAndRemovesWhitespace()
    {
        var test = new TestScenario();
        test.TestCode =
            """
            class Test
            {
            }   
            """;
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(3, 2, 3, 5).WithArguments("3"));
        test.FixedCode =
            """
            class Test
            {
            }
            """;

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_TrailingWhitespaceOnMultipleLines_ReportsMultipleDiagnosticsAndRemovesWhitespaceInBatchFix()
    {
        var test = new TestScenario();
        test.TestCode =
            """
            class Test 
            { 
            } 
            """;
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(1, 11, 1, 12).WithArguments("1"));
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(2, 2, 2, 3).WithArguments("2"));
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(3, 2, 3, 3).WithArguments("3"));
        test.FixedCode =
            """
            class Test
            {
            }
            """;

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_WhitespaceOnEmptyLineBetweenNamespaceStatementAndClass_ReportsDiagnosticAndRemovesWhitespace()
    {
        var test = new TestScenario();
        test.TestCode =
            """
            namespace Test;
               
            class Test { }
            """;
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(2, 1, 2, 4).WithArguments("2"));
        test.FixedCode =
            """
            namespace Test;
            
            class Test { }
            """;

        await test.RunAsync();
    }
}