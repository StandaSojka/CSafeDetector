using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(StartsWithRefactoringProvider)), Shared]
    public class StartsWithRefactoringProvider : StartsWithRefactoringBase
    {
        protected override string MethodToReplaceName => "StartsWithCSafe";
        protected override bool IgnoreCase => false;

        protected override bool AnalyzeAdditionalRestrictions(InvocationExpressionSyntax invocationExpr)
        {
            return invocationExpr.ArgumentList.Arguments.Count == 1;
        }
    }
}