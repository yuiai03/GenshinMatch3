using JetBrains.Annotations;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Helper : MonoBehaviour
{
    public static void SwapEmpty(Tile tile1, Tile tile2)
    {
        var tempEmpty = tile1.Empty;
        tile1.Empty = tile2.Empty;
        tile2.Empty = tempEmpty;
    }

    public static void SwapTileTypes(Tile tile1, Tile tile2)
    {
        TileType temp = tile1.TileType;
        tile1.TileType = tile2.TileType;
        tile2.TileType = temp;
    }
  
    public static string ReactionText(ReactionType reactionType) => reactionType switch
    {
        ReactionType.Vaporize => "Bốc hơi",
        ReactionType.Melt => "Tan chảy", 
        ReactionType.Freeze => "Đóng băng",
        ReactionType.Overloaded => "Quá tải",
        ReactionType.Superconduct => "Siêu dẫn",
        ReactionType.ElectroCharged => "Điện cảm",
        ReactionType.Swirl => "Khuếch tán",
        ReactionType.Crystallize => "Kết tinh",
        ReactionType.Burning => "Thiêu đối",
        ReactionType.Bloom => "Nở rộ",
        ReactionType.Aggravate => "Tăng cường",
        _ => ""
    };

    public static TileConfig GetTileConfig(TileType tileType)
    {
        return GameManager.Instance.TileData.GetTileConfig(tileType);
    }

    public static TileType GetCharacterElemental(EntityType entityType) => entityType switch
    {
        EntityType.Buba => TileType.None,  
        EntityType.Olek => TileType.Dendro,    
        EntityType.Puffy => TileType.Cryo,  
        EntityType.Pomodoro => TileType.Pyro,
        EntityType.Machito => TileType.Electro,
        EntityType.Hybird => TileType.Geo,
        EntityType.Aquatic => TileType.Hydro,
        _ => TileType.None
    };

    public static string BuffText(EntityType entityType) => entityType switch
    {
        EntityType.Olek => "Thảo",
        EntityType.Puffy => "Băng",
        EntityType.Pomodoro => "Hỏa",
        EntityType.Machito => "Lôi",
        EntityType.Hybird => "Nham",
        EntityType.Aquatic => "Thủy",
        _ => ""
    };
}
