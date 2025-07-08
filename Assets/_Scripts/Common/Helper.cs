using JetBrains.Annotations;
using UnityEngine;
using System.Collections.Generic;

public class Helper : MonoBehaviour
{
    private static readonly Dictionary<(TileType, TileType), ReactionType> ElementalReactionDictionary = 
        new Dictionary<(TileType, TileType), ReactionType>
        {
            { (TileType.Hydro, TileType.Pyro), ReactionType.Vaporize },
            { (TileType.Hydro, TileType.Cryo), ReactionType.Frozen },
            { (TileType.Hydro, TileType.Electro), ReactionType.ElectroCharged },
            { (TileType.Hydro, TileType.Dendro), ReactionType.Bloom },
            
            { (TileType.Pyro, TileType.Hydro), ReactionType.Vaporize },
            { (TileType.Pyro, TileType.Cryo), ReactionType.Melt },
            { (TileType.Pyro, TileType.Electro), ReactionType.Overloaded },
            { (TileType.Pyro, TileType.Dendro), ReactionType.Burning },
            
            { (TileType.Cryo, TileType.Hydro), ReactionType.Frozen },
            { (TileType.Cryo, TileType.Pyro), ReactionType.Melt },
            { (TileType.Cryo, TileType.Electro), ReactionType.Superconduct },
            
            { (TileType.Electro, TileType.Hydro), ReactionType.ElectroCharged },
            { (TileType.Electro, TileType.Pyro), ReactionType.Overloaded },
            { (TileType.Electro, TileType.Cryo), ReactionType.Superconduct },
            { (TileType.Electro, TileType.Dendro), ReactionType.Aggravate },
            
            { (TileType.Dendro, TileType.Hydro), ReactionType.Bloom },
            { (TileType.Dendro, TileType.Pyro), ReactionType.Burning },
            { (TileType.Dendro, TileType.Electro), ReactionType.Aggravate },
            
            { (TileType.Geo, TileType.Hydro), ReactionType.Crystallize },
            { (TileType.Geo, TileType.Pyro), ReactionType.Crystallize },
            { (TileType.Geo, TileType.Cryo), ReactionType.Crystallize },
            { (TileType.Geo, TileType.Electro), ReactionType.Crystallize },
            
            { (TileType.Anemo, TileType.Hydro), ReactionType.Swirl },
            { (TileType.Anemo, TileType.Pyro), ReactionType.Swirl },
            { (TileType.Anemo, TileType.Cryo), ReactionType.Swirl },
            { (TileType.Anemo, TileType.Electro), ReactionType.Swirl }
        };

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

    public static float ElementalReaction(TileConfig activeTileConfig, float baseDamage, Entity target)
    {
        if (target.CurrentTileType == TileType.None)
        {
            if (IsCurrentTileNonElement(activeTileConfig.tileType))
            {
                Debug.Log("Current tile is non-elemental, no reaction.");
                return baseDamage;
            }

            Debug.Log($"Applying {activeTileConfig.tileType} to target with no current tile type.");
            target.CurrentTileType = activeTileConfig.tileType;
            return baseDamage;
        }

        if (ElementalReactionDictionary.TryGetValue((activeTileConfig.tileType, target.CurrentTileType), out ReactionType reaction))
        {
            return ApplyElementalReaction(reaction, baseDamage, target, activeTileConfig);
        }
        return baseDamage;
    }

    private static float ApplyElementalReaction(ReactionType reaction, float baseDamage, Entity target, TileConfig activeTileConfig)
    {
        Debug.Log($"Elemental Reaction: {reaction}");

        float damage = baseDamage;
        switch (reaction)
        {
            case ReactionType.Vaporize:
                damage = baseDamage * 2f;
                break;

            case ReactionType.Melt:
                damage = baseDamage * 2f;
                break;

            case ReactionType.ElectroCharged:
                TargetApplyDamage(2f, target, activeTileConfig);
                break;

            case ReactionType.Overloaded:
                TargetApplyDamage(3f, target, activeTileConfig);
                break;

            case ReactionType.Superconduct:
                TargetApplyDamage(3f, target, activeTileConfig);
                break;

            case ReactionType.Crystallize:
                //CreateShield(target);
                break;

            case ReactionType.Swirl:
                TargetApplyDamage(5f, target, activeTileConfig);
                break;

            case ReactionType.Bloom:
                CreateDendroExplosion(target, 3f);
                break;

            case ReactionType.Burning:
                TargetApplyDamage(2f, target, activeTileConfig);
                break;

            case ReactionType.Frozen:
                TargetApplyDamage(2f, target, activeTileConfig);
                break;

            case ReactionType.Aggravate:
                TargetApplyDamage(2f, target, activeTileConfig);
                break;
        }
        target.CurrentTileType = TileType.None;
        return damage;
    }

    private static void TargetApplyDamage(float damage, Entity target, TileConfig activeTileConfig)
    {
        target.ApplyDamage(damage, activeTileConfig);
    }

    private static void CreateShield(Entity target)
    {
        Debug.Log("Shield created for target!");
    }

    private static void CreateDendroExplosion(Entity target, float damage)
    {
        //TargetApplyDamage(target, damage);
        Debug.Log($"Dendro explosion deals {damage} damage!");
    }

    private static bool IsCurrentTileNonElement(TileType activeTileTileType)
    {
        return activeTileTileType == TileType.Geo ||
            activeTileTileType == TileType.Anemo;
    }
}
