using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerInputManager : MonoBehaviourPunCallbacks
{
    public bool CanSwap { get; set; }
    public bool IsSwapping { get; set; }
    public bool IsBackSwapping { get; set; }
    public Tile SelectedTile { get; private set; }
    public Tile TargetTile { get; private set; }
    public static MultiplayerInputManager Instance;

    [SerializeField] private Vector2 _inputStartPosition;
    [SerializeField] private Vector2Int _currentSwapDirection;

    private MultiplayerBoardManager _boardManager => MultiplayerLevelManager.Instance.MultiplayerBoardManager;

    // Mouse input tracking
    private bool _isMousePressed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public override void OnEnable()
    {
        EventManager.OnEndSwapTile += OnEndSwapTile;
        EventManager.OnStartSwapTile += OnStartSwapTile;
        EventManager.OnBoardStateChanged +=  (isBusy) => CanSwap = !isBusy;
    }
    public override void OnDisable()
    {
        EventManager.OnEndSwapTile -= OnEndSwapTile;
        EventManager.OnStartSwapTile -= OnStartSwapTile;
        EventManager.OnBoardStateChanged -= (isBusy) => CanSwap = !isBusy;
    }
    void Update()
    {
        HandleGameInput();
    }
    private void OnEndSwapTile(Tile selectedTile, Tile targetTile)
    {
        IsSwapping = false;

         _boardManager.CheckAndDeleteMatches();

        if (_boardManager.GetMatchHistory().Count == 0) 
        {
            IsBackSwapping = !IsBackSwapping;
            if (IsBackSwapping)
            {
                if (MultiplayerGameManager.Instance.IsNotMyTurn()) return;
                _boardManager.HandleSwapTiles(selectedTile, targetTile);
            }
        }
        else
        {
            SelectedTile = TargetTile = null;
        }
    }

    [PunRPC]
    public void TileEndSwap(Vector2 selectedPos, Vector2 targetPos)
    {
        IsSwapping = false;

        _boardManager.CheckAndDeleteMatches();

        if (_boardManager.GetMatchHistory().Count == 0)
        {
            IsBackSwapping = !IsBackSwapping;
        }
        else
        {
            SelectedTile = TargetTile = null;
        }
    }

    private void OnStartSwapTile(Tile selectedTile, Tile targetTile)
    {
        if (!selectedTile || !targetTile) return;

        IsSwapping = true;
    }
    private void HandleGameInput()
    {
        if (!CanSwap || !CanInput()) return;
        
        if (Input.touchCount > 0)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }
    }

    private void HandleTouchInput()
    {
        Touch touch = Input.GetTouch(0);
        var touchPos = Camera.main.ScreenToWorldPoint(touch.position);
        RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
        if (!hit.collider || IsSwapping) return;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                BeganPhase(touch.position, hit);
                break;

            case TouchPhase.Moved:
                MovedPhase(touch.position, hit);
                break;

            case TouchPhase.Ended:
                EndedPhase(touch.position, hit);
                break;
        }
    }

    private void HandleMouseInput()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (!hit.collider || IsSwapping) return;

        if (Input.GetMouseButtonDown(0))
        {
            _isMousePressed = true;
            BeganPhase(Input.mousePosition, hit);
        }
        else if (Input.GetMouseButton(0) && _isMousePressed)
        {
            MovedPhase(Input.mousePosition, hit);
        }
        else if (Input.GetMouseButtonUp(0) && _isMousePressed)
        {
            _isMousePressed = false;
            EndedPhase(Input.mousePosition, hit);
        }
    }

    private void BeganPhase(Vector2 inputPosition, RaycastHit2D hit)
    {
        SelectedTile = hit.collider.GetComponent<Tile>();
        if (!SelectedTile) return;

        _inputStartPosition = inputPosition;
    }
    private void MovedPhase(Vector2 inputPosition, RaycastHit2D hit)
    {
        if (!SelectedTile) return;

        Vector2 direction = inputPosition - _inputStartPosition;
        _currentSwapDirection = GetSwapDirection(direction);
        TargetTile = hit.collider.GetComponent<Tile>();

        if (SelectedTile != TargetTile) CheckToSwapTiles();
    }
    private void EndedPhase(Vector2 inputPosition, RaycastHit2D hit)
    {
        if(IsSwapping) return;
        SelectedTile = TargetTile = null;
    }

    private Vector2Int GetSwapDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return new Vector2Int(Mathf.RoundToInt(Mathf.Sign(direction.x)), 0); 
        else
            return new Vector2Int(0, Mathf.RoundToInt(Mathf.Sign(direction.y))); 
    }

    private void CheckToSwapTiles()
    {
        Vector2Int newPos = SelectedTile.Empty.IntPos + _currentSwapDirection;
        if(IsValidPos(newPos) && newPos == TargetTile.Empty.IntPos)
        {
            if (MultiplayerGameManager.Instance.IsNotMyTurn()) return;
            _boardManager.HandleSwapTiles(SelectedTile, TargetTile);

            var matches = _boardManager.GetType()
                .GetMethod("FindAllMatches", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_boardManager, null) as List<Match>;
            if (matches != null && matches.Count > 0)
            {
                var selectedPos = (Vector2)SelectedTile.Empty.IntPos;
                var targetPos = (Vector2)TargetTile.Empty.IntPos;
                photonView.RPC("SyncSwapTiles", RpcTarget.Others, selectedPos, targetPos);
            }
        }
    }

    [PunRPC]
    public void SyncSwapTiles(Vector2 selectedPos, Vector2 targetPos)
    {
        Tile selectedTile = _boardManager.GetTileAtPos(Vector2Int.RoundToInt(selectedPos));
        Tile targetTile = _boardManager.GetTileAtPos(Vector2Int.RoundToInt(targetPos));
        _boardManager.HandleSwapTiles(selectedTile, targetTile);
    }

    private bool IsValidPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < _boardManager.Width 
            && pos.y >= 0 && pos.y < _boardManager.Height;
    }

    private bool CanInput()
    {
        return MultiplayerGameManager.Instance.GameState == GameState.Player1Turn
            || MultiplayerGameManager.Instance.GameState == GameState.Player2Turn;
    }
}