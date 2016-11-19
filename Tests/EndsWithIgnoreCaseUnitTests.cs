using CSafeRefactoring;
using Microsoft.CodeAnalysis.CodeRefactorings;
using NUnit.Framework;
using CodeRefactoringTestFixture = Tests.Base.CodeRefactoringTestFixture;

namespace Tests
{
    [TestFixture]
    public class EndsWithIgnoreCaseUnitTests : CodeRefactoringTestFixture
    {
        [Test]
        public void Identifiers()
        {
            const string markupCode = @"
public class Ex 
{
    public void method()
    {
        str.[|EndsWithCSafe|](str2, true);
    }
}
";

            const string expected = @"
public class Ex 
{
    public void method()
    {
        str.EndsWith(str2, StringComparison.InvariantCultureIgnoreCase);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        str.EndsWith(str2, StringComparison.OrdinalIgnoreCase);
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
        ""text"".[|EndsWithCSafe|](str2, true);
    }
}
";
            const string expected = @"
public class Ex 
{
    public void method()
    {
        ""text"".EndsWith(str2, StringComparison.InvariantCultureIgnoreCase);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        ""text"".EndsWith(str2, StringComparison.OrdinalIgnoreCase);
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
        str.[|EndsWithCSafe|](""literal"", true);
    }
}
";

            const string expected = @"
public class Ex 
{
    public void method()
    {
        str.EndsWith(""literal"", StringComparison.InvariantCultureIgnoreCase);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        str.EndsWith(""literal"", StringComparison.OrdinalIgnoreCase);
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
        (""text"" + str).[|EndsWithCSafe|](str2, true);
    }
}
";
            const string expected = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).EndsWith(str2, StringComparison.InvariantCultureIgnoreCase);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).EndsWith(str2, StringComparison.OrdinalIgnoreCase);
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
        (""text"" + str).[|EndsWithCSafe|](str2 + str3, true);
    }
}
";
            const string expected = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).EndsWith(str2 + str3, StringComparison.InvariantCultureIgnoreCase);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).EndsWith(str2 + str3, StringComparison.OrdinalIgnoreCase);
    }
}
";
            TestCodeRefactoring(markupCode, expected, expected2);
        }

        protected override CodeRefactoringProvider CreateProvider()
        {
            return new EndsWithIgnoreCaseRefactoringProvider();
        }
    }
}
