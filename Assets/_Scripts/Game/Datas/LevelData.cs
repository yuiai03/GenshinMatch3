using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig", order = 0)]
public class LevelData : ScriptableObject
{
    public LevelConfig levelConfig;
}

[Serializable]
public class LevelConfig
{
    public int LevelNumber;
    public int turnsNumber;
    public EntityType playerType;
    public EntityType enemyType;
}
