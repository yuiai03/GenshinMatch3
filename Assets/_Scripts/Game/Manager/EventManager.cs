using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action<Tile, Tile> OnEndSwapTile;
    public static void EndSwapTileAction(Tile selectedTile, Tile targetTile)
    {
        OnEndSwapTile?.Invoke(selectedTile, targetTile);
    }
    public static event Action<Tile, Tile> OnStartSwapTile;
    public static void StartSwapTileAction(Tile selectedTile, Tile targetTile)
    {
        OnStartSwapTile?.Invoke(selectedTile, targetTile);
    }
}
