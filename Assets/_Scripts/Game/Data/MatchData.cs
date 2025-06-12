using UnityEngine;

public class MatchData
{
    public TileType TypeType { get; }
    public int Count { get; }

    public MatchData(TileType element, int count)
    {
        TypeType = element;
        Count = count;
    }
}
