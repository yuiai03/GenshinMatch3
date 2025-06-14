using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public SceneType SceneType { get; private set; }
    public GameState GameState { get; private set; }
    private BoardManager _boardManager => BoardManager.Instance;
    private UIManager _uiManager => UIManager.Instance;
    private Coroutine _onPlayerEndedActionCoroutine;

    private void Start()
    {
        EventManager.GameStateChangedAction(GameState.GameWaiting);
    }

    private void OnEnable()
    {
        EventManager.OnGameStateChanged += SetGameState;
        EventManager.OnSceneChanged += OnSceneChange;
    }
    private void OnDisable()
    {
        EventManager.OnGameStateChanged -= SetGameState;
        EventManager.OnSceneChanged -= OnSceneChange;
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

    private void OnSceneChange(SceneType sceneType)
    {
        SceneType = sceneType;
        if (sceneType == SceneType.Game)
        {
            EventManager.GameStateChangedAction(GameState.GameStart);
            UIManager.Instance.ActionPanel.gameObject.SetActive(false);
        }
    }

    private IEnumerator OnPlayerEndedActionCoroutine()
    {
        yield return new WaitForSeconds(1f);
        _boardManager.SetBoardState(false);
        _boardManager.InitializeMatchedTiles();
    }

}
