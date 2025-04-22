using TestScenario = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    fl3pp.Analyzers.Whitespace.EmptyLineBetweenMatchingBrackets.EmptyLineBetweenMatchingBrackets,
    fl3pp.Analyzers.Whitespace.EmptyLineBetweenMatchingBrackets.RemoveEmptyLineBetweenMatchingBracketsFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    fl3pp.Analyzers.Whitespace.EmptyLineBetweenMatchingBrackets.EmptyLineBetweenMatchingBrackets,
    fl3pp.Analyzers.Whitespace.EmptyLineBetweenMatchingBrackets.RemoveEmptyLineBetweenMatchingBracketsFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace fl3pp.Analyzers.Tests.Whitespace;

public sealed class EmptyLineBetweenMatchingBracketsTests
{
    [Fact]
    public async Task AnalyzeAndFix_EmptyLineBetweenClassOpeningAndClosingBrace_NotReportsDiagnostic()
    {
        var test = new TestScenario();
        test.TestCode =
            """
            class Test
            {
            
            }
            """;
        
        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_NoEmptyLineBetweenNamespaceAndClassClosingBraces_NotReportsDiagnostic()
    {
        var test = new TestScenario();
        test.TestCode =
            """
            namespace TestNamespace
            {
                class TestClass { }
            }
            """;
        
        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_EmptyLineBetweenNamespaceAndClassClosingBraces_ReportsDiagnosticAndRemovesLine()
    {
        var test = new TestScenario();
        test.TestCode =
            """
            namespace TestNamespace
            {
                class TestClass
                {
                
                }
                
            }
            """;
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(6, 6, 8, 1).WithArguments("7", "}"));
        test.FixedCode =
            """
            namespace TestNamespace
            {
                class TestClass
                {
                
                }
            }
            """;
        
        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_TwoEmptyLineBetweenNamespaceAndClassClosingBraces_ReportsSingleDiagnosticAndRemovesTwoLines()
    {
        var test = new TestScenario();
        test.TestCode =
            """
            namespace TestNamespace
            {
                class TestClass
                {
                
                }
                
                
            }
            """;
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(6, 6, 9, 1).WithArguments("7", "}"));
        test.FixedCode =
            """
            namespace TestNamespace
            {
                class TestClass
                {
                
                }
            }
            """;
        
        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_EmptyLineBetweenClosingParenthesis_ReportsDiagnosticAndRemovesLine()
    {
        var test = new TestScenario();
        test.TestCode =
            """
            class test
            {
                int a = ((1)
                
                );
            }
            """;
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(3, 17, 5, 1).WithArguments("4", ")"));
        test.FixedCode =
            """
            class test
            {
                int a = ((1)
                );
            }
            """;
        
        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_EmptyLineBetweenCoEndOfCodeBlockAndMethodEnd_ReportsDiagnosticAndRemovesLine()
    {
        var test = new TestScenario();
        test.TestCode =
            """
            class test
            {
                void Method()
                {
                    {
                    }
                    
                }
            }
            """;
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan(6, 10, 8, 1).WithArguments("7", "}"));
        test.FixedCode =
            """
            class test
            {
                void Method()
                {
                    {
                    }
                }
            }
            """;
        
        await test.RunAsync();
    }
}