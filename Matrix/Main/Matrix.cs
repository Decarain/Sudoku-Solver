using Matrix.Interphases;
using Matrix.Tools;
using System.Linq;
using System.Security.Cryptography;
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

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    try
                    {
                        matrix[i, j].ToValues();
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException($"Invalid value {matrix[i, j]} at ({i},{j}) position.");
                    }
                }
            }

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

        public string GetString()
        {
            var ans = new StringBuilder();

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    ans.Append(matrix[i, j]);
                }
            }

            return ans.ToString();
        }

        public void Solve()
        {
            SolveFirstLevel();
            SolveSecondLevel();
            var a = matrixMask.Values;
            SolveThirdLevel();
            SolveFirstLevel();
            SolveSecondLevel();
            var b = matrixMask.Values;

            var c = a == b;
        }

        //Метод прямого исключения
        private bool SolveFirstLevel()
        {
            bool hasChanged = true;
            while (hasChanged)
            {
                hasChanged = false;
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        if (matrixMask.ContainsKey((i, j)))
                        {
                            var ints = matrixMask[(i, j)].ToIntArray();
                            if (ints.Length == 1)
                            {
                                matrix[i, j] = ints[0];
                                matrixMask.Remove((i, j));
                                hasChanged = true;
                            }
                        }
                    }
                }
                UpdateMatrixMask();
            }

            return hasChanged;
        }

        //Метод косвенного исключения
        private bool SolveSecondLevel()
        {
            bool hasChanged = true;
            while (hasChanged)
            {
                hasChanged = false;

                foreach (Blocks block in Enum.GetValues(typeof(Blocks)))
                {
                    for (int blockNumber = 0; blockNumber < Consts.Size; blockNumber++)
                    {
                        var valueDictionary = GetValues((Blocks)block, blockNumber);
                        foreach (Values value in Enum.GetValues(typeof(Values)))
                        {
                            var currentValuesPair = valueDictionary.Where(x => (x.Value & value) == value).ToList();
                            if (currentValuesPair.Count == 1)
                            {
                                var position = currentValuesPair[0].Key;
                                matrix[position.Item1, position.Item2] = value.ToIntArray()[0];
                                hasChanged = true;
                            }
                        }
                    }
                }

                UpdateMatrixMask();
            }

            return hasChanged;
        }

        private bool SolveThirdLevel()
        {
            bool hasChanged = true;
            while (hasChanged)
            {
                hasChanged = false;

                foreach (Blocks block in Enum.GetValues(typeof(Blocks)))
                {
                    for (int blockNumber = 0; blockNumber < Consts.Size; blockNumber++)
                    {
                        var valueDictionary = GetMaskValues((Blocks)block, blockNumber);
                        var transposeValueDictionary = new Dictionary<Values, List<(int, int)>>();
                        List<(int, int)> positionList;
                        foreach (Values value in Enum.GetValues(typeof(Values)))
                        {
                            positionList = new List<(int, int)>();
                            var currentValuesPair = valueDictionary.Where(x => (x.Value & value) == value).Select(x => x.Key).ToList();
                            if (currentValuesPair.Count != 0)
                            {
                                transposeValueDictionary.Add(value, currentValuesPair);
                            }
                        }

                        var superValueDictionary = new Dictionary<Values, List<(int, int)>>();
                        foreach (var item in transposeValueDictionary)
                        {
                            var localValues1 = transposeValueDictionary.Where(x => (item.Value).SequenceEqual(x.Value)).Select(x => x.Key).ToList();
                            if (localValues1.Count != 0)
                            {
                                var localValues = localValues1.Aggregate((x, y) => x | y);
                                superValueDictionary.TryAdd(localValues, item.Value);
                            }
                        }
                        foreach (var item in superValueDictionary)
                        {
                            if (item.Value.Count > 1 && item.Key.ToIntArray().Length == item.Value.Count)
                            {
                                for (int i = 0; i < Consts.Size; i++)
                                {
                                    var position = GetPosition(block, blockNumber, i);
                                    if (!item.Value.Contains(position))
                                    {
                                        matrixMask[position] &= ~item.Key;
                                    }
                                }
                            }
                        }
                    }
                }

                UpdateMatrixMask();
            }

            return hasChanged;
        }

        private Values GetBlockValues(int n, Blocks block)
        {
            var values = Values.None;
            int cell;

            for (int i = 0; i < Consts.Size; i++)
            {
                var position = GetPosition(block, n, i);
                cell = matrix[position.Item1, position.Item2];
                values |= cell.ToValues();
            }

            return values;
        }

        private Values GetBlockMaskValues(int n, Blocks block)
        {
            var values = Values.None;
            int cell;

            for (int i = 0; i < Consts.Size; i++)
            {
                var position = GetPosition(block, n, i);
                if (matrixMask.ContainsKey(position))
                {
                    values |= matrixMask[position];
                }
            }

            return values;
        }

        private (int, int) GetPosition(Blocks block, int blockNumber, int cellNumber)
        {
            return block switch
            {
                Blocks.Row => (blockNumber, cellNumber),
                Blocks.Column => (cellNumber, blockNumber),
                Blocks.Block => ((blockNumber / 3) * 3 + (cellNumber / 3), 3 * (blockNumber % 3) + (cellNumber % 3)),
                _ => throw new ArgumentException()
            };
        }

        private int BlockNumberContainsElement(int row, int column) => 3 * (row / 3) + (column / 3);

        //private bool IsBlockValid(int n, Blocks block) => GetElement(n, block).Where(x => (Values)x != 0).GroupBy(x => x).Select(x => x.Count()).All(x => x == 1);

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
                    values &= matrixMask[(row, column)];
                    matrixMask.Remove((row, column));
                }

                values &= ~GetBlockValues(row, Blocks.Row);
                values &= ~GetBlockValues(column, Blocks.Column);
                values &= ~GetBlockValues(BlockNumberContainsElement(row, column), Blocks.Block);
                matrixMask.Add((row, column), values);
            }
        }

        private Dictionary<(int, int), Values> GetValues(Blocks block, int blockNuber)
        {
            var valuesDictionary = new Dictionary<(int, int), Values>();

            for (int i = 0; i < Consts.Size; i++)
            {
                valuesDictionary.Add(GetPosition(block, blockNuber, i), GetBlockValues(i, block));
            }

            return valuesDictionary;
        }


        private Dictionary<(int, int), Values> GetMaskValues(Blocks block, int blockNumber)
        {
            var valuesDictionary = new Dictionary<(int, int), Values>();

            for (int i = 0; i < Consts.Size; i++)
            {
                var position = GetPosition(block, blockNumber, i);
                if (matrix[position.Item1,position.Item2] != 0)
                {
                    valuesDictionary.Add(GetPosition(block, blockNumber, i), GetBlockMaskValues(i, block));
                }
            }

            return valuesDictionary;
        }

    }
}