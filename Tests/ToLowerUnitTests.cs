using CSafeRefactoring;
using Microsoft.CodeAnalysis.CodeRefactorings;
using NUnit.Framework;
using CodeRefactoringTestFixture = Tests.Base.CodeRefactoringTestFixture;

namespace Tests
{
    [TestFixture]
    public class ToLowerUnitTests : CodeRefactoringTestFixture
    {

        [Test]
        public void Identifiers()
        {
            const string markupCode = @"
public class Ex 
{
    public void method()
    {
        str.[|ToLowerCSafe|]();
    }
}
";

            const string expected = @"
public class Ex 
{
    public void method()
    {
        str.ToLowerInvariant();
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
        ""str"".[|ToLowerCSafe|]();
    }
}
";

            const string expected = @"
public class Ex 
{
    public void method()
    {
        ""str"".ToLowerInvariant();
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
        (""text"" + str).[|ToLowerCSafe|]();
    }
}
";

            const string expected = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).ToLowerInvariant();
    }
}
";
            TestCodeRefactoring(markupCode, expected);
        }
        protected override CodeRefactoringProvider CreateProvider()
        {
            return new ToLowerRefactoringProvider();
        }
    }
}
