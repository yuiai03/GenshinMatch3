using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerPanel : Singleton<MultiplayerPanel>
{
    [SerializeField] public GameObject Menu;
    [SerializeField] public GameObject MatchedTilesViewHolder;

    [SerializeField] public HPBar Player1HPBar;
    [SerializeField] public HPBar Player2HPBar;

    [SerializeField] public ElementalReactionView Player1ElementalReactionView;
    [SerializeField] public ElementalReactionView Player2ElementalReactionView;

    [SerializeField] public TextMeshProUGUI TurnText, PlayerTurnText;
    [SerializeField] public Button ReturnButton;
    protected override void Awake()
    {
        base.Awake();
        ReturnButton.onClick.AddListener(ReturnClick);
    }

    private void OnEnable()
    {
        EventManager.OnMaxHPChanged += OnMaxHPChanged;
        EventManager.OnHPChanged += OnHPChanged;
        EventManager.OnTurnNumberChanged += SetTurnText;
        EventManager.OnCurrentTileTypeChanged += OnCurrentTileTypeChange;
    }

    private void OnDisable()
    {
        EventManager.OnMaxHPChanged -= OnMaxHPChanged;
        EventManager.OnHPChanged -= OnHPChanged;
        EventManager.OnTurnNumberChanged -= SetTurnText;
        EventManager.OnCurrentTileTypeChanged -= OnCurrentTileTypeChange;
    }

    private void OnMaxHPChanged(float hpValue, bool isPlayer1)
    {
        if (isPlayer1) Player1HPBar.SetMaxHP(hpValue);
        else Player2HPBar.SetMaxHP(hpValue);
    }
    private void OnHPChanged(float hpValue, bool isPlayer1)
    {
        if (isPlayer1) Player1HPBar.SetHP(hpValue);
        else Player2HPBar.SetHP(hpValue);
    }

    public void SetTurnText(int value)
    {
        TurnText.text = $"{value}";
    }
    public void SetTurnNameText(string name)
    {
        PlayerTurnText.text = $"{name}";
    }
    private void ReturnClick()
    {
        LoadManager.Instance.TransitionLevel(SceneType.MainMenu);
    }
    private void OnCurrentTileTypeChange(TileType tileType, bool isPlayer1)
    {
        SetElementalReactionViewStatus(isPlayer1 ? Player1ElementalReactionView : Player2ElementalReactionView, tileType);
    }
    private void SetElementalReactionViewStatus(ElementalReactionView elementalReactionView, TileType tileType)
    {
        if (tileType == TileType.None)
        {
            elementalReactionView.gameObject.SetActive(false);
            return;
        }
        elementalReactionView.gameObject.SetActive(true);
        elementalReactionView.SetTypeImage(tileType);
    }
}
