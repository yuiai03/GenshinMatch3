using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            var enemyBullet = collision.GetComponent<EnemyBullet>();
            if (enemyBullet)
            {
                PoolManager.Instance.ReturnObject(PoolType.EnemyBullet, enemyBullet.gameObject);
                PoolManager.Instance.ReturnObject(PoolType.Shield, gameObject);
            }
        }
    }
}
