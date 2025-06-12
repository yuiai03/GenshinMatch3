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
        InitialTile();
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
    private void InitialTile()
    {
        if (!_tilePrefab || _emptys.Length == 0) return;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile tile = Instantiate(_tilePrefab);
                tile.InitialData(GetRandomTileType(), _emptys[x, y]);
            }
        }
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
            // Đếm số khoảng trống trong mỗi cột
            int emptyCount = 0;

            // Từ dưới lên trên, đếm khoảng trống và di chuyển tile xuống
            for (int y = 0; y < Height; y++)
            {
                if (_emptys[x, y].Tile == null)
                {
                    emptyCount++;
                }
                else if (emptyCount > 0)
                {
                    // Di chuyển tile xuống emptyCount vị trí
                    Tile tile = _emptys[x, y].Tile;
                    _emptys[x, y].Tile = null;

                    // Gán tile vào vị trí mới
                    tile.Empty = _emptys[x, y - emptyCount];
                }
            }

            // Tạo tile mới ở phía trên cùng
            for (int i = 0; i < emptyCount; i++)
            {
                int newY = Height - i - 1;

                // Vị trí bắt đầu (trên cùng màn hình)
                Vector2 startPos = new Vector2(x, Height + i + 1);

                // Tạo tile mới
                Tile newTile = Instantiate(_tilePrefab, startPos, Quaternion.identity, transform);
                newTile.InitialData(GetRandomTileType(), _emptys[x, newY]);
            }
        }

        // Đợi cho animation di chuyển hoàn tất
        yield return new WaitForSeconds(0.6f);

        CheckAndDeleteMatches();
    }
    public List<MatchData> GetMatchHistory() => new List<MatchData>(_matchHistory);

    public void ClearMatchHistory() => _matchHistory.Clear();

    public Tile GetTileAtPos(Vector2Int position)
    {
        return _emptys[position.x, position.y].Tile;
    }
}

public class Match
{
    public TileType TileType { get; set; }
    public List<Tile> Tiles { get; set; } = new List<Tile>();
}
