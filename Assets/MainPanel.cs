using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : PanelBase
{
    [SerializeField] private readonly float _unSelectPosY = 70, _selectPosY = 120;
    [SerializeField] private readonly float _unSelectScale = 1, _selectScale = 1.2f;
    [SerializeField] private MainButton _mapButton, _characterButton, _pvpButton;

    protected override void Awake()
    {
        base.Awake();        _mapButton._button.onClick.AddListener(OnMapButtonClicked);
        _characterButton._button.onClick.AddListener(OnCharacterButtonClicked);
        _pvpButton._button.onClick.AddListener(OnPvPButtonClicked);
    }
    private void Start()
    {
        OnClickButton(_pvpButton);
    }
    private void OnMapButtonClicked()
    {
        OnClickButton(_mapButton);
    }
    private void OnCharacterButtonClicked()
    {
        OnClickButton(_characterButton);
    }
    private void OnPvPButtonClicked()
    {
        OnClickButton(_pvpButton);
    }

    private void OnClickButton(MainButton mainButton)
    {
        if(mainButton.IsSelected) return;

        var listButton = new List<MainButton> { _mapButton, _characterButton, _pvpButton };
        foreach (var btn in listButton)
        {
            btn.IsSelected = btn == mainButton;
            btn.SetBgImageState(btn == mainButton);
            btn.GetComponent<RectTransform>().DOAnchorPosY(btn == mainButton ? _selectPosY : _unSelectPosY, 0.3f);
            btn.GetComponent<RectTransform>().DOScale(btn == mainButton ? _selectScale : _unSelectScale, 0.3f);
        }
        UIManager.Instance.ShowPanel(mainButton._mainButtonType);
    }
}
