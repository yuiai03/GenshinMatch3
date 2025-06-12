using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private BoardManager boardManager => BoardManager.Instance;
    [SerializeField] private bool _isSwapping = false;
    [SerializeField] private Tile _selectedTile;
    [SerializeField] private Tile _targetTile;
    [SerializeField] private Empty _selectedEmpty;
    [SerializeField] private Vector2Int _swapDirection;
    [SerializeField] private Vector2 _touchStartPosition;
    [SerializeField] private Vector2Int _currentSwapDirection;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            RaycastHit2D hit = 
                Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
            switch (touch.phase)
            {
                case TouchPhase.Began:

                    if (hit.collider)
                    {
                        _selectedTile = hit.collider.GetComponent<Tile>();
                        if (_selectedTile)
                        {
                            _touchStartPosition = touch.position;
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    if (_selectedTile && !_isSwapping)
                    {
                        Vector2 touchPosition = touch.position;
                        Vector2 direction = touchPosition - _touchStartPosition;
                        _currentSwapDirection = GetSwapDirection(direction);
                        _targetTile = hit.collider.GetComponent<Tile>();

                        if(_selectedTile != _targetTile) AttemptSwap();
                    }
                    break;

                case TouchPhase.Ended:
                    //selectedTile = null;
                    //targetTile = null;
                    break;
            }
        }
    }

    private Vector2Int GetSwapDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return new Vector2Int(Mathf.RoundToInt(Mathf.Sign(direction.x)), 0); 
        else
            return new Vector2Int(0, Mathf.RoundToInt(Mathf.Sign(direction.y))); 
    }

    private void AttemptSwap()
    {
        Vector2Int newPos = _selectedTile.Empty.IntPos + _currentSwapDirection;
        if(IsValidPos(newPos) && newPos == _targetTile.Empty.IntPos)
        {
            if (boardManager.SwapTiles(_selectedTile, _targetTile))
            {
                Debug.Log($"Swapped {_selectedTile.TileType} with {_targetTile.TileType}");
                //boardManager.CheckAndExplodeMatches();
                //if (boardManager.GetMatchHistory().Count == 0)
                //{
                //    boardManager.SwapTiles(selectedTile, targetTile);
                //}
            }
        }
    }

    private bool IsValidPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < boardManager.Width && pos.y >= 0 && pos.y < boardManager.Height;
    }
}