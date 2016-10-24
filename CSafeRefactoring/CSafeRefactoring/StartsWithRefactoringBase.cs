using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring
{
    public abstract class StartsWithRefactoringBase : CodeRefactoringProvider
    {
        protected abstract string MethodToReplaceName { get; }

        protected abstract bool IgnoreCase { get; }

        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var node = root.FindNode(context.Span);

            if (node.GetText().ToString() != MethodToReplaceName)
            {
                return;
            }

            var declaration =
                root.FindNode(context.Span).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();

            if (!AnalyzeAdditionalRestrictions(declaration))
            {
                return;
            }

            var messageInvariantCulture = IgnoreCase
                ? "Use StringComparison.InvariantCultureIgnoreCase"
                : "Use StringComparison.InvariantCulture";
            var messageOrdinal = IgnoreCase ? "Use StringComparison.OrdinalIgnoreCase" : "Use StringComparison.Ordinal";

            var actionIvariantCulture = CodeAction.Create(messageInvariantCulture,
                c => StringComparisonInvariantCulture(context.Document, declaration, c));
            var actionOrdinal = CodeAction.Create(messageOrdinal,
                c => StringComparisonOrdinal(context.Document, declaration, c));

            context.RegisterRefactoring(actionIvariantCulture);
            context.RegisterRefactoring(actionOrdinal);
        }


        protected async Task<Document> StringComparisonInvariantCulture(Document document,
            InvocationExpressionSyntax invocationExpr, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);

            Debug.WriteLine("root");
            var memberAccessExpressionSyntax = invocationExpr.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax?.Expression == null)
            {
                return document;
            }

            var param1 = memberAccessExpressionSyntax.Expression;
            var argumentList = invocationExpr.ArgumentList;
            var param2 = argumentList.Arguments[0].Expression;

            SyntaxNode newRoot;

            if (IgnoreCase)
            {
                newRoot = root.ReplaceNode(invocationExpr, Create(param1, param2, "InvariantCultureIgnoreCase"));
            }
            else
            {
                newRoot = root.ReplaceNode(invocationExpr, Create(param1, param2, "InvariantCulture"));
            }

            return document.WithSyntaxRoot(newRoot);
        }


        protected async Task<Document> StringComparisonOrdinal(Document document, InvocationExpressionSyntax invocationExpr, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);

            var memberAccessExpressionSyntax = invocationExpr.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax != null)
            {
                var param1 = memberAccessExpressionSyntax.Expression;
                var argumentList = invocationExpr.ArgumentList;
                var param2 = argumentList.Arguments[0].Expression;

                SyntaxNode newRoot;

                if (IgnoreCase)
                {
                    newRoot = root.ReplaceNode(invocationExpr, Create(param1, param2, "OrdinalIgnoreCase"));
                }
                else
                {
                    newRoot = root.ReplaceNode(invocationExpr, Create(param1, param2, "Ordinal"));
                }

                var newDocument = document.WithSyntaxRoot(newRoot);

                return newDocument;
            }

            return document;
        }


        protected abstract bool AnalyzeAdditionalRestrictions(InvocationExpressionSyntax invocationExpr);


        private InvocationExpressionSyntax Create(ExpressionSyntax param1, ExpressionSyntax param2, string type)
        {
            return
                SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            param1,
                            SyntaxFactory.IdentifierName("StartsWith")
                        )
                    )
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    SyntaxFactory.Argument(param2),
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

            /*return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.PredefinedType(
                            SyntaxFactory.Token(SyntaxKind.StringKeyword)
                        ),
                        SyntaxFactory.IdentifierName("Equals")
                    )
                )
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                SyntaxFactory.Argument(param1),
                                SyntaxFactory.Token(
                                    SyntaxFactory.TriviaList(),
                                    SyntaxKind.CommaToken,
                                    SyntaxFactory.TriviaList(
                                        SyntaxFactory.Space
                                    )
                                ),
                                SyntaxFactory.Argument(param2),
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
                );*/
        }
    }
}