using System.Linq;
using System.Threading.Tasks;
using CSafeRefactoring.Substituters;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring
{
    public abstract class EqualsRefactoringBase : CodeRefactoringProviderBase
    {
        private const string TYPE_ORDINAL = "Ordinal";
        private const string TYPE_INVARIANTCULTURE = "InvariantCulture";
        
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

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

        private void CreateAndRegisterCodeActions(CodeRefactoringContext context, InvocationExpressionSyntax declaration)
        {
            var messageInvariantCulture = GetInvariantCultureMessage();
            var messageOrdinal = GetOrdinalMessage();

            var substituter = new EqualsSubstituter(context.Document, declaration, IgnoreCase);

            var actionIvariantCulture = CodeAction.Create(messageInvariantCulture,
                cancellationToken => substituter.FixDocument(TYPE_INVARIANTCULTURE, cancellationToken));
            var actionOrdinal = CodeAction.Create(messageOrdinal,
                cancellationToken => substituter.FixDocument(TYPE_ORDINAL, cancellationToken));

            context.RegisterRefactoring(actionIvariantCulture);
            context.RegisterRefactoring(actionOrdinal);
        }


        protected abstract bool AnalyzeAdditionalRestrictions(InvocationExpressionSyntax invocationExpr);

    }
}
