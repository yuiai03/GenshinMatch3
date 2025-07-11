using System.Collections;
using UnityEngine;

public class EnemyBullet : BulletBase
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<Player>();
            if (player)
            {
                player.TakeDamage(_damage, _tileConfig);
                PoolManager.Instance.ReturnObject(PoolType.EnemyBullet, gameObject);
            }
        }
    }

    protected override void MoveToTarget()
    {
        _direction = Vector2.left; 
        base.MoveToTarget();
    }
}

