using System.Collections;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private Transform shootPoint;
    private Coroutine attackCoroutine;
    public override void GetData(EntityData entityData)
    {
        base.GetData(entityData);
        EventManager.MaxHPChanged(entityData.entityConfig.HP, true);
    }
    public override void HPChanged(float hp)
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
            _entityAnim.Attack();
            yield return new WaitForSeconds(0.3f);
            var bullet = PoolManager.Instance.GetObject<PlayerBullet>(PoolType.PlayerBullet, shootPoint.position, transform);
            bullet.Initialize(matchData);
            yield return new WaitForSeconds(0.5f); 
        }
        _entityAnim.anim.state.AddAnimation(0, _entityAnim.idle, true, 0f);
    }
}
