using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(EqualsIgnoreCaseRefactoringProvider)), Shared]
    public class EqualsIgnoreCaseRefactoringProvider : CodeRefactoringBase
    {
        protected override string MethodToReplaceName => "EqualsCSafe";

        protected override bool IgnoreCase => true;

        protected override bool AnalyzeAdditionalRestrictions(InvocationExpressionSyntax invocationExpr)
        {
            return invocationExpr.ArgumentList.Arguments.Count == 2;
        }

    }
}