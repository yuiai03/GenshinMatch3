using DG.Tweening;
using UnityEngine;

public class PanelBase : MonoBehaviour
{
    [SerializeField] private CanvasGroup bg;
    [SerializeField] private CanvasGroup menu;

    protected virtual void Awake()
    {
        if (bg) bg.gameObject.SetActive(false);
        if (menu) menu.gameObject.SetActive(false);
    }

    public virtual void ShowPanel(float time = 0.5f)
    {
        menu.gameObject.SetActive(true);
        bg.gameObject.SetActive(true);

        bg.alpha = menu.alpha = 0;

        bg.DOFade(1, time).SetUpdate(true);
        menu.DOFade(1, time).SetUpdate(true);
    }
    public virtual void HidePanel(float time = 0.5f)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(bg.DOFade(0, time).SetUpdate(true));
        sequence.Join(menu.DOFade(0, time).SetUpdate(true));
        sequence.OnComplete(() =>
        {
            menu.gameObject.SetActive(false);
            bg.gameObject.SetActive(false);
        });
    }
}
