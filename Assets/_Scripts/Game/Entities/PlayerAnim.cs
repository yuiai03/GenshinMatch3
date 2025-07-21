using System.Collections;
using UnityEngine;

public class PlayerAnim : EntityAnim
{
    protected override IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        anim.timeScale = 0f;
    }
}
