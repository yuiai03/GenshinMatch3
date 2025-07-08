using UnityEngine;

public class Entity : MonoBehaviour
{
    private float _hp;
    public float HP
    {
        get => _hp;
        set
        {
            _hp = value;
            HPChanged(_hp);
        }
    }
    
    public TileType CurrentTileType { get; set; }
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
        _entityAnim.idle = _entityData.entityConfig.idle;
        _entityAnim.hurt = _entityData.entityConfig.hurt;
        _entityAnim.die = _entityData.entityConfig.die;
        _entityAnim.attack = _entityData.entityConfig.attack;
        _entityAnim.anim.skeletonDataAsset = _entityData.entityConfig.skeletonDataAsset;
        _entityAnim.anim.Initialize(true);
        _entityAnim.Idle();
    }

    public virtual void TakeDamage(float damage, TileConfig tileConfig)
    {
        float finalDamage = Helper.ElementalReaction(tileConfig, damage, this);
        ApplyDamage(finalDamage, tileConfig);
    }

    public virtual void ApplyDamage(float damage, TileConfig tileConfig)
    {
        HP -= damage;
        _entityAnim.Hurt();
        if (HP <= 0)
        {
            HP = 0;
            _entityAnim.Die();
        }

        var takeDamagePopup = PoolManager.Instance.GetObject<TextDamagePopup>(
            PoolType.TextDamagePopup, shootPoint.position, transform);
        takeDamagePopup.SetTakeDamageData((int)damage, tileConfig.color);
    }

    public virtual void HPChanged(float hp) { }
    public virtual void Attack(Entity target) { }
}
