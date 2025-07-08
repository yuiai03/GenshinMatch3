using UnityEngine;

public class EnemyBullet : BulletBase
{
    protected override void Awake()
    {
        base.Awake();
        _direction = Vector2.left;
    }
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
}

