using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action OnEndSwapTile;
    public static void EndSwapTileAction()
    {
        OnEndSwapTile?.Invoke();
    }
    public static event Action<Tile, Tile> OnStartSwapTile;
    public static void StartSwapTileAction(Tile selectedTile, Tile targetTile)
    {
        OnStartSwapTile?.Invoke(selectedTile, targetTile);
    }
}
