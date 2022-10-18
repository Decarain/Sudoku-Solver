namespace Matrix.Tools
{
    [Flags]
    public enum Values
    {
        None = 0,
        One = 1,
        Two = 2,
        Tree = 4,
        Four = 8,
        Five = 16,
        Six = 32,
        Seven = 64,
        Eight = 128,
        Nine = 256,
        All = One | Two | Tree | Four | Five | Six | Seven | Eight | Nine
    }
}