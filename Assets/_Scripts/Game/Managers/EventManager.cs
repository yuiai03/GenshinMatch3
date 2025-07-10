using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action<Tile, Tile> OnEndSwapTile;
    public static void EndSwapTile(Tile selectedTile, Tile targetTile)
    {
        OnEndSwapTile?.Invoke(selectedTile, targetTile);
    }
    
    public static event Action<Tile, Tile> OnStartSwapTile;
    public static void StartSwapTile(Tile selectedTile, Tile targetTile)
    {
        OnStartSwapTile?.Invoke(selectedTile, targetTile);
    }

    public static event Action<bool> OnBoardStateChanged;
    public static void BoardStateChanged(bool isBusy)
    {
        OnBoardStateChanged?.Invoke(isBusy);
    }

    public static event Action<GameState> OnGameStateChanged;
    public static void GameStateChanged(GameState gameState)
    {
        OnGameStateChanged?.Invoke(gameState);
    }

    public static event Action<SceneType> OnSceneChanged;
    public static void SceneChanged(SceneType sceneType)
    {
        OnSceneChanged?.Invoke(sceneType);
    }

    public static event Action<LevelData, EntityType> OnOpenLevelPanel;
    public static void OpenLevelPanel(LevelData levelData, EntityType entityType)
    {
        OnOpenLevelPanel?.Invoke(levelData, entityType);
    }

    public static event Action<float, bool> OnMaxHPChanged;
    public static void MaxHPChanged(float hpValue, bool isPlayer)
    {
        OnMaxHPChanged?.Invoke(hpValue, isPlayer);
    }

    public static event Action<float, bool> OnHPChanged;
    public static void HPChanged(float hpValue, bool isPlayer)
    {
        OnHPChanged?.Invoke(hpValue, isPlayer);
    }

    public static event Action<TileType, bool> OnCurrentTileTypeChanged;
    public static void CurrentTileTypeChanged(TileType tileType, bool isPlayer)
    {
        OnCurrentTileTypeChanged?.Invoke(tileType, isPlayer);
    }

    public static event Action<int> OnTurnNumberChanged;
    public static void TurnNumberChanged(int turn)
    {
        OnTurnNumberChanged?.Invoke(turn);
    }
}
