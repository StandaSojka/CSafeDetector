using System.Threading;
using System.Threading.Tasks;
using CSafeRefactoring.SyntaxTreeCreators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring.Substituters
{
    internal class ToLowerSubstituter
    {
        private Document Document { get; }

        private InvocationExpressionSyntax InvocationExpressionSyntax { get; }

        public ToLowerSubstituter(Document document, InvocationExpressionSyntax invocationExpressionSyntax)
        {
            Document = document;
            InvocationExpressionSyntax = invocationExpressionSyntax;
        }

        public async Task<Document> Replace(CancellationToken cancellationToken)
        {
            var root = await Document.GetSyntaxRootAsync(cancellationToken);

            var memberAccessExpressionSyntax = InvocationExpressionSyntax.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax?.Expression == null)
            {
                return Document;
            }

            var leadingTrivia = memberAccessExpressionSyntax.Expression.GetLeadingTrivia();
            var identifier = memberAccessExpressionSyntax.Expression.WithoutLeadingTrivia();

            var creator = new ToLowerSyntaxTreeCreator(identifier);

            var newSyntaxTree = creator.Create()
                .WithLeadingTrivia(leadingTrivia);

            var newRoot = root.ReplaceNode(InvocationExpressionSyntax, newSyntaxTree);

            return Document.WithSyntaxRoot(newRoot);
        }
    }
}
