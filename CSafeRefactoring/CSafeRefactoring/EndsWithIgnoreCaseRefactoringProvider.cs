using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(EndsWithIgnoreCaseRefactoringProvider)), Shared]
    public class EndsWithIgnoreCaseRefactoringProvider : StartsEndsWithRefactoringBase
    {
        protected override string MethodToReplaceName => "EndsWithCSafe";
        protected override string NewMethodName => "EndsWith";

        protected override bool IgnoreCase => true;

        protected override bool AnalyzeAdditionalRestrictions(InvocationExpressionSyntax invocationExpr)
        {
            return invocationExpr.ArgumentList.Arguments.Count == 2;
        }
    }
}