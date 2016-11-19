using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;

namespace CSafeRefactoring
{
    public abstract class CodeRefactoringProviderBase : CodeRefactoringProvider
    {
        private const string ORDINAL_IGNORECASE = "Use StringComparison.OrdinalIgnoreCase";
        private const string ORDINAL = "Use StringComparison.Ordinal";

        private const string INVARIANTCULTURE_IGNORECASE = "Use StringComparison.InvariantCultureIgnoreCase";
        private const string INVARIANTCULTURE = "Use StringComparison.InvariantCulture";

        protected abstract string MethodToReplaceName { get; }

        protected abstract bool IgnoreCase { get; }

        protected string GetOrdinalMessage()
        {
            return IgnoreCase ? ORDINAL_IGNORECASE : ORDINAL;
        }

        protected string GetInvariantCultureMessage()
        {
            return IgnoreCase ? INVARIANTCULTURE_IGNORECASE : INVARIANTCULTURE;
        }

        protected bool IsCurrentNodeReplacedMethod(SyntaxNode node)
        {
            return node.GetText().ToString() == MethodToReplaceName;
        }
    }
}
