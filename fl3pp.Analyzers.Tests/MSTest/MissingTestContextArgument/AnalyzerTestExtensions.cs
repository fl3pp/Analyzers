using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

namespace fl3pp.Analyzers.Tests.MSTest.MissingTestContextArgument;

public static class AnalyzerTestExtensions
{
    public static SourceFileList AddRuntimeDummyDeclarationsCode(this SourceFileList sourceFiles)
    {
        IList<(string, SourceText)> testFiles = sourceFiles;
        testFiles.Add(("/RuntimeDeclarations.cs", SourceText.From(
            """
            namespace System.Runtime.CompilerServices
            {
                public class RequiredMemberAttribute : Attribute { }
                public class CompilerFeatureRequiredAttribute(string featureName) : Attribute { }
                
                interface INotifyCompletion
                {
                    public void OnCompleted(Action continuation);
                }
            }
            
            namespace System.Threading.Tasks
            {
                public class Awaiter<T> : System.Runtime.CompilerServices.INotifyCompletion
                {
                    public bool IsCompleted => true;
                    public void OnCompleted(Action continuation) { }
                    public Awaiter<T> GetAwaiter() => this;
                    public T GetResult() { return default!; }
                }
            
                public class Task<TResult>
                {
                    public Awaiter<TResult> GetAwaiter() => new();
                }
            }
            """)));
        return sourceFiles;
    }
    
    public static SourceFileList AddMSTestDeclarationsCode(this SourceFileList sourceFiles)
    {
        IList<(string, SourceText)> testFiles = sourceFiles;
        testFiles.Add(("/MSTestDeclarations.cs", SourceText.From(
            """
            namespace Microsoft.VisualStudio.TestTools.UnitTesting
            {
                public class TestContext { }
                public class TestMethodAttribute : global::System.Attribute { }
                public class TestClassAttribute : global::System.Attribute { }
            }
            """)));
        return sourceFiles;
    }
 
    public static SourceFileList AddSampleTestHelperCode(
        this SourceFileList sourceFiles)
    {
        IList<(string, SourceText)> testFiles = sourceFiles;
        testFiles.Add(("/TestHelper.cs", SourceText.From(
            """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            
            class TestHelper
            {
                public TestHelper(TestContext? testContext = null) { }
                
                public void SetTestContext(TestContext? testContext = null) { }
            
                public static TestHelper New(TestContext? context = null) => new(context);
            }
            """)));
        return sourceFiles;
    }
 
    public static SourceFileList AddTestMethodSampleCode(
        this SourceFileList sourceFiles, string code)
    {
        return sourceFiles.AddTestClassSampleCode(
            $$"""
            public void Test()
            {
            {{code}}
            }
            """);
    }
    
    
    public static SourceFileList AddTestClassSampleCode(
        this SourceFileList sourceFiles, string code)
    {
        return sourceFiles.AddTestFileCode(
            $$"""
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            
            class Tests
            {
            {{code}}
            }
            """);
    }
 
    public static SourceFileList AddTestFileCode(
        this SourceFileList sourceFiles, string code)
    {
        IList<(string, SourceText)> testFiles = sourceFiles;
        testFiles.Add(("/TestSample.cs", SourceText.From(code)));
        return sourceFiles;
    }
}