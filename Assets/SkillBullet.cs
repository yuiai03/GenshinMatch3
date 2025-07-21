using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBullet : BulletBase
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.TakeDamage(_damage, _tileConfig.tileType);
                PoolManager.Instance.ReturnObject(PoolType.SkillBullet, gameObject);
            }
        }
        else if (collision.CompareTag("Shield"))
        {
            var shield = collision.GetComponent<Shield>();
            if (shield)
            {
                PoolManager.Instance.ReturnObject(PoolType.Shield, shield.gameObject);
                PoolManager.Instance.ReturnObject(PoolType.SkillBullet, gameObject);
            }
        }
        else if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<Player>();
            if (player)
            {
                player.TakeDamage(_damage, _tileConfig.tileType);
                PoolManager.Instance.ReturnObject(PoolType.SkillBullet, gameObject);
            }
        }
    }
    protected override IEnumerator FollowTargetCoroutine()
    {
        if (GameManager.Instance.IsSingleScene())
        {
            Vector2 targetPosition = SinglePlayerLevelManager.Instance.Enemy.transform.position;
            _direction = (targetPosition - (Vector2)transform.position).normalized;
            _rb2d.velocity = _direction * _speed;
            yield return null;
        }
        else
        {
            var isPlayer1 = MultiplayerGameManager.Instance.IsPlayer1Turn() 
                || MultiplayerGameManager.Instance.IsPlayer1EndTurn();
            Vector2 targetPosition = isPlayer1 
                ? MultiplayerLevelManager.Instance.Player2.transform.position 
                : MultiplayerLevelManager.Instance.Player1.transform.position;
            _direction = (targetPosition - (Vector2)transform.position).normalized;
            _rb2d.velocity = _direction * _speed;
            yield return null;
        }
    }
}
