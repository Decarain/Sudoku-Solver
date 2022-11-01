using NUnit.Framework;
using Matrix.Tools;

namespace MatrixTests
{
    [TestFixture]
    public class MatrixTest
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void Matrix_Solve_Positive(int pathNumber)
        {
            string actualPath = "Tests\\Matrix_SolveFirstLevel_Positive_" + pathNumber + ".txt";
            string expectedPath = "Tests\\Matrix_SolveFirstLevel_PositiveExpected_" + pathNumber + ".txt";

            StreamReader reader = new StreamReader(Consts.path + actualPath);
            var textActual = RemoveConrtolChars(reader.ReadToEnd());
            reader = new StreamReader(Consts.path + expectedPath);
            var textExpected = RemoveConrtolChars(reader.ReadToEnd());
            reader.Dispose();

            int[,] squareIntArray = new int[Consts.Size, Consts.Size];
            for (int i = 0; i < squareIntArray.GetLength(0); i++)
            {
                for (int j = 0; j < squareIntArray.GetLength(1); j++)
                {
                    squareIntArray[i, j] = textActual[9 * i + j] - 48;
                }
            }

            Matrix.Main.Matrix matrix = new Matrix.Main.Matrix(squareIntArray);
            matrix.Solve();
            var actual = new string(matrix.GetString());

            Assert.AreEqual(new string(textExpected), actual);
        }

        private char[] RemoveConrtolChars(string str) => str.ToCharArray().Where(x => Consts.posibleValues.Contains(x)).ToArray();    }
}