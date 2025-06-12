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
        return (TileType)Random.Range(0, Config.TileTypesCount);
    }

    public bool SwapTiles(Tile selectedTile, Tile targetTile)
    {
        var tempEmpty = selectedTile.Empty;
        selectedTile.Empty = targetTile.Empty;
        targetTile.Empty = tempEmpty;
        return true;
    }

    //public void CheckAndExplodeMatches()
    //{
    //    var matches = FindAllMatches();
    //    if (matches.Count > 0)
    //    {
    //        foreach (var match in matches)
    //        {
    //            ExplodeMatch(match);
    //            _matchHistory.Add(new MatchData(match.TileType, match.Tiles.Count));
    //        }
    //        RefillBoard();
    //        CheckAndExplodeMatches(); // Kiểm tra lại nếu có match mới sau khi refill
    //    }
    //}

    //private List<Match> FindAllMatches()
    //{
    //    var matches = new List<Match>();
    //    for (int x = 0; x < Width; x++)
    //    {
    //        for (int y = 0; y < Height; y++)
    //        {
    //            var horizontal = CheckHorizontalMatch(x, y);
    //            if (horizontal.Tiles.Count >= 3) matches.Add(horizontal);
    //            var vertical = CheckVerticalMatch(x, y);
    //            if (vertical.Tiles.Count >= 3) matches.Add(vertical);
    //        }
    //    }
    //    return matches;
    //}
    //private Match CheckHorizontalMatch(int x, int y)
    //{
    //    if (y + 2 >= Height) return new Match();
    //    var tiles = new List<Tile> { GetTileAtPos(new Vector2Int(x,y)) };
    //    var type = tiles[0]?.TileType ?? TileType.Hydro; // Lấy loại của ô đầu tiên
    //    for (int i = 1; i < 3; i++)
    //    {
    //        Tile nextTile = GetTileAtPos(new Vector2Int(x, y + i));
    //        if (nextTile != null && nextTile.TileType == type) tiles.Add(nextTile);
    //        else break;
    //    }
    //    return new Match { TileType = type, Tiles = tiles };
    //}

    //private Match CheckVerticalMatch(int x, int y)
    //{
    //    if (x + 2 >= Width) return new Match();
    //    var tiles = new List<Tile> { GetTileAtPos(new Vector2Int(x, y)) };
    //    var type = tiles[0]?.TileType ?? TileType.Hydro;
    //    for (int i = 1; i < 3; i++)
    //    {
    //        Tile nextTile = GetTileAtPos(new Vector2Int(x + i, y));
    //        if (nextTile != null && nextTile.TileType == type) tiles.Add(nextTile);
    //        else break;
    //    }
    //    return new Match { TileType = type, Tiles = tiles };
    //}

    //private void ExplodeMatch(Match match)
    //{
    //    foreach (var tile in match.Tiles)
    //    {
    //        emptys[tile.IntPos.x, tile.IntPos.y] = null;
    //        Destroy(tile.gameObject);
    //    }
    //}

    //private void RefillBoard()
    //{
    //    for (int x = 0; x < Width; x++)
    //    {
    //        for (int y = 0; y < Height; y++)
    //        {
    //            if (emptys[x, y] == null)
    //            {
    //                Tile tile = Instantiate(tilePrefab, new Vector2(x, y), Quaternion.identity, transform);
    //                tile.InitialData(GetRandomTileType(), new Vector2Int(x, y), emptys[x,y]);
    //                emptys[x, y].Tile = tile;
    //            }
    //        }
    //    }
    //}

    public List<MatchData> GetMatchHistory() => new List<MatchData>(_matchHistory);

    public void ClearMatchHistory() => _matchHistory.Clear();

    //public Tile GetTileAtPos(Vector2Int position)
    //{
    //    return _emptys[position.x, position.y];
    //}
}

public class Match
{
    public TileType TileType { get; set; }
    public List<Tile> Tiles { get; set; } = new List<Tile>();
}
