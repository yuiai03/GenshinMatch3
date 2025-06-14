using UnityEngine;

public class Entity : MonoBehaviour
{
    public EntityType EntityType;
    private EntityAnim _entityAnim;

    protected virtual void Awake()
    {
        gameObject.name = EntityType.ToString();
        _entityAnim = GetComponent<EntityAnim>();
    }
}
