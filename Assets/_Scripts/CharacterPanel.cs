using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : PanelBase
{
    private int currentIndex = 0;
    private readonly float _currentPosX = 0f, _tempPosX = 1000f;
    [SerializeField] private EntityType[] _characterTypes;
    [SerializeField] private SkeletonGraphic _SkeletonGraphic_1, _SkeletonGraphic_2;
    [SerializeField] private Button _leftButton, _rightButton, _selectButton;
    [SerializeField] private TextMeshProUGUI _characterNameText, _buffText;
    protected override void Awake()
    {
        base.Awake();
        _leftButton.onClick.AddListener(LeftButtonClick);
        _rightButton.onClick.AddListener(RightButtonClick);
        _selectButton.onClick.AddListener(SelectedButtonClick);

        _SkeletonGraphic_1.rectTransform.anchoredPosition = new Vector2(_currentPosX, 0);
        _SkeletonGraphic_2.rectTransform.anchoredPosition = new Vector2(_tempPosX, 0);
    }
    private void Start()
    {
        Init();
    }
    private void Init()
    {
        if (_characterTypes.Length == 0) return;
        var characterData = LoadManager.DataLoad<EntityData>($"Entities/Player/{_characterTypes[0].ToString()}");
        if (!characterData) return;

        _SkeletonGraphic_1.skeletonDataAsset = characterData.entityConfig.skeletonDataAsset;
        _SkeletonGraphic_1.Initialize(true);
        _SkeletonGraphic_1.AnimationState.SetAnimation(0, characterData.entityConfig.idle, true);
        SetCharacterNameText();
        SetCurrentCharacterType();
        SetBuffText();
    }

    private void LeftButtonClick()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = _characterTypes.Length - 1;
        UpdateCharacterPanel(true);
    }

    private void RightButtonClick()
    {
        currentIndex++;
        if (currentIndex >= _characterTypes.Length) currentIndex = 0;
        UpdateCharacterPanel(false);
    }

    private void SelectedButtonClick()
    {
        SetCurrentCharacterType();
    }

    private void SetCharacterNameText() => _characterNameText.text = _characterTypes[currentIndex].ToString();
    private void SetBuffText()
    {
        if (_characterTypes[currentIndex] == EntityType.Buba)
        {
            _buffText.text = "Không có buff";
            return;
        }
        _buffText.text = $"Sát thương nguyên tố {Helper.BuffText(_characterTypes[currentIndex])} +1";
    }

    private void UpdateCharacterPanel(bool isLeft)
    {
        _leftButton.interactable = _rightButton.interactable = false;
        _SkeletonGraphic_2.rectTransform.anchoredPosition = new Vector2(isLeft ? -_tempPosX : _tempPosX, 0);

        var characterData = LoadManager.DataLoad<EntityData>($"Entities/Player/{_characterTypes[currentIndex].ToString()}");
        if (!characterData) return;

        _SkeletonGraphic_2.skeletonDataAsset = characterData.entityConfig.skeletonDataAsset;
        _SkeletonGraphic_2.Initialize(true);
        _SkeletonGraphic_2.AnimationState.SetAnimation(0, characterData.entityConfig.idle, true);


        Sequence sequence = DOTween.Sequence();
        sequence.Append(_SkeletonGraphic_2.rectTransform.DOAnchorPosX(_currentPosX, 0.5f).SetUpdate(true));
        sequence.Join(_SkeletonGraphic_1.rectTransform.DOAnchorPosX(isLeft ? _tempPosX : -_tempPosX, 0.5f).SetUpdate(true));
        sequence.OnComplete(() =>
        {
            var tempGraphic = _SkeletonGraphic_1;
            _SkeletonGraphic_1 = _SkeletonGraphic_2;
            _SkeletonGraphic_2 = tempGraphic;
            _leftButton.interactable = _rightButton.interactable = true;

            SetCharacterNameText();
            SetBuffText();
            SelectedButtonState();
        });
    }

    public void SetCurrentCharacterType()
    {
        GameManager.Instance.CurrentPlayerType = _characterTypes[currentIndex];
        SelectedButtonState();
    }
    public void SelectedButtonState()
    {
        var state = GameManager.Instance.CurrentPlayerType == _characterTypes[currentIndex];
        var text = _selectButton.GetComponentInChildren<TextMeshProUGUI>();
        text.text = state ? "Đã chọn" : "Chọn";
        _selectButton.interactable = state ? false : true;
    }
}
