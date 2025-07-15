using System.Collections;
using UnityEngine;

public class Player : Entity
{
    private Coroutine attackCoroutine;
    public override void GetData(EntityData entityData)
    {
        base.GetData(entityData);
        EventManager.MaxHPChanged(entityData.entityConfig.MaxHP, true);
    }
    protected override void HPChanged(float hp)
    {
        EventManager.HPChanged(hp, true);
    }
    public override void Attack(Entity target)
    {
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine(target));
    }
    private IEnumerator AttackCoroutine(Entity target)
    {
        var matchsHistory = BoardManager.Instance.GetMatchHistory();
        foreach (var matchData in matchsHistory)
        {
            if(Helper.GetCharacterElemental(GameManager.Instance.PlayerType) == matchData.TileType)
            {
                matchData.Count++;
            }

            _entityAnim.Attack();
            yield return new WaitForSeconds(0.3f);
            var bullet = PoolManager.Instance.GetObject<PlayerBullet>(PoolType.PlayerBullet, shootPoint.position, transform);
            bullet.Initialize(matchData);
            yield return new WaitForSeconds(0.5f); 
        }
        yield return new WaitForSeconds(1f);
        EventManager.GameStateChanged(GameState.EnemyTurn);
    }

    protected override void CurrentTileTypeChanged()
    {
        EventManager.CurrentTileTypeChanged(CurrentTileType, true);
    }
}
