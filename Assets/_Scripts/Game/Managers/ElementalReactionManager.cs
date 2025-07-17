using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalReactionManager : Singleton<ElementalReactionManager>
{
    private Coroutine _targetApplyDamageCoroutine;
    private Dictionary<(TileType, TileType), ReactionType> _elementalReactionDictionary =
        new Dictionary<(TileType, TileType), ReactionType>
        {
            { (TileType.Hydro, TileType.Pyro), ReactionType.Vaporize },
            { (TileType.Hydro, TileType.Cryo), ReactionType.Freeze },
            { (TileType.Hydro, TileType.Electro), ReactionType.ElectroCharged },
            { (TileType.Hydro, TileType.Dendro), ReactionType.Bloom },

            { (TileType.Pyro, TileType.Hydro), ReactionType.Vaporize },
            { (TileType.Pyro, TileType.Cryo), ReactionType.Melt },
            { (TileType.Pyro, TileType.Electro), ReactionType.Overloaded },
            { (TileType.Pyro, TileType.Dendro), ReactionType.Burning },

            { (TileType.Cryo, TileType.Hydro), ReactionType.Freeze },
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

    public ElementalReactionData ElementalReaction(TileConfig activeTileConfig, float baseDamage, Entity target)
    {
        if (target.CurrentTileType == TileType.None)
        {
            if (IsCurrentTileNonElement(activeTileConfig.tileType))
                return new ElementalReactionData(baseDamage, ReactionType.None);

            target.CurrentTileType = activeTileConfig.tileType;
            return new ElementalReactionData(baseDamage, ReactionType.None);
        }

        if (_elementalReactionDictionary.TryGetValue((activeTileConfig.tileType, target.CurrentTileType), out ReactionType reaction))
        {
            return ApplyElementalReaction(new ElementalReactionData(baseDamage, reaction), target, activeTileConfig);
        }
        return new ElementalReactionData(baseDamage, ReactionType.None);
    }

    private ElementalReactionData ApplyElementalReaction(ElementalReactionData elementalReactionData, Entity target, TileConfig activeTileConfig)
    {

        float finalDamage = elementalReactionData.damage;
        switch (elementalReactionData.reactionType)
        {
            case ReactionType.Vaporize or ReactionType.Melt:
                finalDamage = elementalReactionData.damage * Config.ReactionDamageMultiplier;
                target.CurrentTileType = TileType.None;
                break;

            case ReactionType.ElectroCharged or ReactionType.Overloaded or ReactionType.Superconduct or ReactionType.Aggravate:
                TargetApplyDamage(new ElementalReactionData(Config.ReactionDamageBonus, elementalReactionData.reactionType), target, activeTileConfig);
                target.CurrentTileType = TileType.None;
                break;

            case ReactionType.Swirl:
                var swirlTileConfig = Helper.GetTileConfig(target.CurrentTileType);
                TargetApplyDamage(new ElementalReactionData(Config.SwirlDamage, elementalReactionData.reactionType), target, swirlTileConfig, Config.SwirlCount);
                break;

            case ReactionType.Burning:
                var burningTileConfig = Helper.GetTileConfig(TileType.Pyro);
                TargetApplyDamage(new ElementalReactionData(Config.BurningDamage, elementalReactionData.reactionType), target, burningTileConfig, Config.BurningCount);
                target.CurrentTileType = TileType.Pyro;
                break;

            case ReactionType.Crystallize:
                CreateShield(target);
                target.CurrentTileType = TileType.None;
                break;


            case ReactionType.Bloom:
                CreateDendroExplosion(target, Config.DendroCoreExplosionDamage);
                target.CurrentTileType = TileType.None;
                break;


            case ReactionType.Freeze:
                TargetApplyDamage(new ElementalReactionData(Config.FreezeDamage, elementalReactionData.reactionType), target, activeTileConfig);
                target.CurrentTileType = TileType.None;
                target.IsFreeze = true;
                break;
        }

        return new ElementalReactionData(finalDamage, elementalReactionData.reactionType);
    }

    private void TargetApplyDamage(ElementalReactionData elementalReactionData, Entity target, TileConfig activeTileConfig, int count = 1)
    {
        if (_targetApplyDamageCoroutine != null) StopCoroutine(_targetApplyDamageCoroutine);
        _targetApplyDamageCoroutine = StartCoroutine(TargetApplyDamageCoroutine(elementalReactionData, target, activeTileConfig, count));
    }

    private IEnumerator TargetApplyDamageCoroutine(ElementalReactionData elementalReactionData, Entity target, TileConfig activeTileConfig, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(Config.ApplyReactionTime / count);
            target.ApplyDamage(activeTileConfig, elementalReactionData);
        }
    }

    private void CreateShield(Entity target)
    {
        var player = SinglePlayerLevelManager.Instance.Player;
        var pos = new Vector2(player.transform.position.x + 0.1f, player.transform.position.y + 0.6f);
        var shield = PoolManager.Instance.GetObject<Shield>(PoolType.Shield, pos, player.transform);
    }

    private void CreateDendroExplosion(Entity target, float damage)
    {
        Vector3 dropStartPosition = target.transform.position + Vector3.up * 0.5f;
        Vector3 targetPosition = target.transform.position;
        
        var dendroCore = PoolManager.Instance.GetObject<DendroCore>(PoolType.DendroCore, dropStartPosition, null);
        dendroCore.Initialize(dropStartPosition, targetPosition, damage);
    }

    private bool IsCurrentTileNonElement(TileType activeTileTileType)
    {
        return activeTileTileType == TileType.Geo || activeTileTileType == TileType.Anemo;
    }
}
