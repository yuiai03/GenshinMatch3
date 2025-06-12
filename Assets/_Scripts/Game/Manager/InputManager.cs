using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public bool IsSwapping { get; set; }
    [SerializeField] private Tile _selectedTile;
    [SerializeField] private Tile _targetTile;
    [SerializeField] private Empty _selectedEmpty;
    [SerializeField] private Vector2 _touchStartPosition;
    [SerializeField] private Vector2Int _swapDirection;
    [SerializeField] private Vector2Int _currentSwapDirection;
    private BoardManager boardManager => BoardManager.Instance;

    private void OnEnable()
    {
        EventManager.OnEndSwapTile += OnEndSwapTile;
        EventManager.OnStartSwapTile += OnStartSwapTile;
    }
    private void OnDisable()
    {
        EventManager.OnEndSwapTile -= OnEndSwapTile;
        EventManager.OnStartSwapTile -= OnStartSwapTile;
    }
    private void OnEndSwapTile()
    {
        IsSwapping = false;
        //boardManager.CheckAndExplodeMatches();
        //if (boardManager.GetMatchHistory().Count == 0)
        //{
        //    boardManager.HandeSwapTiles(_targetTile, _selectedTile);
        //}
        //else
        //{
        //    _selectedTile = null;
        //    _targetTile = null;
        //}
    }

    private void OnStartSwapTile(Tile selectedTile, Tile targetTile)
    {
        if(!selectedTile || !targetTile) return;
        IsSwapping = true;
    }

    void Update()
    {
        InputHandle();
    }
    private void InputHandle()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            var touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
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
        if (hit.collider)
        {
            _selectedTile = hit.collider.GetComponent<Tile>();
            if (_selectedTile)
            {
                _touchStartPosition = touch.position;
            }
        }
    }
    private void MovedPhase(Touch touch, RaycastHit2D hit)
    {
        if (_selectedTile && !IsSwapping)
        {
            Vector2 touchPosition = touch.position;
            Vector2 direction = touchPosition - _touchStartPosition;
            _currentSwapDirection = GetSwapDirection(direction);
            _targetTile = hit.collider.GetComponent<Tile>();

            if (_selectedTile != _targetTile) CheckToSwapTiles();
        }
    }
    private void EndedPhase(Touch touch, RaycastHit2D hit)
    {
        if (_selectedTile) _selectedTile = null;
        if (_targetTile) _targetTile = null;
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
        Vector2Int newPos = _selectedTile.Empty.IntPos + _currentSwapDirection;
        if(IsValidPos(newPos) && newPos == _targetTile.Empty.IntPos)
        {
            boardManager.HandeSwapTiles(_selectedTile, _targetTile);
        }
    }

    private bool IsValidPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < boardManager.Width 
            && pos.y >= 0 && pos.y < boardManager.Height;
    }
}