using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] private GameObject BoardBg;
    [SerializeField] private GameObject EmptyHolder;
    public int Width { get; } = Config.BoardWidth;
    public int Height { get; } = Config.BoardHeight;

    private Empty[,] _emptys;
    private List<MatchData> _matchsHistory = new List<MatchData>();
    private List<MatchedTileView> _matchedTileViews = new List<MatchedTileView>();

    private Coroutine _initialTilesCoroutine;

    private void OnEnable()
    {
        EventManager.OnTileMatch += AddElementMatchHistory;
    }
    private void OnDisable()
    {
        EventManager.OnTileMatch -= AddElementMatchHistory;
    }

    public void InitializeEmpty()
    {
        if (!EmptyHolder || !BoardBg) return;
        
        _emptys = new Empty[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Empty empty = PoolManager.Instance.GetObject<Empty>
                    (PoolType.Empty, new Vector2(x, y), EmptyHolder.transform);
                _emptys[x, y] = empty;
                _emptys[x, y].IntPos = new Vector2Int(x, y);
            }
        }
    }
    public void InitializeTiles()
    {
        if(_initialTilesCoroutine != null) StopCoroutine(_initialTilesCoroutine);
        _initialTilesCoroutine = StartCoroutine(InitialTilesCoroutine());
    }
    public void InitializeMatchedTiles()
    {
        foreach (var match in _matchsHistory)
        {
            var holder = UIManager.Instance.GamePanel.MatchedTilesViewHolder;
            MatchedTileView matchedTileView = 
                PoolManager.Instance.GetObject<MatchedTileView>(
                PoolType.MatchedTileView, Vector2.zero, holder.transform);
            matchedTileView.InitialData(match.TileType, match.Count);

            _matchedTileViews.Add(matchedTileView);
        }
    }
    private IEnumerator InitialTilesCoroutine()
    {
        if (_emptys.Length == 0) yield break;

        EventManager.BoardStateChanged(true);
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector2 startPos = new Vector2(x, Height + y);
                Tile tile = PoolManager.Instance.GetObject<Tile>
                    (PoolType.Tile, startPos, EmptyHolder.transform);
                TileType tileType = GetNoMatchingTileType(x, y);

                tile.InitialData(tileType, _emptys[x, y]);

                yield return new WaitForSeconds(0.01f);
            }
        }
        yield return new WaitForSeconds(Config.TileMoveDuration);

        if (!BoardCanMatches()) RecreateBoard();
        else
        {
            EventManager.BoardStateChanged(false);
            EventManager.GameStateChanged(GameState.PlayerTurn);
        }
    }

    private TileType GetNoMatchingTileType(int x, int y)
    {
        bool[] availableTypes = new bool[Config.TileTypesCount];
        for (int i = 0; i < Config.TileTypesCount; i++)
        {
            availableTypes[i] = true;
        }

        // Kiểm tra match ngang với 2 tile bên trái
        if (x >= 2)
        {
            Tile tile1 = _emptys[x - 1, y].Tile;
            Tile tile2 = _emptys[x - 2, y].Tile;

            if (tile1 && tile2 && tile1.TileType == tile2.TileType)
            {
                int typeIndex = (int)tile1.TileType;
                availableTypes[typeIndex] = false;
            }
        }

        // Kiểm tra match dọc với 2 tile bên dưới
        if (y >= 2)
        {
            Tile tile1 = _emptys[x, y - 1].Tile;
            Tile tile2 = _emptys[x, y - 2].Tile;

            if (tile1 && tile2 && tile1.TileType == tile2.TileType)
            {
                int typeIndex = (int)tile1.TileType;
                availableTypes[typeIndex] = false;
            }
        }

        // Tạo danh sách các loại tile khả dụng
        List<TileType> validTypes = new List<TileType>();
        for (int i = 0; i < Config.TileTypesCount; i++)
        {
            if (availableTypes[i]) validTypes.Add((TileType)i);
        }

        if (validTypes.Count == 0) 
            return GetRandomTileType();
        else 
            return validTypes[UnityEngine.Random.Range(0, validTypes.Count)];
    }

    private TileType GetRandomTileType()
    {
        return (TileType)UnityEngine.Random.Range(0, Config.TileTypesCount);
    }

    public void HandleSwapTiles(Tile selectedTile, Tile targetTile)
    {
        EventManager.StartSwapTile(selectedTile, targetTile);

        Helper.SwapEmpty(selectedTile, targetTile);
    }

    public void CheckAndDeleteMatches()
    {
        var matches = FindAllMatches();
        if (matches.Count > 0)
        {
            // Tập hợp tất cả tile sẽ bị xóa (không trùng lặp)
            var allTilesToRemove = new HashSet<Tile>();
            var tileTypeCount = new Dictionary<TileType, int>();

            // Thu thập tất cả tile từ các match
            foreach (var match in matches)
            {
                foreach (var tile in match.Tiles)
                {
                    if (allTilesToRemove.Add(tile)) 
                    {
                        if (tileTypeCount.ContainsKey(tile.TileType))
                            tileTypeCount[tile.TileType]++;
                        else
                            tileTypeCount[tile.TileType] = 1;
                    }
                }
            }

            // Xóa các tile (mỗi tile chỉ xóa một lần)
            foreach (var tile in allTilesToRemove)
            {
                _emptys[tile.Empty.IntPos.x, tile.Empty.IntPos.y].Tile = null;
                PoolManager.Instance.ReturnObject(PoolType.Tile, tile.gameObject);
            }

            // Cập nhật match history với số lượng chính xác
            foreach (var tile in tileTypeCount)
            {
                var matchData = new MatchData(tile.Key, tile.Value);
                EventManager.TileMatch(matchData);
            }

            RefillBoard();
        }
        else
        {
            if (!BoardCanMatches()) RecreateBoard();
            else
            {
                if (_matchsHistory.Count == 0) return;
                EventManager.GameStateChanged(GameState.PlayerEndTurn);
            }
        }
    }

    private List<Match> FindAllMatches()
    {
        var matches = new List<Match>();

        // Tìm TẤT CẢ match ngang
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var horizontal = CheckHorizontalMatch(x, y);
                if (horizontal.Tiles.Count >= 3)
                {
                    matches.Add(horizontal);
                }
            }
        }

        // Tìm TẤT CẢ match dọc
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var vertical = CheckVerticalMatch(x, y);
                if (vertical.Tiles.Count >= 3)
                {
                    matches.Add(vertical);
                }
            }
        }

        // Loại bỏ các match trùng lặp (cùng tile set)
        var uniqueMatches = new List<Match>();
        for (int i = 0; i < matches.Count; i++)
        {
            bool isDuplicate = false;
            for (int j = 0; j < uniqueMatches.Count; j++)
            {
                if (AreMatchesIdentical(matches[i], uniqueMatches[j]))
                {
                    isDuplicate = true;
                    break;
                }
            }
            if (!isDuplicate)
            {
                uniqueMatches.Add(matches[i]);
            }
        }

        return uniqueMatches;
    }

    // kiểm tra 2 match có giống nhau không
    private bool AreMatchesIdentical(Match match1, Match match2)
    {
        if (match1.Tiles.Count != match2.Tiles.Count) return false;

        foreach (var tile1 in match1.Tiles)
        {
            bool found = false;
            foreach (var tile2 in match2.Tiles)
            {
                if (tile1 == tile2)
                {
                    found = true;
                    break;
                }
            }
            if (!found) return false;
        }
        return true;
    }

    private Match CheckHorizontalMatch(int x, int y)
    {
        var firstTile = GetTileAtPos(new Vector2Int(x, y));
        if (firstTile == null) return new Match();

        var tiles = new List<Tile> { firstTile };
        var type = firstTile.TileType;

        for (int i = 1; y + i < Height; i++)
        {
            Tile nextTile = GetTileAtPos(new Vector2Int(x, y + i));
            if (nextTile != null && nextTile.TileType == type)
                tiles.Add(nextTile);
            else
                break;
        }

        if (tiles.Count >= 3)
            return new Match { TileType = type, Tiles = tiles };
        else
            return new Match();
    }

    private Match CheckVerticalMatch(int x, int y)
    {
        var firstTile = GetTileAtPos(new Vector2Int(x, y));
        if (firstTile == null) return new Match();

        var tiles = new List<Tile> { firstTile };
        var type = firstTile.TileType;

        for (int i = 1; x + i < Width; i++)
        {
            Tile nextTile = GetTileAtPos(new Vector2Int(x + i, y));
            if (nextTile != null && nextTile.TileType == type)
                tiles.Add(nextTile);
            else
                break;
        }

        if (tiles.Count >= 3)
            return new Match { TileType = type, Tiles = tiles };
        else
            return new Match();
    }

    private bool BoardCanMatches()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile currentTile = GetTileAtPos(new Vector2Int(x, y));
                if (!currentTile) continue;

                // Kiểm tra swap với tile bên phải
                if (x < Width - 1)
                {
                    Tile rightTile = GetTileAtPos(new Vector2Int(x + 1, y));
                    if (rightTile)
                    {
                        Helper.SwapTileTypes(currentTile, rightTile);
                        bool canMatch = CanMatchAt(x, y) || CanMatchAt(x + 1, y);
                        Helper.SwapTileTypes(currentTile, rightTile);
                        if (canMatch) return true;  
                    }
                }

                // Kiểm tra swap với tile bên trên
                if (y < Height - 1)
                {
                    Tile upTile = GetTileAtPos(new Vector2Int(x, y + 1)); 
                    if (upTile)
                    {
                        Helper.SwapTileTypes(currentTile, upTile);
                        bool canMatch = CanMatchAt(x, y) || CanMatchAt(x, y + 1);
                        Helper.SwapTileTypes(currentTile, upTile);

                        if (canMatch) return true;
                    }
                }
            }
        }
        return false;
    }

    private bool CanMatchAt(int x, int y)
    {
        Tile center = GetTileAtPos(new Vector2Int(x, y));
        if (!center) return false;

        //Check hàng ngang
        if (CheckThreeTiles(x - 2, y, x - 1, y, x, y) ||    // check với 2 tile bên trái
            CheckThreeTiles(x - 1, y, x, y, x + 1, y) ||    // check với 2 tile trái và phải
            CheckThreeTiles(x, y, x + 1, y, x + 2, y))      // check với 2 tile bên phải
        {
            return true;
        }

        // Check hàng dọc
        if (CheckThreeTiles(x, y - 2, x, y - 1, x, y) ||    // check với 2 tile bên dưới
            CheckThreeTiles(x, y - 1, x, y, x, y + 1) ||    // check với 2 tile trên và dưới
            CheckThreeTiles(x, y, x, y + 1, x, y + 2))      // check với 2 tile bên trên
        {
            return true;
        }

        return false;
    }

    private bool CheckThreeTiles(int x1, int y1, int xC, int yC, int x2, int y2)
    {
        // Trả về false nếu pos không trong phạm vi board
        if (x1 < 0 || x1 >= Width || y1 < 0 || y1 >= Height ||
            xC < 0 || xC >= Width || yC < 0 || yC >= Height ||
            x2 < 0 || x2 >= Width || y2 < 0 || y2 >= Height)
        {
            return false;
        }

        Tile tile1 = GetTileAtPos(new Vector2Int(x1, y1));
        Tile center = GetTileAtPos(new Vector2Int(xC, yC));
        Tile tile2 = GetTileAtPos(new Vector2Int(x2, y2));

        // Trả về true nếu cả 3 tile cùng loại
        return tile1 && center && tile2 &&
               tile1.TileType == center.TileType &&
               center.TileType == tile2.TileType;
    }

    private void RecreateBoard()
    {
        Debug.Log("Recreate Board");

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (_emptys[x, y].Tile)
                {
                    Tile tile = _emptys[x, y].Tile;
                    PoolManager.Instance.ReturnObject(PoolType.Tile, tile.gameObject);
                    _emptys[x, y].Tile = null;
                }
            }
        }

        if (_initialTilesCoroutine != null) StopCoroutine(_initialTilesCoroutine);
        _initialTilesCoroutine = StartCoroutine(InitialTilesCoroutine());
    }

    private void AddElementMatchHistory(MatchData newMatch)
    {
        _matchsHistory.Add(newMatch);
    }
    private void RefillBoard()
    {
        StartCoroutine(RefillBoardCoroutine());
    }

    private IEnumerator RefillBoardCoroutine()
    {
        EventManager.BoardStateChanged(true);
        yield return new WaitForSeconds(0.01f);

        for (int x = 0; x < Width; x++)
        {
            MoveExistingTilesDown(x);

            int emptyCount = CountEmptyPositions(x);

            if (emptyCount > 0)
            {
                CreateNewTilesInColumn(x, emptyCount);
            }
        }
        yield return new WaitForSeconds(Config.TileMoveDuration);
        EventManager.BoardStateChanged(false);

        CheckAndDeleteMatches();
    }

    //Di chuyển các tile hiện có xuống các vị trí trống
    private void MoveExistingTilesDown(int column)
    {
        //check các vị trí trống từ dưới lên
        for (int y = 0; y < Height - 1; y++)
        {
            if (_emptys[column, y].Tile) continue;

            // Nếu vị trí này trống, tìm tile ở trên (nếu có) để di chuyển xuống
            for (int above = y + 1; above < Height; above++)
            {
                if (!_emptys[column, above].Tile) continue;

                // Di chuyển tile từ vị trí trên xuống vị trí trống
                Tile tile = _emptys[column, above].Tile;
                _emptys[column, above].Tile = null;
                tile.Empty = _emptys[column, y];
                break;
            }
        }
    }

    // Đếm số vị trí trống trong một cột
    private int CountEmptyPositions(int column)
    {
        int count = 0;
        for (int y = 0; y < Height; y++)
        {
            if (_emptys[column, y].Tile == null)
            {
                count++;
            }
        }
        return count;
    }

    // Tạo tile mới cho các vị trí trống trong một cột
    private void CreateNewTilesInColumn(int column, int emptyCount)
    {
        List<int> emptyPositions = new List<int>();
        for (int y = 0; y < Height; y++)
        {
            if (_emptys[column, y].Tile == null)
            {
                emptyPositions.Add(y);
            }
        }

        for (int i = 0; i < emptyPositions.Count; i++)
        {
            int targetY = emptyPositions[i];

            Vector2 startPos = new Vector2(column, Height + i);

            Tile newTile = PoolManager.Instance.GetObject<Tile>
                (PoolType.Tile, startPos, EmptyHolder.transform);
            newTile.InitialData(GetRandomTileType(), _emptys[column, targetY]);
        }
    }

    public List<MatchData> GetMatchHistory() => new List<MatchData>(_matchsHistory);
    public void ClearMatchHistory() => _matchsHistory.Clear();
    public void ClearMatchedTileViews()
    {
        foreach (var matchedTileView in _matchedTileViews)
        {
            PoolManager.Instance.ReturnObject(PoolType.MatchedTileView, matchedTileView.gameObject);
        }
        _matchedTileViews.Clear();
    }

    public Tile GetTileAtPos(Vector2Int position)
    {
        return _emptys[position.x, position.y].Tile;
    }
    public void SetBoardState(bool state) => BoardBg.SetActive(state);


}
