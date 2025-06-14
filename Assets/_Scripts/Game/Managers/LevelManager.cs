using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private GameObject _entitiesHolder;

    [Header("Spawn Points")]
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private Transform _enemySpawnPoint;

    public Player Player { get; set; }
    public Enemy Enemy { get; set; }
    public LevelData LevelData { get; set; }
    private BoardManager _boardManager;
    protected override void Awake()
    {
        base.Awake();

        _boardManager = GetComponentInChildren<BoardManager>();
    }
    private void Start()
    {
        _boardManager.InitializeEmpty();
        _boardManager.InitializeTiles();
        InitializeEntities();
    }

    public void InitializeData(LevelData levelData)
    {
        LevelData = levelData;
    }
    private void InitializeEntities()
    {

    }
}
