using System.Collections;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    protected Vector2 _direction = Vector2.right;
    protected int _damage = 0;
    protected float _speed = 10f;
    private SpriteRenderer _spriteRenderer;
    private TrailRenderer _trailRenderer;
    private Rigidbody2D _rb2d;
    private Coroutine _moveCoroutine;
    protected virtual void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
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
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(MoveToTargetCoroutine());
    }

    private IEnumerator MoveToTargetCoroutine()
    {
        float distance = _speed * Time.deltaTime;
        _rb2d.linearVelocity = _direction * _speed;
        yield return null;
    }
}
