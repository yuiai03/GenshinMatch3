using System.Collections;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private int _damage;
    private float speed = 10f;
    private SpriteRenderer _spriteRenderer;
    private TrailRenderer _trailRenderer;
    private Coroutine moveCoroutine;
    private Rigidbody2D rb2d;
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    public void Initialize(MatchData matchData)
    {
        var tileConfig = GameManager.Instance.TileData.GetTileConfig(matchData.TileType);
        if (tileConfig == null) return;

        _spriteRenderer.color = tileConfig.color;
        _trailRenderer.colorGradient = tileConfig.gradient;
        _damage = matchData.Count;

        MoveToTarget();
    }

    public void MoveToTarget()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToTargetCoroutine());
    }

    private IEnumerator MoveToTargetCoroutine()
    {
        float distance = speed * Time.deltaTime;
        rb2d.linearVelocity = Vector2.right * speed;
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.TakeDamage(_damage);
                PoolManager.Instance.ReturnObject(PoolType.PlayerBullet, gameObject);
            }
        }
    }
}
