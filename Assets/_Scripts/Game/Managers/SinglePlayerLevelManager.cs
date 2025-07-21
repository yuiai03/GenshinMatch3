using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerLevelManager : Singleton<SinglePlayerLevelManager>
{
    [SerializeField] protected GameObject _entitiesHolder;

    [Header("Spawn Points")]
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private Transform _enemySpawnPoint;

    public Player Player { get; private set; }
    public Enemy Enemy { get; private set; }

    private LevelData _levelData;
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        InitializeData();
    }

    public void InitializeData()
    {
        if (!SinglePlayerGameManager.Instance.CurrentLevelData) return;
        _levelData = SinglePlayerGameManager.Instance.CurrentLevelData;

        SinglePlayerBoardManager.Instance.InitializeEmpty();
        SinglePlayerBoardManager.Instance.InitializeTiles();

        InitializeEntities();
    }

    public void InitializeEntities()
    {
        var playerPath = $"Entities/Player/{SinglePlayerGameManager.Instance.PlayerType}";
        var enemyPath = $"Entities/Enemies/{_levelData.levelConfig.enemyType}";

        var playerPrefab = LoadManager.PrefabLoad<Player>("Entities/Player");
        var enemyPrefab = LoadManager.PrefabLoad<Enemy>("Entities/Enemy");

        var playerData = LoadManager.DataLoad<EntityData>(playerPath);
        var enemyData = LoadManager.DataLoad<EntityData>(enemyPath);


        if (!playerPrefab || !enemyPrefab) return;

        Player = Instantiate(playerPrefab, _playerSpawnPoint.position, Quaternion.identity, _entitiesHolder.transform);
        Enemy = Instantiate(enemyPrefab, _enemySpawnPoint.position, Quaternion.identity, _entitiesHolder.transform);

        Player.GetData(playerData);
        Enemy.GetData(enemyData);
    }

    public void PlayerAction()
    {
        Player.Attack(Enemy);
    }

    public void EnemyAction()
    {
        if (Enemy.IsFreeze)
        {
            Enemy.IsFreeze = false;
            EventManager.GameStateChanged(GameState.EndRound);
            return;
        }
        Enemy.Attack(Player);
    }
}

