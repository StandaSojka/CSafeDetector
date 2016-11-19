using System.Linq;
using System.Threading.Tasks;
using CSafeRefactoring.Substituters;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring
{
    public abstract class StartsEndsWithRefactoringBase : CodeRefactoringProviderBase
    {
        protected abstract string NewMethodName { get; }

        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var node = root.FindNode(context.Span);

            if (!IsCurrentNodeReplacedMethod(node))
            {
                return;
            }

            var declaration = root
                .FindNode(context.Span)
                .Parent
                .AncestorsAndSelf()
                .OfType<InvocationExpressionSyntax>()
                .First();

            if (!AnalyzeAdditionalRestrictions(declaration))
            {
                return;
            }

            CreateAndRegisterCodeActions(context, declaration);
        }

        protected abstract bool AnalyzeAdditionalRestrictions(InvocationExpressionSyntax invocationExpr);

      
        private void CreateAndRegisterCodeActions(CodeRefactoringContext context, InvocationExpressionSyntax declaration)
        {
            var messageInvariantCulture = GetInvariantCultureMessage();
            var messageOrdinal = GetOrdinalMessage();

            var substitutor = new StartsWithEndsWithSubstituter(context.Document, declaration, IgnoreCase, NewMethodName);

            var actionIvariantCulture = CodeAction.Create(
                messageInvariantCulture,
                cancellationToken => substitutor.StringComparisonInvariantCulture(cancellationToken)
            );

            var actionOrdinal = CodeAction.Create(
                messageOrdinal, 
                cancellationToken => substitutor.StringComparisonOrdinal(cancellationToken)
            );

            context.RegisterRefactoring(actionIvariantCulture);
            context.RegisterRefactoring(actionOrdinal);
        }
    }
}