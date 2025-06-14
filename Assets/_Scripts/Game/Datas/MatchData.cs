using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MatchData
{
    public TileType TypeType { get; set; }
    public int Count { get; set; }
}

public class Match
{
    public TileType TileType { get; set; }
    public List<Tile> Tiles { get; set; } = new List<Tile>();
}