using System.Collections;
using UnityEngine;
using DG.Tweening;

public class DendroCore : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 2f;
    
    private float _currentDamage;
    private Vector3 _targetPosition;
    private Sequence _animationSequence;
    private Coroutine _explosionCoroutine;

    public void Initialize(Vector3 dropStartPosition, Vector3 targetPosition, float damage)
    {
        _currentDamage = damage;
        _targetPosition = targetPosition;
        transform.position = dropStartPosition + Vector3.up * Config.DendroCoreDropHeight;
        
        StartDropAnimation();
    }

    private void StartDropAnimation()
    {
        var dropTween = transform.DOMove(_targetPosition, Config.DendroCoreDropDuration)
            .SetEase(Ease.OutBounce);
        
        var impactScale = DOTween.Sequence()
            .Append(transform.DOScale(0.8f, 0.1f).SetEase(Ease.Linear))
            .Append(transform.DOScale(1.2f, 0.1f).SetEase(Ease.Linear))
            .SetLoops(5, LoopType.Yoyo);

        _animationSequence?.Kill();
        _animationSequence = DOTween.Sequence();
        _animationSequence
            .Append(dropTween)
            .Append(impactScale)
            .AppendCallback(() => Explosion())
            .SetTarget(this);
    }

    private void Explosion()
    {
        if(_explosionCoroutine != null) StopCoroutine(_explosionCoroutine);
        _explosionCoroutine = StartCoroutine(ExplosionCoroutine());
    }

    private IEnumerator ExplosionCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        ApplyExplosionDamage();
        PoolManager.Instance.ReturnObject(PoolType.DendroCore, gameObject);

    }

    private void ApplyExplosionDamage()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        
        foreach (var collider in colliders)
        {
            if (GameManager.Instance.IsSingleScene())
            {
                var enemy = collider.GetComponent<Enemy>();
                if (enemy)
                {
                    var dendroConfig = Helper.GetTileConfig(TileType.Dendro);
                    enemy.TakeDamage(_currentDamage, dendroConfig.tileType);
                }
            }
            else
            {
                var player = collider.GetComponent<Player>();
                if (player)
                {
                    var dendroConfig = Helper.GetTileConfig(TileType.Dendro);
                    player.TakeDamage(_currentDamage, dendroConfig.tileType);
                }
            }
        }
    }

    private void OnDisable()
    {
        _animationSequence?.Kill();
        if (_explosionCoroutine != null) StopCoroutine(_explosionCoroutine);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
        
        // Draw drop path
        if (_targetPosition != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _targetPosition);
        }
    }
}
