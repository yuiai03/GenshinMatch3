public enum TileType
{
    Hydro,
    Pyro,
    Cryo,
    Electro,
    Dendro,
    Geo,
    Anemo,
}

public enum PoolType
{
    Empty,
    Tile,
    MatchedTileView
}

public enum EntityType
{
    //Player
    Buba,
    Olek,
    Puffy,
    Ena,

    //Enemy
    TreantFlowering,
    TreantFighter,
    Slime,
    SlimeForestB,
    BearMom,
    BearDad,
    DryadFighter,
    UFO,
}

public enum GameState
{
    GameWaiting,
    GameStart,
    GameOver,
    GameWin,

    PlayerTurn,
    EnemyTurn,
    
    PlayerEndedAction,
    EnemyEndedAction,

    GamePause,
}

public enum SceneType
{
    Map,
    Game
}