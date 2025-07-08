using Spine.Unity;
using UnityEngine;

public class EntityAnim : MonoBehaviour
{
    public AnimationReferenceAsset idle { get; set;}
    public AnimationReferenceAsset hurt {get; set;}
    public AnimationReferenceAsset die {get; set;}
    public AnimationReferenceAsset attack {get; set;}
    public SkeletonAnimation anim {get; set;}


    protected virtual void Awake()
    {
        if (anim == null)
            anim = GetComponentInChildren<SkeletonAnimation>();
    }
    private void OnEnable() { }
    public void Idle()
    {
        anim.state.SetAnimation(0, idle, true);
    }
    public void Die()
    {
        anim.state.SetAnimation(0, die, false);
    }
    public void Hurt()
    {
        anim.state.SetAnimation(0, hurt, false);
        AddAnim(idle, true);
    }
    public void Attack()
    {
        anim.state.SetAnimation(0, attack, false);
        AddAnim(idle, true);
    }
    public void AddAnim(AnimationReferenceAsset anim, bool loop)
    {
        this.anim.state.AddAnimation(0, anim, loop, 0f);
    }

}
