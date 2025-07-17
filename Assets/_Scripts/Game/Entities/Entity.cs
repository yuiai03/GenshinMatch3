using System.Net.Http.Headers;
using UnityEngine;
using Photon.Pun;

public class Entity : MonoBehaviourPunCallbacks
{
    private float _hp;
    public float HP
    {
        get => _hp;
        set
        {
            _hp = value;
            HPChanged(_hp);
         
            if (_hp <= 0) EventManager.GameStateChanged(GameState.GameEnded);
        }
    }
    private TileType _currentTileType;
    public TileType CurrentTileType
    {
        get => _currentTileType;
        set
        {
            _currentTileType = value;

            CurrentTileTypeChanged();
        }
    }
    public bool IsFreeze { get; set; }
    public bool IsShield { get; set; }
    protected EntityAnim _entityAnim { get; private set; }
    protected EntityData _entityData {get; private set;}

    [SerializeField] protected Transform shootPoint;

    public virtual void GetData(EntityData entityData)
    {
        _entityData = entityData;
        gameObject.name = _entityData.entityConfig.entityType.ToString();
        HP = _entityData.entityConfig.MaxHP;
        CurrentTileType = TileType.None;
        
        _entityAnim = GetComponent<EntityAnim>();
        if (_entityAnim != null)
        {
            _entityAnim.idle = _entityData.entityConfig.idle;
            _entityAnim.hurt = _entityData.entityConfig.hurt;
            _entityAnim.die = _entityData.entityConfig.die;
            _entityAnim.attack = _entityData.entityConfig.attack;
            _entityAnim.anim.skeletonDataAsset = _entityData.entityConfig.skeletonDataAsset;
            _entityAnim.anim.Initialize(true);
            _entityAnim.Idle();
        }
    }

    public virtual void TakeDamage(float damage, TileConfig tileConfig)
    {
        if (IsShield) return;

        var elementalReactionData = ElementalReactionManager.Instance.ElementalReaction(tileConfig, damage, this);
        ApplyDamage(tileConfig, elementalReactionData);
    }

    public virtual void ApplyDamage( TileConfig tileConfig, ElementalReactionData elementalReactionData)
    {
        var data = elementalReactionData;
        HP -= data.damage;
        _entityAnim.Hurt();
        if (HP <= 0)
        {
            HP = 0;
            _entityAnim.Die();
        }

        var takeDamagePopup = PoolManager.Instance.GetObject<TextDamagePopup>(
            PoolType.TextDamagePopup, shootPoint.position, transform);
        takeDamagePopup.SetTakeDamageData(tileConfig.color, data);
    }

    public virtual void Attack(Entity target) { }
    protected virtual void HPChanged(float hp) { }
    protected virtual void CurrentTileTypeChanged() { }
}
