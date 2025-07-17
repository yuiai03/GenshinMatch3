using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SinglePlayerPanel : Singleton<SinglePlayerPanel>
{
    [SerializeField] public GameObject Menu;
    [SerializeField] public GameObject MatchedTilesViewHolder;

    [SerializeField] public HPBar PlayerHPBar;
    [SerializeField] public HPBar EnemyHPBar;

    [SerializeField] public ElementalReactionView PlayerElementalReactionView;
    [SerializeField] public ElementalReactionView EnemyElementalReactionView;

    [SerializeField] public TextMeshProUGUI TurnText;
    [SerializeField] public Button ReturnButton;
    protected override void Awake()
    {
        base.Awake();
        ReturnButton.onClick.AddListener(ReturnClick);
    }

    private void OnEnable()
    {
        EventManager.OnMaxHPChanged +=  OnMaxHPChanged;
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

    private void OnMaxHPChanged(float hpValue, bool isPlayer)
    {
        if (isPlayer) PlayerHPBar.SetMaxHP(hpValue);
        else EnemyHPBar.SetMaxHP(hpValue);
    }
    private void OnHPChanged(float hpValue, bool isPlayer)
    {
        if (isPlayer) PlayerHPBar.SetHP(hpValue);
        else EnemyHPBar.SetHP(hpValue);
    }

    public void SetTurnText(int value)
    {
        TurnText.text = $"{value}";
    }
    private void ReturnClick()
    {
        LoadManager.Instance.TransitionLevel(SceneType.MainMenu);
    }
    private void OnCurrentTileTypeChange(TileType tileType, bool isPlayer)
    {
        SetElementalReactionViewStatus(isPlayer ? PlayerElementalReactionView : EnemyElementalReactionView, tileType);
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
