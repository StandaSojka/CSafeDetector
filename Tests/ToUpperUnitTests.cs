using CSafeRefactoring;
using Microsoft.CodeAnalysis.CodeRefactorings;
using NUnit.Framework;
using CodeRefactoringTestFixture = Tests.Base.CodeRefactoringTestFixture;

namespace Tests
{
    [TestFixture]
    public class ToUpperUnitTests : CodeRefactoringTestFixture
    {
        [Test]
        public void Identifiers()
        {
            const string markupCode = @"
public class Ex 
{
    public void method()
    {
        str.[|ToUpperCSafe|]();
    }
}
";

            const string expected = @"
public class Ex 
{
    public void method()
    {
        str.ToUpperInvariant();
    }
}
";
            TestCodeRefactoring(markupCode, expected);
        }


        [Test]
        public void StringLiteral()
        {
            const string markupCode = @"
public class Ex 
{
    public void method()
    {
        ""str"".[|ToUpperCSafe|]();
    }
}
";
            const string expected = @"
public class Ex 
{
    public void method()
    {
        ""str"".ToUpperInvariant();
    }
}
";
            TestCodeRefactoring(markupCode, expected);
        }


        [Test]
        public void ExpresionInSecondParameter()
        {
            const string markupCode = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).[|ToUpperCSafe|]();
    }
}
";

            const string expected = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).ToUpperInvariant();
    }
}
";
            TestCodeRefactoring(markupCode, expected);
        }
        protected override CodeRefactoringProvider CreateProvider()
        {
            return new ToUpperRefactoringProvider();
        }
    }
}
