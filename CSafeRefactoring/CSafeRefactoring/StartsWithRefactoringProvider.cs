using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(StartsWithRefactoringProvider)), Shared]
    public class StartsWithRefactoringProvider : StartsEndsWithRefactoringBase
    {
        protected override string MethodToReplaceName => "StartsWithCSafe";
        protected override string NewMethodName => "StartsWith";

        protected override bool IgnoreCase => false;

        protected override bool AnalyzeAdditionalRestrictions(InvocationExpressionSyntax invocationExpr)
        {
            return invocationExpr.ArgumentList.Arguments.Count == 1;
        }
    }
}