using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerGameManager : Singleton<SinglePlayerGameManager>
{
    private int _turnNumber;
    public int TurnNumber
    {
        get => _turnNumber;
        set
        {
            _turnNumber = value;
            EventManager.TurnNumberChanged(_turnNumber);
            if (_turnNumber <= 0)
            {
                EventManager.GameStateChanged(GameState.GameEnded);
            }
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
    public GameState GameState { get; private set; }
    public EntityType PlayerType { get; set; }
    private Coroutine _onPlayerEndedActionCoroutine;
    protected SinglePlayerBoardManager _boardManager => SinglePlayerBoardManager.Instance;

    protected override void Awake()
    {
        base.Awake();
        PlayerType = GameManager.Instance.CurrentPlayerType;
        CurrentLevelData = GameManager.Instance.CurrentLevelData;
        EventManager.GameStateChanged(GameState.GameStart);
    }

    private void OnEnable()
    {
        EventManager.OnGameStateChanged += SetGameState;
    }
    private void OnDisable()
    {
        EventManager.OnGameStateChanged -= SetGameState;
    }

    public void SetGameState(GameState state)
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
            case GameState.EndRound:
                OnEndRoundAction();
                break;
            case GameState.GameEnded:
                OnGameEnded();
                break;
        }
    }

    private void OnPlayerTurn()
    {
        if (GameState == GameState.GameEnded) return;

        _boardManager.SetBoardState(true);
    }
    private void OnEnemyTurn()
    {
        if (GameState == GameState.GameEnded) return;

        EventManager.GameStateChanged(GameState.EnemyEndTurn);
    }

    private void OnPlayerEndedAction()
    {
        if (_onPlayerEndedActionCoroutine != null) StopCoroutine(_onPlayerEndedActionCoroutine);
        _onPlayerEndedActionCoroutine = StartCoroutine(OnPlayerEndedActionCoroutine());
    }
    private void OnEnemyEndedAction()
    {
        SinglePlayerBoardManager.Instance.ClearMatchedTileViews();
        SinglePlayerLevelManager.Instance.EnemyAction();
    }

    private void OnEndRoundAction()
    {
        TurnNumber--;
        if (GameState == GameState.GameEnded) return;

        EventManager.GameStateChanged(GameState.PlayerTurn);
    }

    private void OnGameStart() { }
    private void OnGameEnded() { }




    private IEnumerator OnPlayerEndedActionCoroutine()
    {
        yield return new WaitForSeconds(1f);
        _boardManager.SetBoardState(false);
        _boardManager.InitializeMatchedTiles();
        yield return new WaitForSeconds(1f);
        SinglePlayerLevelManager.Instance.PlayerAction();
    }
}
