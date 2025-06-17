using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/TileData", order = 1)]
public class TileData : ScriptableObject
{
    public TileConfig[] tileConfigs;

    public TileConfig GetTileConfig(TileType tileType)
    {
        foreach (var config in tileConfigs)
        {
            if (config.tileType == tileType)
            {
                return config;
            }
        }
        return null;
    }
}

[System.Serializable]
public class TileConfig
{
    public TileType tileType;
    public Color color;
    public Gradient gradient;
}
