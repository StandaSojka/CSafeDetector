using CSafeRefactoring;
using Microsoft.CodeAnalysis.CodeRefactorings;
using NUnit.Framework;
using CodeRefactoringTestFixture = Tests.Base.CodeRefactoringTestFixture;

namespace Tests
{
    [TestFixture]
    public class StartsWithIgnoreCaseUnitTests : CodeRefactoringTestFixture
    {
        [Test]
        public void Identifiers()
        {
            const string markupCode = @"
public class Ex 
{
    public void method()
    {
        str.[|StartsWithCSafe|](str2, true);
    }
}
";

            const string expected = @"
public class Ex 
{
    public void method()
    {
        str.StartsWith(str2, StringComparison.InvariantCultureIgnoreCase);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        str.StartsWith(str2, StringComparison.OrdinalIgnoreCase);
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
        ""text"".[|StartsWithCSafe|](str2, true);
    }
}
";
            const string expected = @"
public class Ex 
{
    public void method()
    {
        ""text"".StartsWith(str2, StringComparison.InvariantCultureIgnoreCase);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        ""text"".StartsWith(str2, StringComparison.OrdinalIgnoreCase);
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
        str.[|StartsWithCSafe|](""literal"", true);
    }
}
";

            const string expected = @"
public class Ex 
{
    public void method()
    {
        str.StartsWith(""literal"", StringComparison.InvariantCultureIgnoreCase);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        str.StartsWith(""literal"", StringComparison.OrdinalIgnoreCase);
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
        (""text"" + str).[|StartsWithCSafe|](str2, true);
    }
}
";
            const string expected = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).StartsWith(str2, StringComparison.InvariantCultureIgnoreCase);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).StartsWith(str2, StringComparison.OrdinalIgnoreCase);
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
        (""text"" + str).[|StartsWithCSafe|](str2 + str3, true);
    }
}
";
            const string expected = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).StartsWith(str2 + str3, StringComparison.InvariantCultureIgnoreCase);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        (""text"" + str).StartsWith(str2 + str3, StringComparison.OrdinalIgnoreCase);
    }
}
";
            TestCodeRefactoring(markupCode, expected, expected2);
        }

        protected override string LanguageName => Microsoft.CodeAnalysis.LanguageNames.CSharp;

        protected override CodeRefactoringProvider CreateProvider()
        {
            return new StartsWithIgnoreCaseRefactoringProvider();
        }
    }
}
