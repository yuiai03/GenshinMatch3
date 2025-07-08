using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    private Coroutine attackCoroutine;

    public override void GetData(EntityData entityData)
    {
        base.GetData(entityData);
        EventManager.MaxHPChanged(entityData.entityConfig.MaxHP, false);
    }
    public override void HPChanged(float hp)
    {
        EventManager.HPChanged(hp, false);
    }

    public override void Attack(Entity target)
    {
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine(target));
    }
    private IEnumerator AttackCoroutine(Entity target)
    {
        _entityAnim.Attack();
        yield return new WaitForSeconds(0.5f);
        var bullet = PoolManager.Instance.GetObject<EnemyBullet>(PoolType.EnemyBullet, shootPoint.position, transform);

        var matchData = new MatchData(TileType.Dendro, 5);
        bullet.Initialize(matchData);
        yield return new WaitForSeconds(1f);
        EventManager.GameStateChanged(GameState.EndRound);
    }
}
