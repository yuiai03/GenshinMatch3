using DG.Tweening;
using UnityEngine;

public class SceneTransiton : MonoBehaviour
{
    private float closePosX = 750f;
    private float openPosX = 1500f;
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

        right.anchoredPosition = new Vector2(closePosX, 0);
        left.anchoredPosition = new Vector2(-closePosX, 0);
        Vector3 newRotate = new Vector3(0, 0, 180);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(bgCanvas.DOFade(0, 1).SetUpdate(true));
        sequence.Join(primogemCanvas.DOFade(0, 0.6f).SetUpdate(true));
        sequence.Join(primogemCanvas.transform.DOLocalRotate(newRotate, 1f));
        sequence.Join(right.DOAnchorPosX(openPosX, 1).SetUpdate(true));
        sequence.Join(left.DOAnchorPosX(-openPosX, 1).SetUpdate(true));
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

        right.anchoredPosition = new Vector2(openPosX, 0);
        left.anchoredPosition = new Vector2(-openPosX, 0);
        primogemCanvas.transform.localRotation = Quaternion.Euler(0, 0, 180);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(bgCanvas.DOFade(1, 1).SetUpdate(true));
        sequence.Join(primogemCanvas.DOFade(1, 1f).SetUpdate(true));
        sequence.Join(primogemCanvas.transform.DOLocalRotate(Vector3.zero, 1f));
        sequence.Join(right.DOAnchorPosX(closePosX, 1).SetUpdate(true));
        sequence.Join(left.DOAnchorPosX(-closePosX, 1).SetUpdate(true));
    }
}
