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

    void Start()
    {
        InitialData();
        InitialEmpty();
        StartCoroutine(InitialTileWithFallingEffect());
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

    private IEnumerator InitialTileWithFallingEffect()
    {
        if (!_tilePrefab || _emptys.Length == 0) yield break;

        // Tạo các tile từ trái sang phải, từ dưới lên trên để có hiệu ứng rơi theo chuỗi
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                // Tạo tile ở vị trí trên màn hình
                Vector2 startPos = new Vector2(x, Height + y + 2);
                Tile tile = Instantiate(_tilePrefab, startPos, Quaternion.identity, transform);

                // Chọn loại tile không tạo thành match
                TileType tileType = GetNoMatchingTileType(x, y);

                // Thiết lập dữ liệu cho tile
                tile.InitialData(tileType, _emptys[x, y]);

                // Đợi một chút trước khi tạo tile tiếp theo để tạo hiệu ứng chuỗi
                yield return new WaitForSeconds(0.02f);
            }
        }

        // Đợi cho animation rơi hoàn tất
        yield return new WaitForSeconds(0.6f);

        // Kiểm tra xem có khả năng tạo match hay không
        if (!HasPotentialMatches())
        {
            Debug.Log("Không có khả năng tạo match, tạo lại bảng...");
            RecreateBoard();
        }
    }

    private TileType GetNoMatchingTileType(int x, int y)
    {
        // Mảng đánh dấu loại tile nào có thể sử dụng
        bool[] availableTypes = new bool[Config.TileTypesCount];
        for (int i = 0; i < Config.TileTypesCount; i++)
        {
            availableTypes[i] = true; // Ban đầu tất cả các loại đều khả dụng
        }

        // Kiểm tra match ngang
        if (x >= 2)
        {
            Tile tile1 = _emptys[x - 1, y].Tile;
            Tile tile2 = _emptys[x - 2, y].Tile;

            if (tile1 != null && tile2 != null && tile1.TileType == tile2.TileType)
            {
                // Loại bỏ loại tile sẽ tạo thành match
                int typeIndex = (int)tile1.TileType;
                if (typeIndex < Config.TileTypesCount)
                {
                    availableTypes[typeIndex] = false;
                }
            }
        }

        // Kiểm tra match dọc
        if (y >= 2)
        {
            Tile tile1 = _emptys[x, y - 1].Tile;
            Tile tile2 = _emptys[x, y - 2].Tile;

            if (tile1 != null && tile2 != null && tile1.TileType == tile2.TileType)
            {
                // Loại bỏ loại tile sẽ tạo thành match
                int typeIndex = (int)tile1.TileType;
                if (typeIndex < Config.TileTypesCount)
                {
                    availableTypes[typeIndex] = false;
                }
            }
        }

        // Tạo danh sách các loại tile khả dụng
        List<TileType> validTypes = new List<TileType>();
        for (int i = 0; i < Config.TileTypesCount; i++)
        {
            if (availableTypes[i])
            {
                validTypes.Add((TileType)i);
            }
        }

        // Nếu không còn loại tile nào khả dụng, trả về một loại ngẫu nhiên
        if (validTypes.Count == 0)
        {
            return GetRandomTileType();
        }

        // Chọn một loại tile ngẫu nhiên từ các loại khả dụng
        int randomIndex = UnityEngine.Random.Range(0, validTypes.Count);
        return validTypes[randomIndex];
    }

    private TileType GetRandomTileType()
    {
        return (TileType)UnityEngine.Random.Range(0, Config.TileTypesCount);
    }

    public void HandleSwapTiles(Tile selectedTile, Tile targetTile)
    {
        EventManager.StartSwapTileAction(selectedTile, targetTile);

        //Đổi Empty của 2 tile
        Helper.SwapEmpty(selectedTile, targetTile);

        //Kiểm tra xem có match nào không

        //Nếu có match thì thực hiện chuyển đổi Tile

        //Neeus không match thì đổi lại Empty cũ của 2 tiles
    }

    public void CheckAndDeleteMatches()
    {
        var matches = FindAllMatches();
        if (matches.Count > 0)
        {
            foreach (var match in matches)
            {
                ExplodeMatch(match);
                _matchHistory.Add(new MatchData(match.TileType, match.Tiles.Count));
            }
            RefillBoard();
        }
        else
        {
            // Kiểm tra xem còn khả năng tạo match không
            if (!HasPotentialMatches())
            {
                Debug.Log("Không có khả năng tạo match, tạo lại bảng...");
                RecreateBoard();
            }
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

    // Kiểm tra xem còn khả năng tạo match không
    private bool HasPotentialMatches()
    {
        // Kiểm tra tất cả các cặp tile kề nhau
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile currentTile = GetTileAtPos(new Vector2Int(x, y));
                if (currentTile == null) continue;

                // Kiểm tra swap với tile bên phải
                if (x < Width - 1)
                {
                    Tile rightTile = GetTileAtPos(new Vector2Int(x + 1, y));
                    if (rightTile != null)
                    {
                        // Swap tạm thời
                        SwapTileTypes(currentTile, rightTile);

                        // Kiểm tra có match không
                        bool hasMatch = HasMatchAt(x, y) || HasMatchAt(x + 1, y);

                        // Swap lại như cũ
                        SwapTileTypes(currentTile, rightTile);

                        if (hasMatch) return true;
                    }
                }

                // Kiểm tra swap với tile bên trên
                if (y < Height - 1)
                {
                    Tile upTile = GetTileAtPos(new Vector2Int(x, y + 1));
                    if (upTile != null)
                    {
                        // Swap tạm thời
                        SwapTileTypes(currentTile, upTile);

                        // Kiểm tra có match không
                        bool hasMatch = HasMatchAt(x, y) || HasMatchAt(x, y + 1);

                        // Swap lại như cũ
                        SwapTileTypes(currentTile, upTile);

                        if (hasMatch) return true;
                    }
                }
            }
        }

        return false;
    }

    // Đổi chỗ TileType của 2 tile
    private void SwapTileTypes(Tile tile1, Tile tile2)
    {
        TileType temp = tile1.TileType;
        tile1.TileType = tile2.TileType;
        tile2.TileType = temp;
    }

    // Kiểm tra xem có match tại vị trí (x, y) không
    private bool HasMatchAt(int x, int y)
    {
        // Kiểm tra match ngang
        if (x > 0 && x < Width - 1)
        {
            Tile left = GetTileAtPos(new Vector2Int(x - 1, y));
            Tile center = GetTileAtPos(new Vector2Int(x, y));
            Tile right = GetTileAtPos(new Vector2Int(x + 1, y));

            if (left != null && center != null && right != null &&
                left.TileType == center.TileType && center.TileType == right.TileType)
            {
                return true;
            }
        }

        // Kiểm tra match dọc
        if (y > 0 && y < Height - 1)
        {
            Tile down = GetTileAtPos(new Vector2Int(x, y - 1));
            Tile center = GetTileAtPos(new Vector2Int(x, y));
            Tile up = GetTileAtPos(new Vector2Int(x, y + 1));

            if (down != null && center != null && up != null &&
                down.TileType == center.TileType && center.TileType == up.TileType)
            {
                return true;
            }
        }

        return false;
    }

    // Tạo lại toàn bộ bảng
    private void RecreateBoard()
    {
        // Xóa tất cả tile hiện tại
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

        // Tạo lại bảng với hiệu ứng rơi
        StartCoroutine(InitialTileWithFallingEffect());
    }

    private void ExplodeMatch(Match match)
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
        // Đợi một chút để hoàn thành việc xóa các tile
        yield return new WaitForSeconds(0.2f);

        // Xử lý từng cột một
        for (int x = 0; x < Width; x++)
        {
            // Bước 1: Di chuyển các tile hiện có xuống các vị trí trống
            MoveExistingTilesDown(x);

            // Bước 2: Đếm số vị trí còn trống sau khi di chuyển
            int emptyCount = CountEmptySpaces(x);

            if (emptyCount > 0)
            {
                // Bước 3: Tạo tile mới từ trên xuống theo thứ tự
                CreateNewTilesInColumn(x, emptyCount);
            }
        }

        // Đợi cho animation di chuyển hoàn tất
        yield return new WaitForSeconds(0.6f);

        // Kiểm tra matches mới và kiểm tra xem bảng còn khả năng match không
        CheckAndDeleteMatches();
    }

    // Di chuyển các tile hiện có trong một cột xuống các vị trí trống
    private void MoveExistingTilesDown(int column)
    {
        for (int y = 0; y < Height - 1; y++)
        {
            // Nếu vị trí hiện tại trống
            if (_emptys[column, y].Tile == null)
            {
                // Tìm tile gần nhất phía trên vị trí này
                for (int above = y + 1; above < Height; above++)
                {
                    if (_emptys[column, above].Tile != null)
                    {
                        // Di chuyển tile xuống vị trí trống
                        Tile tile = _emptys[column, above].Tile;
                        _emptys[column, above].Tile = null;
                        tile.Empty = _emptys[column, y];
                        break;
                    }
                }
            }
        }
    }

    // Đếm số vị trí trống trong một cột
    private int CountEmptySpaces(int column)
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

            // Đợi một khoảng thời gian nhỏ giữa các lần tạo để tạo hiệu ứng chuỗi
            // Không đợi trong coroutine vì sẽ làm gián đoạn quy trình
        }
    }

    public List<MatchData> GetMatchHistory() => new List<MatchData>(_matchHistory);
    public void ClearMatchHistory() => _matchHistory.Clear();

    public Tile GetTileAtPos(Vector2Int position)
    {
        return _emptys[position.x, position.y].Tile;
    }
}
