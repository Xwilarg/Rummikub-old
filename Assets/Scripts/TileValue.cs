public struct TileValue
{
    public enum TileColor
    {
        Red,
        Black,
        Yellow,
        Blue
    }

    public TileValue(int cvalue, TileColor ccolor)
    {
        value = cvalue;
        color = ccolor;
        id = currId++;
    }

    public bool IsSame(TileValue t)
        => t.value == value && t.color == color;

    public int value; // 0: Joker
    public TileColor color;
    private int id;
    private static int currId = 0;
}