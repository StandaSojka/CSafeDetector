using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using NUnit.Framework;

namespace Tests.Base
{
    class Verify
    {
        public static void CodeAction(CodeAction codeAction, Document document, string expectedCode)
        {
            ImmutableArray<CodeActionOperation> result = codeAction.GetOperationsAsync(CancellationToken.None).Result;
            Assert.That(result.Count(), Is.EqualTo(1));

            CodeActionOperation codeActionOperation = result.Single();
            Workspace workspace1 = document.Project.Solution.Workspace;

            Workspace workspace2 = workspace1;
            CancellationToken none = CancellationToken.None;
            codeActionOperation.Apply(workspace2, none);
            Assert.That(workspace1.CurrentSolution.GetDocument(document.Id).GetTextAsync(CancellationToken.None).Result.ToString(), Is.EqualTo(expectedCode));
        }
    }
}
