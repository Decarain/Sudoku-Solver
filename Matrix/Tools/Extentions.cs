namespace Matrix.Tools
{
    public static class Extentions
    {
        public static Values ToValues(this int[] ints)
        {
            if (ints is null)
            {
                throw new ArgumentNullException();
            }

            if (ints.Length >= Consts.Size)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (!ints.All(x => ints.Where(y => y == x).Count() == 1))
            {
                throw new ArgumentException($"Dublicates: {ints.Select(x => x.ToString()).Aggregate((x, y) => x + " " + y)}");
            }

            var values = new Values();
            foreach (var item in ints)
            {
                values |= item.ToValues();
            }

            return values;
        }

        public static Values ToValues(this int x) => x switch
        {
            0 => Values.None,
            int y when (y > 0 && y <= Consts.Size) => (Values)(1 << (y - 1)),
            _ => throw new ArgumentOutOfRangeException(),
            };

        public static int[] ToIntArray(this Values values)
        {
            if (values < Values.None || values > Values.All)
            {
                throw new ArgumentOutOfRangeException();
            }

            List<int> ints = new List<int>();
            int value = (int)values;

            if (value < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 1; i <= Consts.Size || value != 0; i++)
            {
                if (value % 2 == 1)
                {
                    ints.Add(i);
                }
                value = value >> 1;
            }

            return ints.ToArray();
        }
    }
}
