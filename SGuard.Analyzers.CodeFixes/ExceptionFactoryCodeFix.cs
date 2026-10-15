using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SGuard.Analyzers;

/// <summary>
/// Fixes SGUARD102 by turning the exception argument into a factory: <c>new X(...)</c> becomes <c>() =&gt; new X(...)</c>.
/// The call then binds to the guard's factory overload.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExceptionFactoryCodeFix)), Shared]
public sealed class ExceptionFactoryCodeFix : CodeFixProvider
{
    private const string Title = "Pass a factory";

    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(Diagnostics.ExceptionInstance.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        if (root?.FindNode(context.Span, getInnermostNodeForTie: true) is not { } node ||
            node.AncestorsAndSelf().OfType<ArgumentSyntax>().FirstOrDefault() is not { } argument)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            context.RegisterCodeFix(
                CodeAction.Create(Title, ct => WrapInLambda(context.Document, argument, ct), equivalenceKey: Title),
                diagnostic);
        }
    }

    private static async Task<Document> WrapInLambda(Document document, ArgumentSyntax argument, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (root is null)
        {
            return document;
        }

        var expression = argument.Expression;
        var arrow = SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken)
                                 .WithLeadingTrivia(SyntaxFactory.Space)
                                 .WithTrailingTrivia(SyntaxFactory.Space);

        var lambda = SyntaxFactory.ParenthesizedLambdaExpression()
                                  .WithParameterList(SyntaxFactory.ParameterList())
                                  .WithArrowToken(arrow)
                                  .WithExpressionBody(expression.WithoutLeadingTrivia())
                                  .WithLeadingTrivia(expression.GetLeadingTrivia());

        return document.WithSyntaxRoot(root.ReplaceNode(expression, lambda));
    }
}
