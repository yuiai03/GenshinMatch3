using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private int _turnNumber;
    public int TurnNumber
    {
        get => _turnNumber;
        set
        {
            _turnNumber = value;
            EventManager.TurnChanged(_turnNumber);
        }
    }
    private LevelData _currentLevelData;
    public LevelData CurrentLevelData
    {
        get => _currentLevelData;
        set
        {
            _currentLevelData = value;
            TurnNumber = _currentLevelData.levelConfig.turnsNumber;
        }
    }
    public SceneType SceneType { get; private set; }
    public GameState GameState { get; private set; }
    public TileData TileData { get; private set; }
    private BoardManager _boardManager => BoardManager.Instance;
    private Coroutine _onPlayerEndedActionCoroutine;

    protected override void Awake()
    {
        base.Awake();
        TileData = LoadManager.DataLoad<TileData>("TileData");
    }
    private void Start()
    {
        EventManager.GameStateChanged(GameState.GameWaiting);
    }

    private void OnEnable()
    {
        EventManager.OnGameStateChanged += SetGameState;
        EventManager.OnSceneChanged += OnSceneChange;
        EventManager.OnOpenLevelPanel += (levelData, entityType) => CurrentLevelData = levelData;
    }
    private void OnDisable()
    {
        EventManager.OnGameStateChanged -= SetGameState;
        EventManager.OnSceneChanged -= OnSceneChange;
        EventManager.OnOpenLevelPanel -= (levelData, entityData) => CurrentLevelData = levelData;
    }
    private void SetGameState(GameState state)
    {
        GameState = state;
        switch (state)
        {
            case GameState.GameStart:
                OnGameStart();
                break;
            case GameState.PlayerTurn:
                OnPlayerTurn();
                break;
            case GameState.EnemyTurn:
                OnEnemyTurn();
                break;
            case GameState.PlayerEndTurn:
                OnPlayerEndedAction();
                break;
            case GameState.EnemyEndTurn:
                OnEnemyEndedAction();
                break;
        }
    }

    private void OnGameStart() { }

    private void OnPlayerTurn()
    {
        _boardManager.SetBoardState(true);

    }

    private void OnEnemyTurn()
    {
        EventManager.GameStateChanged(GameState.EnemyEndTurn);
    }

    private void OnPlayerEndedAction()
    {
        if (_onPlayerEndedActionCoroutine != null) StopCoroutine(_onPlayerEndedActionCoroutine);
        _onPlayerEndedActionCoroutine = StartCoroutine(OnPlayerEndedActionCoroutine());
    }
    private void OnEnemyEndedAction()
    {
        BoardManager.Instance.ClearMatchedTileViews();
        LevelManager.Instance.EnemyAction();
    }

    private void OnSceneChange(SceneType sceneType)
    {
        SceneType = sceneType;
        if (sceneType == SceneType.Game)
        {
            EventManager.GameStateChanged(GameState.GameStart);
        }
    }

    private IEnumerator OnPlayerEndedActionCoroutine()
    {
        yield return new WaitForSeconds(1f);
        _boardManager.SetBoardState(false);
        _boardManager.InitializeMatchedTiles();
        yield return new WaitForSeconds(1f);
        LevelManager.Instance.PlayerAction();
    }

}
