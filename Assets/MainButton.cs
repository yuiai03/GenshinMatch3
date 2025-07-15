using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainButton : MonoBehaviour
{
    public bool IsSelected { get; set; }
    [SerializeField] public MainButtonType _mainButtonType;
    [SerializeField] public Button _button;
    [SerializeField] public Image _bgImage;

    public void SetBgImageState(bool isSelect)
    {
        _bgImage.sprite = isSelect ? 
            LoadManager.SpriteLoad($"Button/GUI_7") : 
            LoadManager.SpriteLoad($"Button/GUI_8");
    }
}
