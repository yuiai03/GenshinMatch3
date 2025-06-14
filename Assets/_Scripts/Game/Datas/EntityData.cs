using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityConfig", menuName = "ScriptableObjects/EntityConfig", order = 0)]
public class EntityData : ScriptableObject
{
    public EntityConfig entityConfig;
}

[System.Serializable]
public class EntityConfig
{
    public EntityType entityType;
    public int HP;

    public AnimationReferenceAsset idle;
    public AnimationReferenceAsset hurt;
    public AnimationReferenceAsset die;
    public AnimationReferenceAsset attack;
    public SkeletonDataAsset skeletonDataAsset;
}
