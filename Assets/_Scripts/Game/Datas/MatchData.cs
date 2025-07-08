using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MatchData
{
    public TileType TileType { get; set; }
    public int Count { get; set; }

    public MatchData(TileType tileType, int count)
    {
        TileType = tileType;
        Count = count;
    }
}

public class Match
{
    public TileType TileType { get; set; }
    public List<Tile> Tiles { get; set; } = new List<Tile>();
}