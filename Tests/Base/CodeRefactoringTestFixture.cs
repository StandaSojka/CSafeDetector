using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;

namespace Tests.Base
{
    public abstract class CodeRefactoringTestFixture : BaseTestFixture
    {
        private static readonly MetadataReference CORLIB_REFERENCE = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private static readonly MetadataReference SYSTEM_CORE_REFERENCE = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
        private static readonly MetadataReference C_SHARP_SYMBOLS_REFERENCE = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        private static readonly MetadataReference CODE_ANALYSIS_REFERENCE = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

        internal static string DefaultFilePathPrefix = "Test";
        internal static string CSharpDefaultFileExt = "cs";
        internal static string TestProjectName = "TestProject";

        protected override string LanguageName => LanguageNames.CSharp;

        protected abstract CodeRefactoringProvider CreateProvider();

        protected void TestCodeRefactoring(string markupCode, params string[] expected)
        {
            Document document;
            TextSpan span;
            Assert.That(TestHelpers.TryGetDocumentAndSpanFromMarkup(markupCode, this.LanguageName, out document, out span), Is.True);

            TestCodeRefactoring(document, span, expected);
        }

        protected void TestCodeRefactoring(Document document, TextSpan span, params string[] expected)
        {
            var count = expected.Length;
            var codeRefactorings = GetCodeRefactorings(document, span);

            Assert.That(codeRefactorings.Length, Is.EqualTo(count));
            for (int i = 0; i < count; i++)
            {
                var document2 = CreateDocument(document.GetTextAsync().Result.ToString(), LanguageName);
                codeRefactorings = this.GetCodeRefactorings(document2, span);

                Verify.CodeAction(codeRefactorings[i], document2, expected[i]);
            }
        }

        private ImmutableArray<CodeAction> GetCodeRefactorings(Document document, TextSpan span)
        {
            var builder = ImmutableArray.CreateBuilder<CodeAction>();
            CreateProvider()
                .ComputeRefactoringsAsync(new CodeRefactoringContext(document, span, a => builder.Add(a), CancellationToken.None))
                .Wait();

            return builder.ToImmutable();
        }


        /// <summary>
        /// Create a Document from a string through creating a project that contains it.
        /// </summary>
        /// <param name="source">Classes in the form of a string</param>
        /// <param name="language">The language the source code is in</param>
        /// <returns>A Document created from the source string</returns>
        protected static Document CreateDocument(string source, string language = LanguageNames.CSharp)
        {
            return CreateProject(new[] { source }, language).Documents.First();
        }

        /// <summary>
        /// Create a project using the inputted strings as sources.
        /// </summary>
        /// <param name="sources">Classes in the form of strings</param>
        /// <param name="language">The language the source code is in</param>
        /// <returns>A Project created out of the Documents created from the source strings</returns>
        private static Project CreateProject(string[] sources, string language = LanguageNames.CSharp)
        {
            string fileNamePrefix = DefaultFilePathPrefix;

            var projectId = ProjectId.CreateNewId(TestProjectName);

            var solution = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, TestProjectName, TestProjectName, language)
                .AddMetadataReference(projectId, CORLIB_REFERENCE)
                .AddMetadataReference(projectId, SYSTEM_CORE_REFERENCE)
                .AddMetadataReference(projectId, C_SHARP_SYMBOLS_REFERENCE)
                .AddMetadataReference(projectId, CODE_ANALYSIS_REFERENCE);

            int count = 0;
            foreach (var source in sources)
            {
                var newFileName = fileNamePrefix + count + "." + CSharpDefaultFileExt;
                var documentId = DocumentId.CreateNewId(projectId, newFileName);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                count++;
            }

            return solution.GetProject(projectId);
        }
    }
}
