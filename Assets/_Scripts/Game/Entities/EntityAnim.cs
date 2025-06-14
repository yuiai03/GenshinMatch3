using Spine.Unity;
using UnityEngine;

public class EntityAnim : MonoBehaviour
{
    [SerializeField] public AnimationReferenceAsset idle;
    [SerializeField] public AnimationReferenceAsset hurt;
    [SerializeField] public AnimationReferenceAsset die;
    [SerializeField] public AnimationReferenceAsset attack;

    public SkeletonAnimation anim { get; private set; }
    private Coroutine dieCoroutine;
    private Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        anim = GetComponentInChildren<SkeletonAnimation>();
    }
    private void OnEnable()
    {
        if (dieCoroutine != null) StopCoroutine(dieCoroutine);
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
    }
    public void Attack()
    {
        anim.state.SetAnimation(0, attack, false);
    }

}
