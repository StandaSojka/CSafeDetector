﻿using CSafeRefactoring;
using Microsoft.CodeAnalysis.CodeRefactorings;
using NUnit.Framework;
using CodeRefactoringTestFixture = Tests.Base.CodeRefactoringTestFixture;

namespace Tests
{
    
       [TestFixture]
    public class EqualsUnitTests : CodeRefactoringTestFixture
    {
        [Test]
        public void Test()
        {
            const string markupCode = @"
public class Ex 
{
    public void method()
    {
        string str = ""aaa"";
        string str2 = ""aaa"";
        str.[|EqualsCSafe|](str2);
    }
}
";

            const string expected = @"
public class Ex 
{
    public void method()
    {
        string str = ""aaa"";
        string str2 = ""aaa"";
        string.Equals(str, str2, StringComparison.InvariantCulture);
    }
}
";

            const string expected2 = @"
public class Ex 
{
    public void method()
    {
        string str = ""aaa"";
        string str2 = ""aaa"";
        string.Equals(str, str2, StringComparison.Ordinal);
    }
}
";
            TestCodeRefactoring(markupCode, expected, expected2);
        }

        protected override string LanguageName => Microsoft.CodeAnalysis.LanguageNames.CSharp;

        protected override CodeRefactoringProvider CreateProvider()
        {
            return new EqualsRefactoringProvider();
        }
    }
}