using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring.SyntaxTreeCreators
{
    public class EqualsSyntaxTreeCreator
    {
        private ExpressionSyntax LeftString { get; }
        private ExpressionSyntax RightString { get; }

        public EqualsSyntaxTreeCreator(ExpressionSyntax leftString, ExpressionSyntax rightString)
        {
            LeftString = leftString;
            RightString = rightString;
        }

        public InvocationExpressionSyntax Create(string type)
        {
            return SyntaxFactory.InvocationExpression(
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
                                SyntaxFactory.Argument(LeftString),
                                SyntaxFactory.Token(
                                    SyntaxFactory.TriviaList(),
                                    SyntaxKind.CommaToken,
                                    SyntaxFactory.TriviaList(
                                        SyntaxFactory.Space
                                    )
                                ),
                                SyntaxFactory.Argument(RightString),
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
