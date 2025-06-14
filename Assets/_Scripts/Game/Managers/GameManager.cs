using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameState GameState { get; private set; }
    private BoardManager _boardManager => BoardManager.Instance;
    private UIManager _uiManager => UIManager.Instance;
    private Coroutine _onPlayerEndedActionCoroutine;

    private void Start()
    {
        EventManager.OnGameStateChangedAction(GameState.GameStart);
    }

    private void OnEnable()
    {
        EventManager.OnGameStateChanged += SetGameState;
    }
    private void OnDisable()
    {
        EventManager.OnGameStateChanged -= SetGameState;
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
            case GameState.PlayerEndedAction:
                OnPlayerEndedAction();
                break;
            case GameState.EnemyEndedAction:
                OnEnemyEndedAction();
                break;
        }
    }

    private void OnGameStart()
    {
        _boardManager.InitializeEmpty();
        _boardManager.InitializeTiles();
    }

    private void OnPlayerTurn()
    {
        _boardManager.SetBoardState(true);

    }

    private void OnEnemyTurn()
    {
        _boardManager.SetBoardState(true);

    }

    private void OnPlayerEndedAction()
    {
        if (_onPlayerEndedActionCoroutine != null) StopCoroutine(_onPlayerEndedActionCoroutine);
        _onPlayerEndedActionCoroutine = StartCoroutine(OnPlayerEndedActionCoroutine());
    }
    private void OnEnemyEndedAction()
    {

    }


    private IEnumerator OnPlayerEndedActionCoroutine()
    {
        yield return new WaitForSeconds(1f);
        _boardManager.SetBoardState(false);
        _boardManager.InitializeMatchedTiles();
    }

}
