using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PanelBase : MonoBehaviour
{
    [SerializeField] protected CanvasGroup bg;
    [SerializeField] protected CanvasGroup menu;
    private bool _canActive = false;

    protected virtual void Awake()
    {
        if (bg) bg.gameObject.SetActive(false);
        if (menu) menu.gameObject.SetActive(false);

        bg.GetComponent<Button>().onClick.AddListener(() => HidePanel());
    }

    public virtual void ShowPanel(float time = 0.5f)
    {
        if (_canActive) return;

        _canActive = true;
        menu.gameObject.SetActive(true);
        bg.gameObject.SetActive(true);
        bg.alpha = menu.alpha = 0;

        var sequence = DOTween.Sequence();
        sequence.Append(bg.DOFade(1, time).SetUpdate(true));
        sequence.Join(menu.DOFade(1, time).SetUpdate(true));
        sequence.OnComplete(() => _canActive = false);
    }
    public virtual void HidePanel(float time = 0.5f)
    {
        if (_canActive) return;
        _canActive = true;

        var sequence = DOTween.Sequence();
        sequence.Append(bg.DOFade(0, time).SetUpdate(true));
        sequence.Join(menu.DOFade(0, time).SetUpdate(true));
        sequence.OnComplete(() =>
        {
            _canActive = false;
            menu.gameObject.SetActive(false);
            bg.gameObject.SetActive(false);
        });
    }
}
