using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementalReactionView : MonoBehaviour
{
    [SerializeField] private Image tileImage;

    public void SetTypeImage(TileType tileType)
    {
        var sprite = LoadManager.SpriteLoad($"Tile/{tileType.ToString()}");
        if(sprite) tileImage.sprite = sprite;
    }
}
