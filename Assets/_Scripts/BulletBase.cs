using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Collections;
using UnityEngine;

public class BulletBase : MonoBehaviourPunCallbacks
{
    protected Vector2 _direction = Vector2.right;
    protected int _damage = 0;
    protected float _speed = 10f;
    protected TileConfig _tileConfig;
    protected SpriteRenderer _spriteRenderer;
    protected TrailRenderer _trailRenderer;
    protected Rigidbody2D _rb2d;
    protected Coroutine _moveCoroutine;
    protected Coroutine _followCoroutine;
    
    protected virtual void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }
    
    public override void OnDisable()
    {
        base.OnDisable();
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
    }

    public void Initialize(MatchData matchData, bool followTarget = false, float speed = 10)
    {
        _tileConfig = GameManager.Instance.TileData.GetTileConfig(matchData.TileType);
        if (_tileConfig == null) return;
        if (MultiplayerLevelManager.Instance.IsPlayer1EndTurn())
        {
            gameObject.tag = "Player1Bullet";
        }
        else if (MultiplayerLevelManager.Instance.IsPlayer2EndTurn())
        {
            gameObject.tag = "Player2Bullet";
        }

        _spriteRenderer.color = _tileConfig.color;
        _trailRenderer.colorGradient = _tileConfig.gradient;
        _damage = matchData.Count;
        _speed = speed;

        if (!followTarget) 
            MoveToTarget();
        else 
            FollowTarget();
    }

    protected virtual void FollowTarget()
    {
        if (_followCoroutine != null) StopCoroutine(_followCoroutine);
        _followCoroutine = StartCoroutine(FollowTargetCoroutine());
    }

    protected virtual IEnumerator FollowTargetCoroutine()
    {
        yield return null;
    }

    protected virtual void MoveToTarget()
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(MoveToTargetCoroutine());
    }

    protected virtual IEnumerator MoveToTargetCoroutine()
    {
        _rb2d.velocity = _direction * _speed;
        yield return null;
    }
}
