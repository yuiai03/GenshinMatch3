using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerGameManager : Singleton<MultiplayerGameManager>
{
    public EntityType PlayerType { get; private set; }
    public GameState GameState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        PlayerType = GameManager.Instance.CurrentPlayerType;

        GameState = GameState.PlayerTurn;
        //EventManager.GameStateChanged(GameState.PlayerTurn);
    }

    private void OnEnable()
    {
        //EventManager.OnGameStateChanged += SetGameState;
    }
    private void OnDisable()
    {
        //EventManager.OnGameStateChanged -= SetGameState;
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
    }
    private void OnEnemyTurn()
    {
        if (GameState == GameState.GameEnded) return;
    }

    private void OnPlayerEndedAction()
    {

    }
    private void OnEnemyEndedAction()
    {
        SinglePlayerBoardManager.Instance.ClearMatchedTileViews();
        SinglePlayerLevelManager.Instance.EnemyAction();
    }

    private void OnEndRoundAction()
    {
        if (GameState == GameState.GameEnded) return;
    }

    private void OnGameStart()
    {
        EventManager.GameStateChanged(GameState.PlayerTurn);
    }
    private void OnGameEnded() { }
}
