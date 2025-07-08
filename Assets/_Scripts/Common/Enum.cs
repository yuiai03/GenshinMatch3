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
    MatchedTileView,
    PlayerBullet,
    EnemyBullet,
    TextDamagePopup,
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
    
    PlayerEndTurn,
    EnemyEndTurn,

    GamePause,
}

public enum SceneType
{
    Map,
    Game
}

public enum LevelType
{
    Level_1_1,
    Level_1_2,
    Level_1_3,
    Level_1_4,
    Level_1_5,
}