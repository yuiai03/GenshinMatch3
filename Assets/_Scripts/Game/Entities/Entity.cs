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
    private EntityAnim _entityAnim;
    private EntityData _entityData;

    public virtual void GetData(EntityData entityData)
    {
        gameObject.name = EntityType.ToString();
        _entityAnim = GetComponent<EntityAnim>();
        _entityData = entityData;
        HP = entityData.entityConfig.HP;
        
        _entityAnim.idle = entityData.entityConfig.idle;
        _entityAnim.hurt = entityData.entityConfig.hurt;
        _entityAnim.die = entityData.entityConfig.die;
        _entityAnim.attack = entityData.entityConfig.attack;
        _entityAnim.anim.skeletonDataAsset = entityData.entityConfig.skeletonDataAsset;
        _entityAnim.anim.Initialize(true);
        _entityAnim.Idle();

    }

    public virtual void Hurt(float damage)
    {
        HP -= damage;
        _entityAnim.Hurt();
        if (HP <= 0)
        {
            HP = 0;
            _entityAnim.Die();
        }
    }

    public virtual void HPChanged(float hp)
    {

    }
}
