using UnityEngine;

public class Enemy : Entity
{
    public override void GetData(EntityData entityData)
    {
        base.GetData(entityData);
        UIManager.Instance.GamePanel.SetPlayerMaxHealth(entityData.entityConfig.HP);
    }
    public override void HPChanged(float hp)
    {
        UIManager.Instance.GamePanel.UpdatePlayerHealth(hp);
    }
}
