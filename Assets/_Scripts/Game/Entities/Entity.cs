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
    public EntityType EntityType;
    protected EntityAnim _entityAnim;
    protected EntityData _entityData;

    public virtual void GetData(EntityData entityData)
    {
        gameObject.name = EntityType.ToString();
        _entityAnim = GetComponent<EntityAnim>();
        _entityData = entityData;
        HP = _entityData.entityConfig.HP;
        
        _entityAnim.idle = _entityData.entityConfig.idle;
        _entityAnim.hurt = _entityData.entityConfig.hurt;
        _entityAnim.die = _entityData.entityConfig.die;
        _entityAnim.attack = _entityData.entityConfig.attack;
        _entityAnim.anim.skeletonDataAsset = _entityData.entityConfig.skeletonDataAsset;
        _entityAnim.anim.Initialize(true);
        _entityAnim.Idle();

    }

    public virtual void TakeDamage(float damage)
    {
        HP -= damage;
        _entityAnim.Hurt();
        if (HP <= 0)
        {
            HP = 0;
            _entityAnim.Die();
        }
    }

    public virtual void HPChanged(float hp) { }
    public virtual void Attack(Entity target) { }
}
