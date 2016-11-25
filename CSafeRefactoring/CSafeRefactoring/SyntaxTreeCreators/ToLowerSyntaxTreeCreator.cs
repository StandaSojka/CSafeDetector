using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring.SyntaxTreeCreators
{
    public class ToLowerSyntaxTreeCreator
    {
        private string NewMethodName { get; }
        private string IdentifierName { get; }

        public ToLowerSyntaxTreeCreator(ExpressionSyntax identifierName, string newMethodName)
        {
            
            if (identifierName == null)
            {
                throw new ArgumentNullException(nameof(identifierName));
            }

            IdentifierName = identifierName.GetText().ToString();
            NewMethodName = newMethodName;
        }

        public InvocationExpressionSyntax Create()
        {
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(IdentifierName),
                    SyntaxFactory.IdentifierName(NewMethodName)
                )
            );
        }
    }
}
