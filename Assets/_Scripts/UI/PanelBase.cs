using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PanelBase : MonoBehaviour
{
    [SerializeField] protected Image mainBg;

    protected virtual void Awake() { }
    public virtual void MainBgState(bool state)
    {
        if (mainBg) mainBg.gameObject.SetActive(state);
    }
}
