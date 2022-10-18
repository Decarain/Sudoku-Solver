using Matrix.Interphases;
using Matrix.Tools;
using System.Linq;
using System.Text;

namespace Matrix.Main
{
    public class Matrix : IMatrix
    {
        int[,] matrix;
        Dictionary<(int, int), Values> matrixMask;

        public Matrix(int[,] matrix)
        {
            if (matrix.GetLength(0) != Consts.Size && matrix.GetLength(1) != Consts.Size)
            {
                throw new ArgumentException($"Incorrect matrixConsts.Size: {matrix.GetLength(0)}, {matrix.GetLength(1)}.");
            }

            /*for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (!Enum.IsDefined(typeof(Values), matrix[i, j]))
                    {
                        throw new ArgumentException($"Invalid value {matrix[i, j]} at ({i},{j}) position.");
                    }
                }
            }*/

            this.matrix = matrix;
            matrixMask = new Dictionary<(int, int), Values>();
            UpdateMatrixMask();
        }

        public void Display()
        {
            StringBuilder buffer = new StringBuilder();

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                buffer.Clear();

                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    buffer.Append(matrix[i, j] == 0 ? "0" : matrix[i, j]);
                }
                Console.WriteLine(buffer.ToString());
            }
        }

        public void Solve()
        {
            for (int k = 0; k < 40; k++)
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        if (matrixMask.ContainsKey((i, j)))
                        {
                            switch (matrixMask[(i, j)])
                            {
                                case Values.One:
                                case Values.Two:
                                case Values.Tree:
                                case Values.Four:
                                case Values.Five:
                                case Values.Six:
                                case Values.Seven:
                                case Values.Eight:
                                case Values.Nine:
                                    matrix[i, j] = matrixMask[(i, j)].ToInt();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                UpdateMatrixMask();
            }
        }

        private int[] GetElement(int n, Blocks block)
        {
            int[] list = new int[Consts.Size];
            int cell;

            for (int i = 0; i < Consts.Size; i++)
            {
                cell = block switch
                {
                    Blocks.Row => matrix[n, i],
                    Blocks.Column => matrix[i, n],
                    Blocks.Block => matrix[(n / 3) * 3 + (i / 3), 3 * (n % 3) + (i % 3)],
                    _ => throw new ArgumentException()
                };
                list[i] = cell;
            }

            return list;
        }

        private int BlockNumberContainsElement(int row, int column) => 3 * (row / 3) + (column / 3);

        private bool IsBlockValid(int n, Blocks block) =>
            GetElement(n, block).Where(x => (Values)x != 0).GroupBy(x => x).Select(x => x.Count()).All(x => x == 1);

        private void UpdateMatrixMask()
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    UpdateCellAnswers(i, j);
                }
            }
        }

        private void UpdateCellAnswers(int row, int column)
        {
            if (matrix[row, column] == 0)
            {
                Values values = Values.All;
                if (matrixMask.ContainsKey((row, column)))
                {
                    values &= ~matrixMask[(row, column)];
                    matrixMask.Remove((row, column));
                }

                values &= ~GetBlockValues(row, Blocks.Row);
                values &= ~GetBlockValues(column, Blocks.Column);
                values &= ~GetBlockValues(BlockNumberContainsElement(row, column), Blocks.Block);
                matrixMask.Add((row, column), values);
            }
        }

        private Values GetBlockValues(int n, Blocks block)
        {
            Values values = 0;
            foreach (var cell in GetElement(n, block))
            {
                values |= cell.ToValues();
            }

            return values;
        }
    }
}