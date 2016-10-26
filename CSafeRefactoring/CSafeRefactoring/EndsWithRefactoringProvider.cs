using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(EndsWithRefactoringProvider)), Shared]
    public class EndsWithRefactoringProvider : StartsWithRefactoringBase
    {
        protected override string MethodToReplaceName => "EndsWithCSafe";
        protected override string NewMethodName => "EndsWith";

        protected override bool IgnoreCase => false;

        protected override bool AnalyzeAdditionalRestrictions(InvocationExpressionSyntax invocationExpr)
        {
            return invocationExpr.ArgumentList.Arguments.Count == 1;
        }
    }
}