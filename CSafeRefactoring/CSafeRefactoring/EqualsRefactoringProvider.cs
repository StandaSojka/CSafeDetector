using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(EqualsRefactoringProvider)), Shared]
    public class EqualsRefactoringProvider : EqualsRefactoringBase
    {
        protected override string MethodToReplaceName => "EqualsCSafe";

        protected override bool IgnoreCase => false;


        protected override bool AnalyzeAdditionalRestrictions(InvocationExpressionSyntax invocationExpr)
        {
            return invocationExpr.ArgumentList.Arguments.Count == 1;
        }

    }
}