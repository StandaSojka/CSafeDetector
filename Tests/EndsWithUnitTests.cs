using CSafeRefactoring;
using Microsoft.CodeAnalysis.CodeRefactorings;
using NUnit.Framework;
using CodeRefactoringTestFixture = Tests.Base.CodeRefactoringTestFixture;

namespace Tests
{
    [TestFixture]
    public class EndsWithUnitTests : CodeRefactoringTestFixture
    {
        [Test]
        public void Identifiers()
        {
            const string markupCode = @"
public class Ex 
{
    public void method()
    {
        str.[|EndsWithCSafe|](str2);
    }
}
";

            const string expected = @"
public class Ex 
{
    public void method()
    {
        str.EndsWith(str2, StringComparison.InvariantCulture);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        str.EndsWith(str2, StringComparison.Ordinal);
    }
}
";
            TestCodeRefactoring(markupCode, expected, expected2);
        }

        [Test]
        public void StringLiteralInFirstParameter()
        {
            const string markupCode = @"
public class Ex 
{
    public void method()
    {
        ""text"".[|EndsWithCSafe|](str2);
    }
}
";
            const string expected = @"
public class Ex 
{
    public void method()
    {
        ""text"".EndsWith(str2, StringComparison.InvariantCulture);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        ""text"".EndsWith(str2, StringComparison.Ordinal);
    }
}
";
            TestCodeRefactoring(markupCode, expected, expected2);
        }

        [Test]
        public void StringLiteralInSecondParameter()
        {
            const string markupCode = @"
public class Ex 
{
    public void method()
    {
        str.[|EndsWithCSafe|](""literal"");
    }
}
";

            const string expected = @"
public class Ex 
{
    public void method()
    {
        str.EndsWith(""literal"", StringComparison.InvariantCulture);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        str.EndsWith(""literal"", StringComparison.Ordinal);
    }
}
";
            TestCodeRefactoring(markupCode, expected, expected2);
        }

        [Test]
        public void ExpressionInFirstparameter()
        {
            const string markupCode = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).[|EndsWithCSafe|](str2);
    }
}
";
            const string expected = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).EndsWith(str2, StringComparison.InvariantCulture);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).EndsWith(str2, StringComparison.Ordinal);
    }
}
";
            TestCodeRefactoring(markupCode, expected, expected2);
        }


        [Test]
        public void ExpresionInSeconParameter()
        {
            const string markupCode = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).[|EndsWithCSafe|](str2 + str3);
    }
}
";
            const string expected = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).EndsWith(str2 + str3, StringComparison.InvariantCulture);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).EndsWith(str2 + str3, StringComparison.Ordinal);
    }
}
";
            TestCodeRefactoring(markupCode, expected, expected2);
        }

        protected override CodeRefactoringProvider CreateProvider()
        {
            return new EndsWithRefactoringProvider();
        }
    }
}
