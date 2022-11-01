using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matrix.Tools;

namespace MatrixTests
{
    [TestFixture]
    public class ExtentionsTests
    {
        [TestCase(1, ExpectedResult = Values.One)]
        [TestCase(2, ExpectedResult = Values.Two)]
        [TestCase(3, ExpectedResult = Values.Tree)]
        [TestCase(4, ExpectedResult = Values.Four)]
        [TestCase(5, ExpectedResult = Values.Five)]
        [TestCase(6, ExpectedResult = Values.Six)]
        [TestCase(7, ExpectedResult = Values.Seven)]
        [TestCase(8, ExpectedResult = Values.Eight)]
        [TestCase(9, ExpectedResult = Values.Nine)]
        [TestCase(0, ExpectedResult = Values.None)]
        public Values ToValues_Positive(int number) => number.ToValues();

        [TestCase(10)]
        [TestCase(-1)]
        [TestCase(99999)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public void ToValues_Negative_ThrowAgrumentOutOfRangeException(int number)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => number.ToValues());
        }
    }
}
