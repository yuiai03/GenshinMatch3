using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerGameManager : MonoBehaviourPunCallbacks
{
    private int _turnNumber;
    public int TurnNumber
    {
        get => _turnNumber;
        set
        {
            _turnNumber = value;
            EventManager.TurnNumberChanged(_turnNumber);
        }
    }
    public EntityType PlayerType { get; private set; }
    public GameState GameState { get; private set; }
    private Coroutine _onPlayer1EndedActionCoroutine;
    private Coroutine _onPlayer2EndedActionCoroutine;
    protected MultiplayerBoardManager _boardManager => MultiplayerLevelManager.Instance.MultiplayerBoardManager;
    public static MultiplayerGameManager Instance { get; private set; }

    protected  void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        TurnNumber = 1;
        PlayerType = GameManager.Instance.CurrentPlayerType;
        EventManager.GameStateChanged(GameState.GameStart);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        EventManager.OnGameStateChanged += SetGameState;
    }
    public override void OnDisable()
    {
        base.OnDisable();
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
            case GameState.Player1Turn:
                OnPlayer1Turn();
                break;
            case GameState.Player2Turn:
                OnPlayer2Turn();
                break;
            case GameState.Player1EndTurn:
                OnPlayer1EndedAction();
                break;
            case GameState.Player2EndTurn:
                OnPlayer2EndedAction();
                break;
            case GameState.EndRound:
                OnEndRoundAction();
                break;
            case GameState.GameEnded:
                OnGameEnded();
                break;
        }
    }

    private void OnPlayer1Turn()
    {
        if (GameState == GameState.GameEnded) return;

        _boardManager.ClearMatchHistory();
        _boardManager.SetBoardState(true);
        _boardManager.ClearMatchedTileViews();

        if (MultiplayerLevelManager.Instance.Player1.IsFreeze)
        {
            MultiplayerLevelManager.Instance.Player1.IsFreeze = false;
            EventManager.GameStateChanged(GameState.Player2Turn);
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                MultiplayerPanel.Instance.SetTurnNameText("Your Turn");
                MultiplayerLevelManager.Instance.Player1.ArrowState(true);
                MultiplayerLevelManager.Instance.Player2.ArrowState(false);
            }
            else
            {
                MultiplayerPanel.Instance.SetTurnNameText("Enemy Turn");
                MultiplayerLevelManager.Instance.Player1.ArrowState(true);
                MultiplayerLevelManager.Instance.Player2.ArrowState(false);
            }
        }
    }
    private void OnPlayer2Turn()
    {
        if (GameState == GameState.GameEnded) return;
        
        _boardManager.ClearMatchHistory();
        _boardManager.SetBoardState(true);
        _boardManager.ClearMatchedTileViews();

        if (MultiplayerLevelManager.Instance.Player2.IsFreeze)
        {
            MultiplayerLevelManager.Instance.Player2.IsFreeze = false;
            EventManager.GameStateChanged(GameState.EndRound);
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                MultiplayerPanel.Instance.SetTurnNameText("Enemy Turn");
                MultiplayerLevelManager.Instance.Player2.ArrowState(true);
                MultiplayerLevelManager.Instance.Player1.ArrowState(false);
            }
            else
            {
                MultiplayerPanel.Instance.SetTurnNameText("Your Turn");
                MultiplayerLevelManager.Instance.Player2.ArrowState(true);
                MultiplayerLevelManager.Instance.Player1.ArrowState(false);
            }
        }
    }

    private void OnPlayer1EndedAction()
    {
        if (GameState == GameState.GameEnded) return;

        if (_onPlayer1EndedActionCoroutine != null) StopCoroutine(_onPlayer1EndedActionCoroutine);
        _onPlayer1EndedActionCoroutine = StartCoroutine(OnPlayer1EndedActionCoroutine());
    }
    private void OnPlayer2EndedAction()
    {
        if (GameState == GameState.GameEnded) return;

        if (_onPlayer2EndedActionCoroutine != null) StopCoroutine(_onPlayer2EndedActionCoroutine);
        _onPlayer2EndedActionCoroutine = StartCoroutine(OnPlayer2EndedActionCoroutine());
    }

    private void OnEndRoundAction()
    {
        if(GameState == GameState.GameEnded) return;

        TurnNumber++;
        EventManager.GameStateChanged(GameState.Player1Turn);
    }

    private void OnGameStart() { }
    private void OnGameEnded()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (MultiplayerLevelManager.Instance.Player2.HP <= 0)
            {
                MultiplayerPanel.Instance.SetTurnNameText("You Won");

            }
            else if (MultiplayerLevelManager.Instance.Player1.HP <= 0)
            {
                MultiplayerPanel.Instance.SetTurnNameText("You Lost");
            }
        }
        else
        {
            if (MultiplayerLevelManager.Instance.Player1.HP <= 0)
            {
                MultiplayerPanel.Instance.SetTurnNameText("You Won");

            }
            else if (MultiplayerLevelManager.Instance.Player2.HP <= 0)
            {
                MultiplayerPanel.Instance.SetTurnNameText("You Lost");
            }
        }
    }

    private IEnumerator OnPlayer1EndedActionCoroutine()
    {
        yield return new WaitForSeconds(1f);
        _boardManager.SetBoardState(false);
        _boardManager.InitializeMatchedTiles();
        yield return new WaitForSeconds(1f);
        MultiplayerLevelManager.Instance.Player1Action();
    }

    private IEnumerator OnPlayer2EndedActionCoroutine()
    {
        yield return new WaitForSeconds(1f);
        _boardManager.SetBoardState(false);
        _boardManager.InitializeMatchedTiles();
        yield return new WaitForSeconds(1f);
        MultiplayerLevelManager.Instance.Player2Action();
    }

    public bool IsNotMyTurn()
    {
        return (PhotonNetwork.IsMasterClient && GameState != GameState.Player1Turn)
            || (!PhotonNetwork.IsMasterClient && GameState == GameState.Player1Turn);
    }
}
