using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchedTileView : MonoBehaviour
{
    private Image _typeImage;
    private TextMeshProUGUI _quantityText;

    public void InitialData(TileType type, int quantity)
    {
        _typeImage = GetComponentInChildren<Image>();
        _quantityText = GetComponentInChildren<TextMeshProUGUI>();

        if (_typeImage) _typeImage.sprite = LoadManager.SpriteLoad($"Tile/{type}");
        if (_quantityText) _quantityText.text = quantity.ToString();
    }


}
