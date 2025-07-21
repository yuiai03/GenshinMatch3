using Spine.Unity;
using System.Collections;
using UnityEngine;

public class EntityAnim : MonoBehaviour
{
    public AnimationReferenceAsset idle { get; set;}
    public AnimationReferenceAsset hurt {get; set;}
    public AnimationReferenceAsset die {get; set;}
    public AnimationReferenceAsset attack {get; set;}
    public SkeletonAnimation anim {get; set;}
    private Coroutine dieCoroutine;


    protected virtual void Awake()
    {
        if (anim == null)
            anim = GetComponentInChildren<SkeletonAnimation>();
    }
    private void OnEnable() { }
    public void Idle()
    {
        anim.timeScale = 1f;
        anim.state.SetAnimation(0, idle, true);
    }
    public void Die()
    {
        anim.state.SetAnimation(0, die, false);
        if (dieCoroutine != null) StopCoroutine(dieCoroutine);
        dieCoroutine = StartCoroutine(DieCoroutine());
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

    protected virtual IEnumerator DieCoroutine()
    {
        yield return null;
    }

}
