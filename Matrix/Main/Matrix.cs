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

            matrixMask = new Dictionary<(int, int), Values>();
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

                    if (matrix[i, j] == 0)
                    {
                        matrixMask.Add((i, j), Values.All);
                    }
                }
            }

            this.matrix = matrix;
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
            for (int i = 0; i < 11; i++)
            {
                SolveFirstLevel();
                SolveSecondLevel();
            }
        }

        private bool UpdateCellAnswers()
        {
            var hasChanged = false;
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

            return hasChanged;
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
                        if (matrix[i, j] == 0)
                        {
                            var oldValues = matrixMask[(i, j)];
                            var values = Values.All;
                            if (matrixMask.ContainsKey((i, j)))
                            {
                                values &= oldValues;
                                matrixMask.Remove((i, j));
                            }

                            values &= ~GetBlockValues(i, Blocks.Row);
                            values &= ~GetBlockValues(j, Blocks.Column);
                            values &= ~GetBlockValues(BlockNumberContainsElement(i, j), Blocks.Block);
                            matrixMask.Add((i, j), values);
                        }
                    }
                }
                hasChanged |= UpdateCellAnswers();
            }

            return hasChanged;
        }

        private int BlockNumberContainsElement(int row, int column) => 3 * (row / 3) + (column / 3);

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
                        var valueDictionary = GetMaskValues((Blocks)block, blockNumber)
                            .Where(x => x.Value != Values.None && matrix[x.Key.Item1, x.Key.Item2] != 0);
                        if (valueDictionary.ToList().Count != 0)
                        {
                            var transposeValueDictionary = new Dictionary<Values, List<(int, int)>>();
                            foreach (Values value in Enum.GetValues(typeof(Values)))
                            {
                                if (value != Values.None)
                                {
                                    var currentValuesPair = valueDictionary
                                        .Where(x => (x.Value & value) == value)
                                        .Select(x => x.Key)
                                        .ToList();
                                    if (currentValuesPair.Count != 0)
                                    {
                                        transposeValueDictionary.Add(value, currentValuesPair);
                                    }
                                }
                            }

                            var possibleValuesList = GetPossibleValuesList(transposeValueDictionary
                                    .Select(x => x.Key)
                                    .Aggregate((x, y) => x | y));

                            var a = new Dictionary<Values, List<(int, int)>>();
                            foreach (var item in possibleValuesList)
                            {
                                var positionList = new List<(int, int)>();
                                foreach (var item1 in transposeValueDictionary)
                                {
                                    if ((item & item1.Key) == item1.Key)
                                    {
                                        foreach (var item2 in item1.Value)
                                        {
                                            positionList.Add(item2);
                                        }
                                    }
                                }
                                a.Add(item, positionList);
                            }

                            var b = a.Where(x => x.Key.ToIntArray().Length == x.Value.Count).ToList();
                            foreach (var item in b)
                            {
                                foreach (var item1 in valueDictionary)
                                {
                                    if (!item.Value.Contains(item1.Key) && (item.Key & item1.Value) == item1.Value)
                                    {
                                        matrixMask[item1.Key] &= ~item.Key;
                                        hasChanged = true;
                                    }
                                }
                            }
                        }

                    }
                }

                hasChanged |= UpdateCellAnswers();
            }

            return hasChanged;
        }

        private List<Values> GetPossibleValuesList(Values allValues)
        {
            var ans = new List<Values>();
            for (Values i = Values.One; i < Values.All; i++)
            {
                if ((i & allValues) == i)
                {
                    ans.Add(i);
                }
            }

            return ans;
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

        //private bool IsBlockValid(int n, Blocks block) => GetElement(n, block).Where(x => (Values)x != 0).GroupBy(x => x).Select(x => x.Count()).All(x => x == 1);

        private Dictionary<(int, int), Values> GetMaskValues(Blocks block, int blockNumber)
        {
            var valuesDictionary = new Dictionary<(int, int), Values>();

            for (int i = 0; i < Consts.Size; i++)
            {
                var position = GetPosition(block, blockNumber, i);
                if (matrix[position.Item1, position.Item2] == 0)
                {
                    valuesDictionary.Add(GetPosition(block, blockNumber, i), GetBlockMaskValues(i, block));
                }
            }

            return valuesDictionary;
        }

    }
}