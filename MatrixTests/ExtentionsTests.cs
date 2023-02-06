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

        [TestCase(Values.One | Values.Eight, ExpectedResult = new int[] { 1, 8 })]
        [TestCase(Values.One | Values.Two | Values.Tree, ExpectedResult = new int[] { 1, 2, 3 })]
        [TestCase(Values.One | Values.Two | Values.Four | Values.Seven | Values.Nine, ExpectedResult = new int[] { 1, 2, 4, 7, 9 })]
        [TestCase(Values.None, ExpectedResult = new int[] { })]
        [TestCase(Values.All, ExpectedResult = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 })]
        public int[] ToIntArray_Positive(Values values) => values.ToIntArray();

        [TestCase((Values)int.MaxValue)]
        [TestCase((Values)int.MinValue)]
        [TestCase((Values)(-1))]
        [TestCase((Values)99999)]
        public void ToIntArray_Negative_ThrowAgrumentOutOfRangeException(Values values)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => values.ToIntArray());
        }
    }
}
