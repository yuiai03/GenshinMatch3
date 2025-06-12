using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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

public class Match
{
    public TileType TileType { get; set; }
    public List<Tile> Tiles { get; set; } = new List<Tile>();
}