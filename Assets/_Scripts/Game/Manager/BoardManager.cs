using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    private Tile _tilePrefab;
    private Empty _emptyPrefab;
    private Empty[,] _emptys;
    private List<MatchData> _matchHistory = new List<MatchData>();
    public GameObject EmptyHolder { get; private set; }
    public int Width { get; } = Config.BoardWidth;
    public int Height { get; } = Config.BoardHeight;

    private Coroutine _initialTilesCoroutine;

    void Start()
    {
        InitialData();
        InitialEmpty();
        InitialTiles();
    }

    private void InitialData()
    {
        _tilePrefab = LoadManager.PrefabLoad<Tile>("Tile");
        _emptyPrefab = LoadManager.PrefabLoad<Empty>("Empty");
        EmptyHolder = transform.GetChild(0).gameObject;
    }

    private void InitialEmpty()
    {
        if (!_emptyPrefab) return;

        _emptys = new Empty[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Empty empty = Instantiate(_emptyPrefab, new Vector2(x, y), Quaternion.identity);
                empty.transform.SetParent(EmptyHolder.transform);
                _emptys[x, y] = empty;
                _emptys[x, y].IntPos = new Vector2Int(x, y);
            }
        }
    }
    private void InitialTiles()
    {
        if(_initialTilesCoroutine != null) StopCoroutine(_initialTilesCoroutine);
        _initialTilesCoroutine = StartCoroutine(InitialTilesCoroutine());
    }

    private IEnumerator InitialTilesCoroutine()
    {
        if (!_tilePrefab || _emptys.Length == 0) yield break;

        EventManager.BoardStateChanged(true);

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector2 startPos = new Vector2(x, Height + y);
                Tile tile = Instantiate(_tilePrefab, startPos, Quaternion.identity, transform);

                //Lấy loại tile không matching ngay khi vừa tạo
                TileType tileType = GetNoMatchingTileType(x, y);

                tile.InitialData(tileType, _emptys[x, y]);

                yield return new WaitForSeconds(0.01f);
            }
        }
        yield return new WaitForSeconds(Config.TileMoveDuration);

        EventManager.BoardStateChanged(false);

        //Nếu không có tile nào có khả năng tạo match
        if (!BoardCanMatches()) RecreateBoard();
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
        EventManager.StartSwapTileAction(selectedTile, targetTile);

        Helper.SwapEmpty(selectedTile, targetTile);
    }

    public void CheckAndDeleteMatches()
    {
        var matches = FindAllMatches();
        if (matches.Count > 0)
        {
            foreach (var match in matches)
            {
                ClearMatch(match);
                _matchHistory.Add(new MatchData(match.TileType, match.Tiles.Count));
            }
            RefillBoard();
        }
        else
        {
            //Nếu không có tile nào có khả năng tạo match
            if (!BoardCanMatches()) RecreateBoard();
        }
    }

    private List<Match> FindAllMatches()
    {
        var matches = new List<Match>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var horizontal = CheckHorizontalMatch(x, y);
                if (horizontal.Tiles.Count >= 3) matches.Add(horizontal);
                var vertical = CheckVerticalMatch(x, y);
                if (vertical.Tiles.Count >= 3) matches.Add(vertical);
            }
        }
        return matches;
    }

    private Match CheckHorizontalMatch(int x, int y)
    {
        if (y + 2 >= Height) return new Match();

        var firstTile = GetTileAtPos(new Vector2Int(x, y));
        // Kiểm tra nếu tile đầu tiên là null
        if (firstTile == null) return new Match();

        var tiles = new List<Tile> { firstTile };
        var type = firstTile.TileType;

        for (int i = 1; i < 3; i++)
        {
            Tile nextTile = GetTileAtPos(new Vector2Int(x, y + i));
            if (nextTile != null && nextTile.TileType == type) tiles.Add(nextTile);
            else break;
        }
        return new Match { TileType = type, Tiles = tiles };
    }

    private Match CheckVerticalMatch(int x, int y)
    {
        if (x + 2 >= Width) return new Match();

        var firstTile = GetTileAtPos(new Vector2Int(x, y));
        // Kiểm tra nếu tile đầu tiên là null
        if (firstTile == null) return new Match();

        var tiles = new List<Tile> { firstTile };
        var type = firstTile.TileType;

        for (int i = 1; i < 3; i++)
        {
            Tile nextTile = GetTileAtPos(new Vector2Int(x + i, y));
            if (nextTile != null && nextTile.TileType == type) tiles.Add(nextTile);
            else break;
        }
        return new Match { TileType = type, Tiles = tiles };
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
                if (_emptys[x, y].Tile != null)
                {
                    Destroy(_emptys[x, y].Tile.gameObject);
                    _emptys[x, y].Tile = null;
                }
            }
        }

        // Tạo lại bảng
        if (_initialTilesCoroutine != null) StopCoroutine(_initialTilesCoroutine);
        _initialTilesCoroutine = StartCoroutine(InitialTilesCoroutine());
    }

    private void ClearMatch(Match match)
    {
        foreach (var tile in match.Tiles)
        {
            _emptys[tile.Empty.IntPos.x, tile.Empty.IntPos.y].Tile = null;
            Destroy(tile.gameObject);
        }
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

            //Đếm số vị trí còn trống sau khi di chuyển
            int emptyCount = CountEmptyPositions(x);

            if (emptyCount > 0)
            {
                // Tạo tile mới từ trên xuống theo thứ tự
                CreateNewTilesInColumn(x, emptyCount);
            }
        }
        yield return new WaitForSeconds(Config.TileMoveDuration);
        EventManager.BoardStateChanged(false);

        // Kiểm tra matches mới và kiểm tra xem bảng còn khả năng match không
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
        // Thu thập tất cả các vị trí trống
        List<int> emptyPositions = new List<int>();
        for (int y = 0; y < Height; y++)
        {
            if (_emptys[column, y].Tile == null)
            {
                emptyPositions.Add(y);
            }
        }

        // Tạo tile mới cho từng vị trí trống, xếp chúng thẳng hàng từ trên xuống
        for (int i = 0; i < emptyPositions.Count; i++)
        {
            int targetY = emptyPositions[i];

            // Vị trí bắt đầu - xếp theo thứ tự ở phía trên bảng
            Vector2 startPos = new Vector2(column, Height + i);

            // Tạo tile mới
            Tile newTile = Instantiate(_tilePrefab, startPos, Quaternion.identity, transform);
            newTile.InitialData(GetRandomTileType(), _emptys[column, targetY]);
        }
    }

    public List<MatchData> GetMatchHistory() => new List<MatchData>(_matchHistory);
    public void ClearMatchHistory() => _matchHistory.Clear();

    public Tile GetTileAtPos(Vector2Int position)
    {
        return _emptys[position.x, position.y].Tile;
    }
}
