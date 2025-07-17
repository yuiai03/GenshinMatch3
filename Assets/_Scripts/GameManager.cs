using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public SceneType CurrentSceneType { get; set; }
    public EntityType CurrentPlayerType { get; set; }
    public LevelData CurrentLevelData { get; set; }
    public TileData TileData { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        TileData = LoadManager.DataLoad<TileData>("TileData");
    }

    private void OnEnable()
    {
        EventManager.OnOpenLevelPanel += OnOpenLevelPanel;
        EventManager.OnSceneChanged += (sceneType) => CurrentSceneType = sceneType;
    }
    private void OnDisable()
    {
        EventManager.OnOpenLevelPanel -= OnOpenLevelPanel;
        EventManager.OnSceneChanged -= (sceneType) => CurrentSceneType = sceneType;
    }

    private void OnOpenLevelPanel(LevelData levelData)
    {
        CurrentLevelData = levelData;
    }
}
