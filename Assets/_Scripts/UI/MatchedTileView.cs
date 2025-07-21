using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MatchedTileView : MonoBehaviour
{
    [SerializeField] private Image _typeImage;
    [SerializeField] private TextMeshProUGUI _quantityText;
    
    private Tween _scaleTween;

    public void InitialData(TileType type, int quantity)
    {
        var rect = gameObject.GetComponent<RectTransform>();
        
        _scaleTween?.Kill();
        
        rect.localScale = Vector3.zero;

        if (_typeImage) _typeImage.sprite = LoadManager.SpriteLoad($"Tile/{type}");
        if (_quantityText) _quantityText.text = quantity.ToString();
        
        _scaleTween = rect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
    
    private void OnDisable()
    {
        _scaleTween?.Kill();
    }
}
