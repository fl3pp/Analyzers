using Microsoft.CodeAnalysis;
using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    fl3pp.Analyzers.LineLength.LineLengthAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using AnalyzerVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
    fl3pp.Analyzers.LineLength.LineLengthAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace fl3pp.Analyzers.Tests.LineLength;

public sealed class LineLengthAnalyzerTest
{
    [Fact]
    public async Task Analyze_NoMaxLengthAnd120LongLine_NotReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestCode =
            """
            class Test
            {
            //----------------------------------------------------------------------------------------------------------------------
            }
            """;
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_NoMaxLengthAnd121LongLine_ReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestCode =
            """
            class Test
            {
            //-----------------------------------------------------------------------------------------------------------------------
            }
            """;
        
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(3, 121, 3, 122).WithArguments("3", "1"));
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_MaxLengthOf200And200LongLine_NotReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig",
            """
            [*.cs]
            max_line_length = 200
            """));
        test.TestCode =
            """
            class Test
            {
            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            }
            """;
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_MaxLengthOf100And101LongLine_ReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig",
            """
            [*.cs]
            max_line_length = 100
            """));
        test.TestCode =
            """
            class Test
            {
            //---------------------------------------------------------------------------------------------------
            }
            """;
        
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(3, 101, 3, 102).WithArguments("3", "1"));
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_GuidelineOf200And200LongLine_NotReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig",
            """
            [*.cs]
            guidelines = 200
            """));
        test.TestCode =
            """
            class Test
            {
            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            }
            """;
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_GuidelineMaxLengthOf100And101LongLine_ReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig",
            """
            [*.cs]
            guidelines = 100
            """));
        test.TestCode =
            """
            class Test
            {
            //---------------------------------------------------------------------------------------------------
            }
            """;
        
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(3, 101, 3, 102).WithArguments("3", "1"));
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_GuidelineMaxLengthOf20AndMaxLineLengthOf100And100LongLine_NotReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig",
            """
            [*.cs]
            guidelines = 100
            """));
        test.TestCode =
            """
            class Test
            {
            //--------------------------------------------------------------------------------------------------
            }
            """;
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_GuidelineMaxLengthOf100AndMaxLineLengthOf20And20LongLine_NotReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig",
            """
            [*.cs]
            guidelines = 100
            """));
        test.TestCode =
            """
            class Test
            {
            //------------------
            }
            """;
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_MaxLengthSetForDifferentFile_NotReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig",
            """
            [test/*.cs]
            guidelines = 1
            """));
        test.TestCode =
            """
            class Test
            {
            //------------------
            }
            """;
 
        await test.RunAsync();
    }
    
    [Fact]
    public async Task Analyze_MaxLengthSetToNegativeValueAnd121LongLing_ReportsDiagnostic()
    {
        var test = new AnalyzerTest();
        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig",
            """
            [*.cs]
            max_line_length = -1
            """));
        test.TestCode =
            """
            class Test
            {
            //-----------------------------------------------------------------------------------------------------------------------
            }
            """;
        
        test.ExpectedDiagnostics.Add(AnalyzerVerifier.Diagnostic()
            .WithSpan(3, 121, 3, 122).WithArguments("3", "1"));
 
        await test.RunAsync();
    }
}