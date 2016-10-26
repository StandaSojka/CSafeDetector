using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSafeRefactoring.SyntaxTreeCreators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring
{
    public abstract class EqualsRefactoringBase : CodeRefactoringProvider
    {
        protected abstract string MethodToReplaceName { get; }

        protected abstract bool IgnoreCase { get; }

        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var node = root.FindNode(context.Span);

            var invocationExpr = node;

            if (invocationExpr.GetText().ToString() != MethodToReplaceName)
            {
                return;
            }

            var declaration = root.FindNode(context.Span).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();

            if (!AnalyzeAdditionalRestrictions(declaration))
            {
                return;
            }

            var messageInvariantCulture = IgnoreCase ? "Use StringComparison.InvariantCultureIgnoreCase" : "Use StringComparison.InvariantCulture";
            var messageOrdinal = IgnoreCase ? "Use StringComparison.OrdinalIgnoreCase" : "Use StringComparison.Ordinal";

            var actionIvariantCulture = CodeAction.Create(messageInvariantCulture, c => FixDocument(context.Document, declaration, c, "InvariantCulture"));
            var actionOrdinal = CodeAction.Create(messageOrdinal, c => FixDocument(context.Document, declaration, c, "Ordinal"));

            context.RegisterRefactoring(actionIvariantCulture);
            context.RegisterRefactoring(actionOrdinal);
        }


        protected async Task<Document> FixDocument(Document document, InvocationExpressionSyntax invocationExpressionSyntax, CancellationToken cancellationToken, string type)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);

            var memberAccessExpressionSyntax = invocationExpressionSyntax.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax?.Expression == null)
            {
                return document;
            }

            var leadingTrivia = memberAccessExpressionSyntax.Expression.GetLeadingTrivia();
            var leftString = memberAccessExpressionSyntax.Expression.WithoutLeadingTrivia();
            var rightString = invocationExpressionSyntax.ArgumentList.Arguments[0].Expression;

            var newSyntaxTree = CreateNewOrdinalSyntaxTree(leftString, rightString, leadingTrivia, type);
            var newRoot = root.ReplaceNode(invocationExpressionSyntax, newSyntaxTree);
            return document.WithSyntaxRoot(newRoot);
        }

        private InvocationExpressionSyntax CreateNewOrdinalSyntaxTree(ExpressionSyntax leftString, ExpressionSyntax rightString, SyntaxTriviaList leadingTrivia, string type)
        {
            var syntaxTreeCreator = new EqualsSyntaxTreeCreator(leftString, rightString);
            if (IgnoreCase)
            {
               return syntaxTreeCreator.Create($"{type}IgnoreCase")
                    .WithLeadingTrivia(leadingTrivia);
            }

            return syntaxTreeCreator.Create(type)
                .WithLeadingTrivia(leadingTrivia);
        }

        protected abstract bool AnalyzeAdditionalRestrictions(InvocationExpressionSyntax invocationExpr);

    }
}
