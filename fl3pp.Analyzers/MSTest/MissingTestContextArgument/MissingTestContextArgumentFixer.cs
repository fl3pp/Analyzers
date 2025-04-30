using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace fl3pp.Analyzers.MSTest.MissingTestContextArgument;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public class MissingTestContextArgumentFixer : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(MissingTestContextArgumentAnalyzerDiagnostic.UnsetTestContextParameter.Id);

    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }
    
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();
        var document = context.Document;
        var editor = await DocumentEditor.CreateAsync(document, context.CancellationToken);
        
        var argumentListToExtend = (ArgumentListSyntax)editor.OriginalRoot.FindNode(diagnostic.Location.SourceSpan);
        
        var memberNode = argumentListToExtend.FirstAncestorOrSelf<MemberDeclarationSyntax>();
        var classNode = memberNode?.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (classNode is null) return;
        
        if (!CanAccessTestClassInstanceScope(editor, classNode, memberNode!)) return;
        
        var testContextReference = GetExistingTestContext(editor, classNode)
            ?? AddTestContextProperty(editor, classNode);
        
        var newArgument = diagnostic.Properties[MissingTestContextArgumentAnalyzerDiagnostic.NamedTestContextArgumentNameProperty]
            is {} namedArgument
            ? Argument(testContextReference)
                .WithNameColon(NameColon(IdentifierName(namedArgument)))
                .NormalizeWhitespace()
            : Argument(testContextReference);
        var newInvocationNode = argumentListToExtend.AddArguments(newArgument);
        editor.ReplaceNode(argumentListToExtend, newInvocationNode);
 
        context.RegisterCodeFix(
            CodeAction.Create(MissingTestContextArgumentAnalyzerDiagnostic.AddTestContextArgumentFixTitle,
                _ => Task.FromResult(editor.GetChangedDocument()),
            nameof(MissingTestContextArgumentFixer)),
            ImmutableArray.Create(diagnostic));
    }

    private bool CanAccessTestClassInstanceScope(
        DocumentEditor editor,
        ClassDeclarationSyntax classDeclaration,
        MemberDeclarationSyntax memberDeclaration)
    {
        var classSymbol = editor.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (classSymbol is null || !IsTestClass(classSymbol)) return false;
        
        if (editor.SemanticModel.GetDeclaredSymbol(memberDeclaration) is IMethodSymbol { IsStatic: false })
        {
            return true;
        }

        return false;
    }
    
    private IdentifierNameSyntax? GetExistingTestContext(
        DocumentEditor editor, ClassDeclarationSyntax classDeclarationSyntax)
    {
        var members = editor.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax)?.GetMembers();
 
        if (members is null) return null;

        foreach (var member in members)
        {
            var type = member switch
            {
                IFieldSymbol { IsImplicitlyDeclared: false } fieldSymbol => fieldSymbol.Type,
                IPropertySymbol propertySymbol => propertySymbol.Type,
                _ => null
            };
            
            if (type is null) continue;
            
            if (MSTestDefinitions.IsTestContext(type))
            {
                return IdentifierName(member.Name);
            }
        }

        return null;
    }
    
    private IdentifierNameSyntax AddTestContextProperty(
        DocumentEditor editor,
        ClassDeclarationSyntax classDeclaration)
    {
        var classLevelIndentation = new string(classDeclaration.Members.FirstOrDefault()
            ?.GetLeadingTrivia().ToString().Reverse().TakeWhile(l => l == ' ').ToArray()
            ?? Array.Empty<char>());

        var testContextProperty = PropertyDeclaration(
                ParseTypeName(MSTestDefinitions.TestContextName),
                MSTestDefinitions.TestContextName)
            .AddModifiers(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.RequiredKeyword))
            .WithAccessorList(AccessorList(List(new[]
            {
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            })))
            .NormalizeWhitespace()
            .WithLeadingTrivia(SyntaxFactory.Whitespace(classLevelIndentation))
            .WithTrailingTrivia(SyntaxFactory.Whitespace("\r\n\r\n"));
        
        editor.InsertMembers(classDeclaration, 0, [testContextProperty]);

        return IdentifierName(testContextProperty.Identifier.WithoutTrivia());
    }
    
    private bool IsTestClass(ITypeSymbol type)
    {
        var attributes = type.GetAttributes();
        foreach (var attribute in attributes)
        {
            if (attribute.AttributeClass is null) continue;
            
            var attributeType = attribute.AttributeClass;

            while (attributeType != null)
            {
                if (attributeType.WithNullableAnnotation(new()).ToDisplayString() ==
                    MSTestDefinitions.TestClassAttributeQualifiedName)
                {
                    return true;
                }

                attributeType = attributeType.BaseType;
            }
        }
 
        return false;
    }
}
