using JetBrains.Annotations;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public static void SwapEmpty(Tile tile1, Tile tile2)
    {
        var tempEmpty = tile1.Empty;
        tile1.Empty = tile2.Empty;
        tile2.Empty = tempEmpty;
    }

    public static void SwapTileTypes(Tile tile1, Tile tile2)
    {
        TileType temp = tile1.TileType;
        tile1.TileType = tile2.TileType;
        tile2.TileType = temp;
    }
}
