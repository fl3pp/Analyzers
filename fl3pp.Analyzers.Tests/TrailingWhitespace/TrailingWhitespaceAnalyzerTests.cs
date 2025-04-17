using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    fl3pp.Analyzers.TrailingWhitespace.TrailingWhitespaceAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using AnalyzerVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
    fl3pp.Analyzers.TrailingWhitespace.TrailingWhitespaceAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace fl3pp.Analyzers.Tests.TrailingWhitespace;

public sealed class TrailingWhitespaceAnalyzerTests
{
    [Fact]
    public async Task Analyze_NoSpaceAfterLine_NotReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestCode = "class Test { }";
 
        await test.RunAsync();
    }
    
    [Theory]
    [InlineData("\r")]
    [InlineData("\r\n")]
    [InlineData("\n")]
    public async Task Analyze_NoSpaceAfterLineWithSecondLineAndVariousNewlineVariants_NotReportsDiagnostic(string newline)
    {
        var test = new AnalyzerTest();
        test.TestCode = $"class Test {{ }}{newline}class Test2 {{ }}";
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_SpaceOnEmptyLine_ReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestCode = "class Test\n{ \n}";
 
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(2, 2, 2, 3).WithArguments("2"));
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_MultipleTrailingWhiteSpace_ReportsMultipleDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestCode = "class Test \n{ \n} ";
 
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(1, 11, 1, 12).WithArguments("1"));
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(2, 2, 2, 3).WithArguments("2"));
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(3, 2, 3, 3).WithArguments("3"));
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_SpaceAfterLine_ReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestCode = "class Test { } ";
 
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(1, 15, 1, 16).WithArguments("1"));
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_TabAfterLine_ReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestCode = "class Test { }\t";
        
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(1, 15, 1, 16).WithArguments("1"));
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_TwoSpacesAfterLine_ReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestCode = "class Test { }  ";
        
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(1, 15, 1, 17).WithArguments("1"));
 
        await test.RunAsync();
    }
}