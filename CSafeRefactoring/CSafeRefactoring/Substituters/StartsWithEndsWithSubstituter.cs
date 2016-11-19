using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring.Substituters
{
    internal class StartsWithEndsWithSubstituter
    {
        private Document Document { get; }

        private InvocationExpressionSyntax InvocationExpressionSyntax { get; }

        private bool IgnoreCase { get; }

        private string NewMethodName { get; }

        public StartsWithEndsWithSubstituter(Document document, InvocationExpressionSyntax invocationExpressionSyntax, bool ignoreCase, string newMethodName)
        {
            Document = document;
            InvocationExpressionSyntax = invocationExpressionSyntax;
            IgnoreCase = ignoreCase;
            NewMethodName = newMethodName;
        }

        public async Task<Document> StringComparisonInvariantCulture(CancellationToken cancellationToken)
        {
            var root = await Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var memberAccessExpressionSyntax = InvocationExpressionSyntax.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax?.Expression == null)
            {
                return Document;
            }

            var param1 = memberAccessExpressionSyntax.Expression;
            var argumentList = InvocationExpressionSyntax.ArgumentList;
            var param2 = argumentList.Arguments[0].Expression;

            SyntaxNode newRoot;

            if (IgnoreCase)
            {
                newRoot = root.ReplaceNode(InvocationExpressionSyntax, Create(param1, param2, "InvariantCultureIgnoreCase"));
            }
            else
            {
                newRoot = root.ReplaceNode(InvocationExpressionSyntax, Create(param1, param2, "InvariantCulture"));
            }

            return Document.WithSyntaxRoot(newRoot);
        }

        public async Task<Document> StringComparisonOrdinal(CancellationToken cancellationToken)
        {
            var root = await Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var memberAccessExpressionSyntax = InvocationExpressionSyntax.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax == null)
            {
                return Document;
            }

            var param1 = memberAccessExpressionSyntax.Expression;
            var argumentList = InvocationExpressionSyntax.ArgumentList;
            var param2 = argumentList.Arguments[0].Expression;

            SyntaxNode newRoot;

            if (IgnoreCase)
            {
                newRoot = root.ReplaceNode(InvocationExpressionSyntax, Create(param1, param2, "OrdinalIgnoreCase"));
            }
            else
            {
                newRoot = root.ReplaceNode(InvocationExpressionSyntax, Create(param1, param2, "Ordinal"));
            }

            return Document.WithSyntaxRoot(newRoot);

        }

        private InvocationExpressionSyntax Create(ExpressionSyntax leftString, ExpressionSyntax rightString, string type)
        {
            return
                SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            leftString,
                            SyntaxFactory.IdentifierName(NewMethodName)
                        )
                    )
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    SyntaxFactory.Argument(rightString),
                                    SyntaxFactory.Token(
                                        SyntaxFactory.TriviaList(),
                                        SyntaxKind.CommaToken,
                                        SyntaxFactory.TriviaList(
                                            SyntaxFactory.Space
                                        )
                                    ),
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName("StringComparison"),
                                            SyntaxFactory.IdentifierName(type)
                                        )
                                    )
                                }
                            )
                        )
                    );
        }

    }
}
