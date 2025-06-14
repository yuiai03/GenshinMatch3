using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : Singleton<LoadManager>
{
    public void ReLoadScene()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Sprites/path
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Sprite SpriteLoad(string path)
    {
        var spritePath = $"Sprites/{path}";
        Sprite sprite = Resources.Load<Sprite>(spritePath);
        if (sprite == null)
        {
            Debug.LogError($"Sprite not found at path: {spritePath}");
        }
        return sprite;
    }

    /// <summary>
    /// Prefabs/path
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T PrefabLoad<T>(string path) where T : Component
    {
        var prefabPath = $"Prefabs/{path}";
        T prefab = Resources.Load<T>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
        }
        return prefab;
    }

}
