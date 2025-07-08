using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private LevelType _levelType;
    private LevelData _levelData;
    private TextMeshPro _levelText;

    public void OpenLevelPanel()
    {
        if (!_levelData) return;
        EventManager.OpenLevelPanel(_levelData, _levelData.levelConfig.enemyType);
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
