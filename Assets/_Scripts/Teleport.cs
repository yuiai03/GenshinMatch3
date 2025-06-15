using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private LevelType _levelType;
    [SerializeField] private EntityType _entityType;
    private LevelData _levelData;
    private TextMeshPro _levelText;

    public void OpenLevelPanel()
    {
        GameManager.Instance.CurrentLevelData = _levelData;
        UIManager.Instance.MapPanel.ShowPanel();
        UIManager.Instance.MapPanel.SetInfo(
            _levelData.levelConfig.turnsNumber, _entityType);
    }

    public void InitializeData()
    {
        _levelData = LoadManager.DataLoad<LevelData>($"Levels/{_levelType}");

        if (!_levelData) return;
        if(!_levelText) _levelText = GetComponentInChildren<TextMeshPro>();

        SetLevelText(_levelData.levelConfig.levelName);
    }

    private void SetLevelText(string level) => _levelText.text = level;
}
