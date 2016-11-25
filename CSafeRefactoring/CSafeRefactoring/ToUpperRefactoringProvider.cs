using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using CSafeRefactoring.Substituters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSafeRefactoring
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ToUpperRefactoringProvider)), Shared]
    public class ToUpperRefactoringProvider : CodeRefactoringProviderBase
    {
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

            CreateAndRegisterCodeActions(context, declaration);
        }


        private void CreateAndRegisterCodeActions(CodeRefactoringContext context, InvocationExpressionSyntax declaration)
        {
            var substituter = new ToLowerSubstituter(context.Document, declaration, "ToUpperInvariant");
            var actionIvariantCulture = CodeAction.Create("Use ToUpperInvariant",
                cancellationToken => substituter.Replace(cancellationToken));

            context.RegisterRefactoring(actionIvariantCulture);
        }

        protected override string MethodToReplaceName => "ToUpperCSafe";

        protected override bool IgnoreCase => false;
    }
}
