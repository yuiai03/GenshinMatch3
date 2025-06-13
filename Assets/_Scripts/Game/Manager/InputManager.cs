using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public bool CanSwap { get; set; }
    public bool IsSwapping { get; set; }
    public bool IsBackSwapping { get; set; }
    public Tile SelectedTile { get; private set; }
    public Tile TargetTile { get; private set; }

    [SerializeField] private Empty _selectedEmpty;
    [SerializeField] private Vector2 _touchStartPosition;
    [SerializeField] private Vector2Int _swapDirection;
    [SerializeField] private Vector2Int _currentSwapDirection;
    private BoardManager boardManager => BoardManager.Instance;

    private void OnEnable()
    {
        EventManager.OnEndSwapTile += OnEndSwapTile;
        EventManager.OnStartSwapTile += OnStartSwapTile;
        EventManager.OnBoardStateChanged +=  (isBusy) => CanSwap = !isBusy;
    }
    private void OnDisable()
    {
        EventManager.OnEndSwapTile -= OnEndSwapTile;
        EventManager.OnStartSwapTile -= OnStartSwapTile;
        EventManager.OnBoardStateChanged -= (isBusy) => CanSwap = !isBusy;
    }
    private void OnEndSwapTile(Tile selectedTile, Tile targetTile)
    {
        IsSwapping = false;
        EventManager.BoardStateChanged(IsSwapping);

        boardManager.CheckAndDeleteMatches();

        Debug.Log(boardManager.GetMatchHistory().Count == 0);
        //Nếu không matches thì hoán đổi lại như cũ
        if (boardManager.GetMatchHistory().Count == 0) 
        {
            IsBackSwapping = !IsBackSwapping;
            if (IsBackSwapping)
            {
                boardManager.HandleSwapTiles(selectedTile, targetTile);
                return;
            }
        }

        //Nếu có matches thì set lại ref tiles = null
        SelectedTile = TargetTile = null;

    }

    private void OnStartSwapTile(Tile selectedTile, Tile targetTile)
    {
        if(!selectedTile || !targetTile) return;
        IsSwapping = true;
        boardManager.ClearMatchHistory();
    }

    void Update()
    {
        if (CanSwap)
        {
            InputHandle();
        }
    }
    private void InputHandle()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            var touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

            if (!hit.collider || IsSwapping) return;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    BeganPhase(touch, hit);
                    break;

                case TouchPhase.Moved:
                    MovedPhase(touch, hit);
                    break;

                case TouchPhase.Ended:
                    EndedPhase(touch, hit);
                    break;
            }
        }
    }
    private void BeganPhase(Touch touch, RaycastHit2D hit)
    {
        SelectedTile = hit.collider.GetComponent<Tile>();
        if (!SelectedTile) return;

        _touchStartPosition = touch.position;
    }
    private void MovedPhase(Touch touch, RaycastHit2D hit)
    {
        if (!SelectedTile) return;

        Vector2 touchPosition = touch.position;
        Vector2 direction = touchPosition - _touchStartPosition;
        _currentSwapDirection = GetSwapDirection(direction);
        TargetTile = hit.collider.GetComponent<Tile>();

        if (SelectedTile != TargetTile) CheckToSwapTiles();
    }
    private void EndedPhase(Touch touch, RaycastHit2D hit)
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

    //Check befo
    private void CheckToSwapTiles()
    {
        Vector2Int newPos = SelectedTile.Empty.IntPos + _currentSwapDirection;
        if(IsValidPos(newPos) && newPos == TargetTile.Empty.IntPos)
        {
            boardManager.HandleSwapTiles(SelectedTile, TargetTile);
        }
    }

    private bool IsValidPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < boardManager.Width 
            && pos.y >= 0 && pos.y < boardManager.Height;
    }
}