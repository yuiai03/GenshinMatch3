using System.Collections;
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

    protected override IEnumerator FollowTargetCoroutine()
    {
        Vector2 targetPosition = LevelManager.Instance.Enemy.transform.position;
        _direction = (targetPosition - (Vector2)transform.position).normalized;
        _rb2d.velocity = _direction * _speed;
        yield return null;
    }

    protected override void MoveToTarget()
    {
        _direction = Vector2.right;
        base.MoveToTarget();
    }
}
