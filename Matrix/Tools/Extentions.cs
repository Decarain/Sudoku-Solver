namespace Matrix.Tools
{
    public static class Extentions
    {
       public static Values ToValues(this int a)
        {
            return a switch
            {
                0 => 0,
                int x when x > 0 && x < 10 => (Values)Math.Pow(2, a - 1),
                _ => throw new ArgumentException()
            };
        }

        public static int ToInt(this Values a)
        {
            return a switch
            {
                Values.One => 1,
                Values.Two => 2,
                Values.Tree => 3,
                Values.Four => 4,
                Values.Five => 5,
                Values.Six => 6,
                Values.Seven => 7,
                Values.Eight => 8,
                Values.Nine => 9,
                _ => throw new ArgumentException()
            };
        }
    }
}
