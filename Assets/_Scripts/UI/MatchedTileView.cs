using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchedTileView : MonoBehaviour
{
    [SerializeField] private Image _typeImage;
    [SerializeField] private TextMeshProUGUI _quantityText;

    public void InitialData(TileType type, int quantity)
    {
        if (_typeImage) _typeImage.sprite = LoadManager.SpriteLoad($"Tile/{type}");
        if (_quantityText) _quantityText.text = quantity.ToString();
    }


}
