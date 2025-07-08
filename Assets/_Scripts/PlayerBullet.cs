using UnityEngine;

public class PlayerBullet : BulletBase
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<Enemy>();
            if (enemy) 
            {
                enemy.TakeDamage(_damage, _tileConfig);
                PoolManager.Instance.ReturnObject(PoolType.PlayerBullet, gameObject);
            }
        }
    }
}
