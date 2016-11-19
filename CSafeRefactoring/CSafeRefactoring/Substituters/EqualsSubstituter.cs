using System.Threading;
using System.Threading.Tasks;
using CSafeRefactoring.SyntaxTreeCreators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring.Substituters
{
    internal class EqualsSubstituter
    {
        private Document Document { get; }

        private InvocationExpressionSyntax InvocationExpressionSyntax { get; }

        private bool IgnoreCase { get; }

        public EqualsSubstituter(Document document, InvocationExpressionSyntax invocationExpressionSyntax, bool ignoreCase)
        {
            Document = document;
            InvocationExpressionSyntax = invocationExpressionSyntax;
            IgnoreCase = ignoreCase;
        }

        public async Task<Document> FixDocument(string type, CancellationToken cancellationToken)
        {
            var root = await Document.GetSyntaxRootAsync(cancellationToken);

            var memberAccessExpressionSyntax = InvocationExpressionSyntax.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax?.Expression == null)
            {
                return Document;
            }

            var leadingTrivia = memberAccessExpressionSyntax.Expression.GetLeadingTrivia();
            var leftString = memberAccessExpressionSyntax.Expression.WithoutLeadingTrivia();
            var rightString = InvocationExpressionSyntax.ArgumentList.Arguments[0].Expression;

            var newSyntaxTree = CreateNewOrdinalSyntaxTree(leftString, rightString, leadingTrivia, type);
            var newRoot = root.ReplaceNode(InvocationExpressionSyntax, newSyntaxTree);
            return Document.WithSyntaxRoot(newRoot);
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
    }
}
