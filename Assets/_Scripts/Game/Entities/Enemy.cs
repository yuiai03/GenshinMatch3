using UnityEngine;

public class Enemy : Entity
{
    public override void GetData(EntityData entityData)
    {
        base.GetData(entityData);
        EventManager.MaxHPChanged(entityData.entityConfig.HP, false);
    }
    public override void HPChanged(float hp)
    {
        EventManager.HPChanged(hp, false);
    }
}
