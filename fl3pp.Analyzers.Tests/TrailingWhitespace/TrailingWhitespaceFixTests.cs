using CodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    fl3pp.Analyzers.TrailingWhitespace.TrailingWhitespaceAnalyzer,
    fl3pp.Analyzers.TrailingWhitespace.TrailingWhitespaceFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using CodeFixVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    fl3pp.Analyzers.TrailingWhitespace.TrailingWhitespaceAnalyzer,
    fl3pp.Analyzers.TrailingWhitespace.TrailingWhitespaceFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace fl3pp.Analyzers.Tests.TrailingWhitespace;

public sealed class TrailingWhitespaceFixTests
{
    [Fact]
    public async Task CodeFix_WhitespaceAfterSingleLineClassDeclaration_RemovesWhitespace()
    {
        var test = new CodeFixTest();
        test.TestCode = "class Test { } ";
        
        test.ExpectedDiagnostics.Add(CodeFixVerifier.Diagnostic()
            .WithSpan(1, 15, 1, 16).WithArguments("1"));
        test.FixedCode = "class Test { }";

        await test.RunAsync();
    }
    
    [Fact]
    public async Task CodeFix_WhitespaceAfterMultiLineClassDeclaration_RemovesWhitespace()
    {
        var test = new CodeFixTest();
        test.TestCode =
            """
            class Test
            {
            }   
            """;
        
        test.ExpectedDiagnostics.Add(CodeFixVerifier.Diagnostic()
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
    public async Task CodeFix_WhitespaceOnEmptyLineBetweenNamespaceStatementAndClass_RemovesWhitespace()
    {
        var test = new CodeFixTest();
        test.TestCode =
            """
            namespace Test;
               
            class Test { }
            """;
        
        test.ExpectedDiagnostics.Add(CodeFixVerifier.Diagnostic()
            .WithSpan(2, 1, 2, 4).WithArguments("2"));
        test.FixedCode =
            """
            namespace Test;
            
            class Test { }
            """;

        await test.RunAsync();
    }
}