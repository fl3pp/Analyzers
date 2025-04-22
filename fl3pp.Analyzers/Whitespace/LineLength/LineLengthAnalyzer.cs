using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace fl3pp.Analyzers.Whitespace.LineLength;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LineLengthAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(LineLengthDiagnostic.Descriptor);
    
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeCompilationUnit, SyntaxKind.CompilationUnit);
    }

    private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
    {
        var node = (CompilationUnitSyntax)context.Node;
        var analyzerConfig = context.Options.AnalyzerConfigOptionsProvider.GetOptions(node.SyntaxTree);

        var maxLineLength = TryGetIntegerSetting(LineLengthDiagnostic.MaxLineLengthConfigKey)
            ?? TryGetIntegerSetting(LineLengthDiagnostic.GuidelinesConfigKey)
            ?? LineLengthDiagnostic.DefaultMaxLineLength;
        
        int? TryGetIntegerSetting(string settingKey)
        {
            if (!analyzerConfig.TryGetValue(settingKey, out var settingValue)) return null;
            if (settingValue is null) return null;
            if (!int.TryParse(settingValue,
                 NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
                 CultureInfo.InvariantCulture,
                 out var intValue)) return null;
            if (intValue < 0) return null;
            return intValue;
        }
        
        foreach (var line in context.Node.SyntaxTree.GetText().Lines)
        {
            var overflow = (line.End - line.Start) - maxLineLength;
            if (overflow > 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    LineLengthDiagnostic.Descriptor,
                    Location.Create(node.SyntaxTree, new(line.Start + maxLineLength, overflow)),
                    line.LineNumber + 1,
                    overflow));
            }
        }
    }
}