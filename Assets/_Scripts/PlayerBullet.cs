using Photon.Pun;
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
                enemy.TakeDamage(_damage, _tileConfig.tileType);
                PoolManager.Instance.ReturnObject(PoolType.PlayerBullet, gameObject);
            }
        }
        else if (collision.CompareTag("Shield"))
        {
            if (GameManager.Instance.IsSingleScene()) return;
            var shield = collision.GetComponent<Shield>();
            if (shield)
            {
                if (gameObject.tag == "Player1Bullet" && shield.PlayerOwner == MultiplayerLevelManager.Instance.Player2)
                {
                    PoolManager.Instance.ReturnObject(PoolType.Shield, shield.gameObject);
                    PoolManager.Instance.ReturnObject(PoolType.SkillBullet, gameObject);
                }
                if (gameObject.tag == "Player2Bullet" && shield.PlayerOwner == MultiplayerLevelManager.Instance.Player1)
                {
                    PoolManager.Instance.ReturnObject(PoolType.Shield, shield.gameObject);
                    PoolManager.Instance.ReturnObject(PoolType.SkillBullet, gameObject);
                }

            }
        }
        else if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance.IsSingleScene()) return;
            var player = collision.GetComponent<Player>();
            if (player)
            {
                if (player == MultiplayerLevelManager.Instance.Player2 && MultiplayerGameManager.Instance.IsPlayer1EndTurn())
                {
                    player.TakeDamage(_damage, _tileConfig.tileType);
                    PoolManager.Instance.ReturnObject(PoolType.PlayerBullet, gameObject);
                }
                else if (player == MultiplayerLevelManager.Instance.Player1 && MultiplayerGameManager.Instance.IsPlayer2EndTurn())
                {
                    player.TakeDamage(_damage, _tileConfig.tileType);
                    PoolManager.Instance.ReturnObject(PoolType.PlayerBullet, gameObject);
                }
            }
        }
    }

    protected override void MoveToTarget()
    {
        if (GameManager.Instance.IsSingleScene())
        {
            _direction = Vector2.right;
        }
        else
        {
            _direction = MultiplayerGameManager.Instance.IsPlayer1EndTurn() ? Vector2.right : Vector2.left;
        }
        base.MoveToTarget();
    }

    [PunRPC]
    public void SetTag(string tag)
    {
        gameObject.tag = tag;
    }
}
