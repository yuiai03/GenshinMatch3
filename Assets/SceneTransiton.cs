using DG.Tweening;
using UnityEngine;

public class SceneTransiton : MonoBehaviour
{
    [SerializeField] private CanvasGroup bgCanvas;
    [SerializeField] private CanvasGroup primogemCanvas;
    [SerializeField] private RectTransform right;
    [SerializeField] private RectTransform left;

    public void Open()
    {
        bgCanvas.alpha = 1;
        bgCanvas.gameObject.SetActive(true);
        right.gameObject.SetActive(true);
        left.gameObject.SetActive(true);
        Vector3 newRotate = new Vector3(0, 0, 180);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(bgCanvas.DOFade(0, 1).SetUpdate(true));
        sequence.Join(primogemCanvas.DOFade(0, 0.6f).SetUpdate(true));
        sequence.Join(primogemCanvas.transform.DOLocalRotate(newRotate, 1f));
        sequence.Join(right.DOAnchorPosX(1200, 1).SetUpdate(true));
        sequence.Join(left.DOAnchorPosX(-1200, 1).SetUpdate(true));
        sequence.OnComplete(() =>
        {
            right.gameObject.SetActive(false);
            left.gameObject.SetActive(false);
            bgCanvas.gameObject.SetActive(false);
        });
    }
    public void Close()
    {
        bgCanvas.alpha = 0;
        bgCanvas.gameObject.SetActive(true);
        right.gameObject.SetActive(true);
        left.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(bgCanvas.DOFade(1, 1).SetUpdate(true));
        sequence.Join(primogemCanvas.DOFade(1, 1f).SetUpdate(true));
        sequence.Join(primogemCanvas.transform.DOLocalRotate(Vector3.zero, 1f));
        sequence.Join(right.DOAnchorPosX(630, 1).SetUpdate(true));
        sequence.Join(left.DOAnchorPosX(-630, 1).SetUpdate(true));
    }
}
