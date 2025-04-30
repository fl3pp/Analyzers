using CodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    fl3pp.Analyzers.MSTest.MissingTestContextArgument.MissingTestContextArgumentAnalyzer,
    fl3pp.Analyzers.MSTest.MissingTestContextArgument.MissingTestContextArgumentFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    fl3pp.Analyzers.MSTest.MissingTestContextArgument.MissingTestContextArgumentAnalyzer,
    fl3pp.Analyzers.MSTest.MissingTestContextArgument.MissingTestContextArgumentFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace fl3pp.Analyzers.Tests.MSTest.MissingTestContextArgument;

public sealed class MissingTestContextArgumentFixerTests
{
    [Fact]
    public async Task AnalyzeAndFix_CallToStaticMethodWithExistingTestContextProperty_AddsArgumentToCall()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                [TestClass]
                public class TestClass
                {
                    public required TestContext TestContext { get; set; }

                    [TestMethod]
                    public void Test01()
                    {
                        var helper = TestHelper.New();
                    }
                }
                """);
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 11, 36, 11, 38));
        test.FixedState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                [TestClass]
                public class TestClass
                {
                    public required TestContext TestContext { get; set; }

                    [TestMethod]
                    public void Test01()
                    {
                        var helper = TestHelper.New(TestContext);
                    }
                }
                """);

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToStaticMethodWithExistingTestContextField_AddsArgumentToCall()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                [TestClass]
                public class TestClass
                {
                    private readonly TestContext _testContext;
                    
                    public TestClass(TestContext testContext)
                    {
                        _testContext = testContext;
                    }

                    [TestMethod]
                    public void Test01()
                    {
                        var helper = TestHelper.New();
                    }
                }
                """);
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 16, 36, 16, 38));
        test.FixedState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                [TestClass]
                public class TestClass
                {
                    private readonly TestContext _testContext;
                    
                    public TestClass(TestContext testContext)
                    {
                        _testContext = testContext;
                    }

                    [TestMethod]
                    public void Test01()
                    {
                        var helper = TestHelper.New(_testContext);
                    }
                }
                """);

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToStaticMethodWithoutOptionalTestContext_ReportsDiagnosticAndAddsArgumentToCallAndPropertyToClass()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
 
                [TestClass]
                public class TestClass
                {
                    [TestMethod]
                    public void Test01()
                    {
                        var helper = TestHelper.New();
                    }
                }
                """);
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 9, 36, 9, 38));
        test.FixedState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                
                [TestClass]
                public class TestClass
                {
                    public required TestContext TestContext { get; set; }
 
                    [TestMethod]
                    public void Test01()
                    {
                        var helper = TestHelper.New(TestContext);
                    }
                }
                """);

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToInstanceMethodWithoutOptionalTestContext_ReportsDiagnosticAndAddsArgumentToCallAndPropertyToClass()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                [TestClass]
                public class TestClass
                {
                    [TestMethod]
                    public void Test01()
                    {
                        var helper = TestHelper.New(new());
                        helper.SetTestContext();
                    }
                }
                """);
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 10, 30, 10, 32));
        test.FixedState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                [TestClass]
                public class TestClass
                {
                    public required TestContext TestContext { get; set; }

                    [TestMethod]
                    public void Test01()
                    {
                        var helper = TestHelper.New(new());
                        helper.SetTestContext(TestContext);
                    }
                }
                """);

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToPrivateInstanceMethodWithoutOptionalTestContext_ReportsDiagnosticAndAddsArgumentToCallAndPropertyToClass()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                [TestClass]
                public class TestClass
                {
                    [TestMethod]
                    public void Test01()
                    {
                        HelperMethod();
                    }
                    
                    private static void HelperMethod(TestContext? context = null) { }
                }
                """);
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 9, 21, 9, 23));
        test.FixedState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                [TestClass]
                public class TestClass
                {
                    public required TestContext TestContext { get; set; }

                    [TestMethod]
                    public void Test01()
                    {
                        HelperMethod(TestContext);
                    }
                    
                    private static void HelperMethod(TestContext? context = null) { }
                }
                """);

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToConstructorWithoutOptionalTestContext_ReportsDiagnosticAndAddsArgumentToCallAndPropertyToClass()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                [TestClass]
                public class TestClass
                {
                    [TestMethod]
                    public void Test01()
                    {
                        var helper = new TestHelper();
                    }
                }
                """);
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 9, 36, 9, 38));
        test.FixedState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                [TestClass]
                public class TestClass
                {
                    public required TestContext TestContext { get; set; }

                    [TestMethod]
                    public void Test01()
                    {
                        var helper = new TestHelper(TestContext);
                    }
                }
                """);

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToStaticMethodWithoutTestContextInNestedExpressionWithTrivia_ReportsDiagnosticAndAddsArgumentToCallAndRetainsTrivia()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
 
                [TestClass]
                public class TestClass
                {
                    public required TestContext TestContext { get; set; }
 
                    [TestMethod]
                    public void Test01()
                    {
                        var helper = 1 + CalculateSomething(
                            // This is a comment
                            2);
                    }
                    
                    private static int CalculateSomething(int one, TestContext? testContext = null)
                    {
                        return one + 2;
                    }
                }
                """);
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 11, 44, 13, 15));
        test.FixedState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                
                [TestClass]
                public class TestClass
                {
                    public required TestContext TestContext { get; set; }
 
                    [TestMethod]
                    public void Test01()
                    {
                        var helper = 1 + CalculateSomething(
                            // This is a comment
                            2, TestContext);
                    }
                    
                    private static int CalculateSomething(int one, TestContext? testContext = null)
                    {
                        return one + 2;
                    }
                }
                """);

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToStaticMethodWithMultipleUnsetParameters_AddsNamedArgumentsToCall()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
 
                [TestClass]
                public class TestClass
                {
                    [TestMethod]
                    public void Test01()
                    {
                        var helper = CalculateSomething(2);
                    }
                    
                    private static int CalculateSomething(
                        int one,
                        int? two = null,
                        TestContext? testContext = null,
                        int? three = null)
                    {
                        return one + 2;
                    }
                }
                """);
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 9, 40, 9, 43));
        test.FixedState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                
                [TestClass]
                public class TestClass
                {
                    public required TestContext TestContext { get; set; }
 
                    [TestMethod]
                    public void Test01()
                    {
                        var helper = CalculateSomething(2, testContext: TestContext);
                    }
                    
                    private static int CalculateSomething(
                        int one,
                        int? two = null,
                        TestContext? testContext = null,
                        int? three = null)
                    {
                        return one + 2;
                    }
                }
                """);

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToStaticMethodWithTestContextAsSecondArgument_AddsArgumentToCall()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
 
                [TestClass]
                public class TestClass
                {
                    public required TestContext TestContext { get; set; }
 
                    [TestMethod]
                    public void Test01()
                    {
                        TestHelperMethod(1);
                    }
                    
                    private void TestHelperMethod(int anInteger, TestContext? testContext = null) { }
                }
                """);
         test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 11, 25, 11, 28));
        
         test.FixedState.Sources
            .AddMSTestDeclarationsCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
 
                [TestClass]
                public class TestClass
                {
                    public required TestContext TestContext { get; set; }
 
                    [TestMethod]
                    public void Test01()
                    {
                        TestHelperMethod(1, TestContext);
                    }
                    
                    private void TestHelperMethod(int anInteger, TestContext? testContext = null) { }
                }
                """);

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToMethodWithoutOptionalTestContextInChildClassOfTestClass_ReportsDiagnosticDoesntFix()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                
                [TestClass]
                public class TestClass
                {
                    private class TestFixture
                    {
                        public void TestMethod()
                        {
                            TestHelper.New();
                        }
                    }
                }
                """);
 
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 10, 27, 10, 29));
        test.FixedState.Sources
            .AddMSTestDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                
                [TestClass]
                public class TestClass
                {
                    private class TestFixture
                    {
                        public void TestMethod()
                        {
                            TestHelper.New();
                        }
                    }
                }
                """);
        
        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToMethodWithoutOptionalTestContextInStaticMethod_ReportsDiagnosticDoesntFix()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                
                [TestClass]
                public class TestClass
                {
                    [TestMethod]
                    public void Test01()
                    {
                        TestHelperMethod();
                    }
                    
                    private static TestHelper TestHelperMethod()
                    {
                        return TestHelper.New();
                    }
                }
                """);
 
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 14, 30, 14, 32));
        test.FixedState.Sources
            .AddMSTestDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                
                [TestClass]
                public class TestClass
                {
                    [TestMethod]
                    public void Test01()
                    {
                        TestHelperMethod();
                    }
                    
                    private static TestHelper TestHelperMethod()
                    {
                        return TestHelper.New();
                    }
                }
                """);
        
        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToMethodWithoutOptionalTestContextInClassWithoutTestClassAttribute_ReportsDiagnosticDoesntFix()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public class TestClass
                {
                    [TestMethod]
                    public void TestMethod()
                    {
                        TestHelper.New();
                    }
                }
                """);
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 8, 23, 8, 25));
        
        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToAsyncMethodWithoutOptionalTestContext_AddsArgumentToAsyncCall()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddTestFileCode(
                """
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                
                [TestClass]
                public class TestClass
                {
                    [TestMethod]
                    public async Task Test01()
                    {
                        await CalculateSomething();
                    }
                    
                    private static async Task<int> CalculateSomething(TestContext? testContext = null)
                    {
                        return 1;
                    }
                }
                """);
 
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 10, 33, 10, 35));
        test.FixedState.Sources
            .AddMSTestDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddTestFileCode(
                """
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                
                [TestClass]
                public class TestClass
                {
                    public required TestContext TestContext { get; set; }
                
                    [TestMethod]
                    public async Task Test01()
                    {
                        await CalculateSomething(TestContext);
                    }
                    
                    private static async Task<int> CalculateSomething(TestContext? testContext = null)
                    {
                        return 1;
                    }
                }
                """);
        
        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_TwoCallToAsyncMethodWithoutOptionalTestContextInTwoTests_AddsArgumentToBothCallsInBatchFix()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddTestFileCode(
                """
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                
                [TestClass]
                public class TestClass
                {
                    [TestMethod]
                    public void Test01()
                    {
                        TestHelper.New();
                    }
                    
                    [TestMethod]
                    public void Test02()
                    {
                        TestHelper.New();
                    }
                }
                """);
 
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 10, 23, 10, 25));
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 16, 23, 16, 25));
        test.FixedState.Sources
            .AddMSTestDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddTestFileCode(
                """
                using System.Threading.Tasks;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                
                [TestClass]
                public class TestClass
                {
                    public required TestContext TestContext { get; set; }
                
                    [TestMethod]
                    public void Test01()
                    {
                        TestHelper.New(TestContext);
                    }
                    
                    [TestMethod]
                    public void Test02()
                    {
                        TestHelper.New(TestContext);
                    }
                }
                """);
        
        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToPrivateMethodWithDifferentTestContextType_NotReportsDiagnostic()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                """
                using MyContext;

                namespace MyContext { public class TestContext { } }

                [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
                public class TestClass
                {
                    [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
                    public void TestMethod()
                    {
                        HelperMethod();
                    }
                    
                    private static void HelperMethod(TestContext? context = null) { }
                }
                """);
        
        await test.RunAsync();
    }
    
    [Theory]
    [InlineData("null")]
    [InlineData("new TestContext()")]
    [InlineData("new()")]
    public async Task AnalyzeAndFix_CallToStaticMethodWithVariousArgumentsForOptionalTestContext_NotReportsDiagnostic(
        string argument)
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddTestFileCode(
                $$"""
                  using Microsoft.VisualStudio.TestTools.UnitTesting;

                  [TestClass]
                  public class TestClass
                  {
                      [TestMethod]
                      public void TestMethod()
                      {
                          TestHelper.New({{argument}});
                      }
                  }
                  """);

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToMethodWithoutOptionalTestContextInMethodWithCustomTestMethodAttribute_ReportsDiagnostic()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                class MyCustomTestMethodAttribute : TestMethodAttribute { }

                [TestClass]
                public class TestClass
                {
                    [MyCustomTestMethodAttribute]
                    public void TestMethod()
                    {
                        TestHelper.New();
                    }
                }
                """);
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 11, 23, 11, 25));

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToMethodWithoutOptionalTestContextInMethodWithCustomTestClassAttribute_ReportsDiagnostic()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                
                class MyCustomTestClassAttribute : TestClassAttribute { }
                
                [MyCustomTestClass]
                public class TestClass
                {
                    [TestMethod]
                    public void TestMethod()
                    {
                        TestHelper.New();
                    }
                }
                """);
        
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 11, 23, 11, 25));

        await test.RunAsync();
    }
    
    [Fact]
    public async Task AnalyzeAndFix_CallToMethodWithoutOptionalTestContextInChildClassOfTestClass_ReportsDiagnostic()
    {
        var test = new CodeFixTest();
        test.TestState.Sources
            .AddMSTestDeclarationsCode()
            .AddSampleTestHelperCode()
            .AddRuntimeDummyDeclarationsCode()
            .AddTestFileCode(
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                
                [TestClass]
                public class TestClass
                {
                    private class TestFixture
                    {
                        public void TestMethod()
                        {
                            TestHelper.New();
                        }
                    }
                }
                """);
 
        test.ExpectedDiagnostics.Add(Verifier.Diagnostic()
            .WithSpan("/TestSample.cs", 10, 27, 10, 29));
        
        await test.RunAsync();
    }
}