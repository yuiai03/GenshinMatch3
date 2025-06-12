using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action OnEndSwapTile;
    public static void EndSwapTileAction()
    {
        OnEndSwapTile?.Invoke();
    }
    public static event Action OnStartSwapTile;
    public static void StartSwapTileAction()
    {
        OnStartSwapTile?.Invoke();
    }
}
