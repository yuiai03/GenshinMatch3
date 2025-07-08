using Spine.Unity;
using UnityEngine;

public class EntityAnim : MonoBehaviour
{
    [SerializeField] public AnimationReferenceAsset idle;
    [SerializeField] public AnimationReferenceAsset hurt;
    [SerializeField] public AnimationReferenceAsset die;
    [SerializeField] public AnimationReferenceAsset attack;

    public SkeletonAnimation anim;
    private Coroutine dieCoroutine;
    private Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    private void OnEnable()
    {
    }
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
