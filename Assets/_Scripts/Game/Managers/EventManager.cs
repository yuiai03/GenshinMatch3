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

    public static event Action<bool> OnBoardStateChanged;
    public static void BoardStateChangedAction(bool isBusy)
    {
        OnBoardStateChanged?.Invoke(isBusy);
    }

    public static event Action<GameState> OnGameStateChanged;
    public static void GameStateChangedAction(GameState gameState)
    {
        OnGameStateChanged?.Invoke(gameState);
    }

    public static event Action<SceneType> OnSceneChanged;
    public static void SceneChangedAction(SceneType sceneType)
    {
        OnSceneChanged?.Invoke(sceneType);
    }

    public static event Action<float, EntityType> OnHPChanged;
    public static void HPChangedAction(float HP, EntityType entityType)
    {
        OnHPChanged?.Invoke(HP, entityType);
    }
}
