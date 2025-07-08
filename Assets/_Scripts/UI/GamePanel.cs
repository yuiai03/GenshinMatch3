using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    [SerializeField] public GameObject Menu;
    [SerializeField] public GameObject MatchedTilesViewHolder;

    [SerializeField] public HPBar PlayerHPBar;
    [SerializeField] public HPBar EnemyHPBar;

    [SerializeField] public TextMeshProUGUI TurnText;

    private void Awake()
    {
        Menu.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.OnMaxHPChanged +=  OnMaxHPChanged;
        EventManager.OnHPChanged += OnHPChanged;
        EventManager.OnTurnNumberChanged += SetTurnText;
    }

    private void OnDisable()
    {
        EventManager.OnMaxHPChanged -= OnMaxHPChanged;
        EventManager.OnHPChanged -= OnHPChanged;
        EventManager.OnTurnNumberChanged -= SetTurnText;
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

}
