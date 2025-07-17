public enum TileType
{
    Hydro,
    Pyro,
    Cryo,
    Electro,
    Dendro,
    Geo,
    Anemo,
    None
}

public enum ReactionType
{
    None,

    /// <summary>
    /// Bốc hơi
    /// </summary>
    Vaporize,

    /// <summary>
    /// Tan chảy
    /// </summary>
    Melt, 

    /// <summary>
    /// Đóng băng
    /// </summary>
    Freeze, 

    /// <summary>
    /// Quá tải
    /// </summary>
    Overloaded, 

    /// <summary>
    /// Siêu dẫn
    /// </summary>
    Superconduct, 

    /// <summary>
    /// điện cảm
    /// </summary>
    ElectroCharged, 
    
    /// <summary>
    /// Khuếch tán
    /// </summary>
    Swirl, 

    /// <summary>
    /// Kết tinh
    /// </summary>
    Crystallize,
    
    /// <summary>
    /// Thiêu đối
    /// </summary>
    Burning, 

    /// <summary>
    /// Nở rộ
    /// </summary>
    Bloom,

    /// <summary>
    /// Tăng cường
    /// </summary>
    Aggravate
}

public enum PoolType
{
    Empty,
    Tile,
    MatchedTileView,
    PlayerBullet,
    EnemyBullet,
    TextDamagePopup,
    Shield,
    DendroCore,
}

public enum EntityType
{
    //Player
    Buba,
    Olek, //Dendro
    Puffy, //Cryo
    Pomodoro, //Pyro
    Machito, //Elec
    Hybird, //Geo
    Aquatic, //Hydro

    //Enemy
    DryadFighter,
    BearMom,
    BearDad,
    Slime,
    SlimeForestB,
    TreantFlowering,
    TreantFighter,
    UFO,
}

public enum GameState
{
    GameEnded,
    GameStart,

    PlayerTurn,
    EnemyTurn,
    
    PlayerEndTurn,
    EnemyEndTurn,

    EndRound,
    
}

public enum SceneType
{
    MainMenu,
    SinglePlayer,
    Multiplayer,
}

public enum LevelType
{
    Level_1_1,
    Level_1_2,
    Level_1_3,
    Level_1_4,
    Level_1_5,
}

public enum MainButtonType
{
    Map,
    Character,
    PvP
}
