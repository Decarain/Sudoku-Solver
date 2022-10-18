using Matrix.Tools;
using Matrix.Main;
using System.IO;
using System.Numerics;

internal class Program
{
    private static async Task Main(string[] args)
    {
        string text;
        int[,] ints = new int[Consts.Size, Consts.Size];

        using (StreamReader reader = new StreamReader(Consts.path + "pr1.txt"))
        {
            text = await reader.ReadToEndAsync();
        }
        var chars = text.ToCharArray().Where(x => x != '\n').Select(x => x - 48).ToArray();

        for (int i = 0; i < ints.GetLength(0); i++)
        {
            for (int j = 0; j < ints.GetLength(1); j++)
            {
                ints[i, j] = chars[9 * i + j];
            }
        }

        Matrix.Main.Matrix matrix = new Matrix.Main.Matrix(ints);
        matrix.Solve();
        matrix.Display();
    }
}

/*using (StreamWriter writer = new StreamWriter(Consts.path + "note1.txt"))
{
    await writer.WriteLineAsync("Hello World\nHello METANIT.COM");
}*/
