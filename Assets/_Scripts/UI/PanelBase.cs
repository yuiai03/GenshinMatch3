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

    public virtual void ShowPanel()
    {
        menu.gameObject.SetActive(true);
        bg.gameObject.SetActive(true);

        bg.alpha = menu.alpha = 0;

        bg.DOFade(1, 0.5f).SetUpdate(true);
        menu.DOFade(1, 0.5f).SetUpdate(true);
    }
    public virtual void HidePanel()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(bg.DOFade(0, 0.5f).SetUpdate(true));
        sequence.Join(menu.DOFade(0, 0.5f).SetUpdate(true));
        sequence.OnComplete(() =>
        {
            menu.gameObject.SetActive(false);
            bg.gameObject.SetActive(false);
        });
    }
}
