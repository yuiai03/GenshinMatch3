using DG.Tweening;
using UnityEngine;

public class SceneTransiton : MonoBehaviour
{
    private const float _duration = 1f;
    private float _closePosX = 750f;
    private float _openPosX = 1500f;
    private Ease _easeType = Ease.OutSine;
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

        right.anchoredPosition = new Vector2(_closePosX, 0);
        left.anchoredPosition = new Vector2(-_closePosX, 0);
        Vector3 newRotate = new Vector3(0, 0, 180);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(bgCanvas.DOFade(0, _duration).SetUpdate(true).SetEase(_easeType));
        sequence.Join(primogemCanvas.DOFade(0, _duration/2).SetUpdate(true).SetEase(_easeType));
        sequence.Join(primogemCanvas.transform.DOLocalRotate(newRotate, _duration).SetEase(_easeType));
        sequence.Join(right.DOAnchorPosX(_openPosX, _duration).SetUpdate(true).SetEase(_easeType));
        sequence.Join(left.DOAnchorPosX(-_openPosX, _duration).SetUpdate(true).SetEase(_easeType));
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

        right.anchoredPosition = new Vector2(_openPosX, 0);
        left.anchoredPosition = new Vector2(-_openPosX, 0);
        primogemCanvas.transform.localRotation = Quaternion.Euler(0, 0, 180);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(bgCanvas.DOFade(1, _duration).SetUpdate(true).SetEase(_easeType));
        sequence.Join(primogemCanvas.DOFade(1, _duration/2).SetUpdate(true).SetEase(_easeType));
        sequence.Join(primogemCanvas.transform.DOLocalRotate(Vector3.zero, _duration).SetEase(_easeType));
        sequence.Join(right.DOAnchorPosX(_closePosX, _duration).SetUpdate(true).SetEase(_easeType));
        sequence.Join(left.DOAnchorPosX(-_closePosX, _duration).SetUpdate(true).SetEase(_easeType));
    }
}
