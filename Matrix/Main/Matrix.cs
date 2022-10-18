using Matrix.Interphases;
using Matrix.Tools;
using System.Linq;
using System.Text;

namespace Matrix.Main
{
    public class Matrix : IMatrix
    {
        int[,] matrix;
        Dictionary<(int, int), List<Values>> matrixMask;

        public Matrix(int[,] matrix)
        {
            if (matrix.GetLength(1) != Consts.Size && matrix.GetLength(2) != Consts.Size)
            {
                throw new ArgumentException($"Incorrect matrixConsts.Size: {matrix.GetLength(1)}, {matrix.GetLength(2)}.");
            }

            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                for (int j = 0; j < matrix.GetLength(2); j++)
                {
                    if (!Enum.IsDefined(typeof(Values), matrix[i, j]))
                    {
                        throw new ArgumentException($"Invalid value {matrix[i, j]} at ({i},{j}) position.");
                    }
                }
            }

            this.matrix = matrix;
            matrixMask = new Dictionary<(int, int), List<Values>>();
        }

        private int[] GetElement(int n, Blocks parameter)
        {
            int[] list = new int[Consts.Size];
            int cell;

            for (int i = 0; i <Consts.Size; i++)
            {
                cell = parameter switch
                {
                    Blocks.Row => matrix[n, i],
                    Blocks.Column => matrix[i, n],
                    Blocks.Block => matrix[(((n - 1) / 3) * 3 + 1) + ((i - 1) / 3), 3 * ((n - 1) % 3) + 1 + ((i - 1) % 3)],
                    _ => throw new ArgumentException()
                };
                list[i] = cell;
            }

            return list;
        }

        private bool IsElementValid(int n, Blocks block) =>
            GetElement(n, block).Where(x => (Values)x != 0).GroupBy(x => x).Select(x => x.Count()).All(x => x == 1);

        private void UpdateMatrixMask()
        {
            matrixMask = new Dictionary<(int, int), List<Values>>();
            
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                for (int j = 0; j < matrix.GetLength(2); j++)
                {
                    
                }
            }
        }

        private Dictionary<(int, int), List<Values>> GetAnswers()
        {
            return null;
        }


        public void Display()
        {
            StringBuilder buffer = new StringBuilder();

            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                buffer.Clear();

                for (int j = 0; j < matrix.GetLength(2); j++)
                {
                    buffer.Append(matrix[i, j] == 0 ? "n" : matrix[i, j]);
                }
                Console.WriteLine(buffer.ToString());
            }
        }

        public void Solve()
        {
            throw new NotImplementedException();
        }
    }
}