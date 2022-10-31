using NUnit.Framework;
using Matrix.Tools;

namespace MatrixTests
{
    [TestFixture]
    public class MatrixTest
    {
        [TestCase("Tests\\Matrix_SolveFirstLevel_Positive_1.txt", "Tests\\Matrix_SolveFirstLevel_Positive_1expected.txt")]
        [TestCase("Tests\\Matrix_SolveFirstLevel_Positive_2.txt", "Tests\\Matrix_SolveFirstLevel_Positive_2expected.txt")]
        [TestCase("Tests\\Matrix_SolveFirstLevel_Positive_3.txt", "Tests\\Matrix_SolveFirstLevel_Positive_3expected.txt")]
        [TestCase("Tests\\Matrix_SolveFirstLevel_Positive_4.txt", "Tests\\Matrix_SolveFirstLevel_Positive_3expected.txt")]
        public void Matrix_SolveFirstLevel_Positive(string path, string pathActual)
        {
            StreamReader reader = new StreamReader(Consts.path + path);
            var textActual = RemoveConrtolChars(reader.ReadToEnd());
            reader = new StreamReader(Consts.path + pathActual);
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