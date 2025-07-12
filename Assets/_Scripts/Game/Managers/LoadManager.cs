using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : Singleton<LoadManager>
{
    private Coroutine transitionCoroutine;

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

    public static T DataLoad<T>(string path) where T : Object
    {
        var dataPath = $"SODatas/{path}";
        T data = Resources.Load<T>(dataPath);
        if (data == null)
        {
            Debug.LogError($"Data not found at path: {dataPath}");
        }
        return data;
    }


    public void TransitionLevel(SceneType sceneType)
    {
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(TransitionCoroutine(sceneType));
    }

    private IEnumerator TransitionCoroutine(SceneType sceneType)
    {
        var levelTransiton = UIManager.Instance.SceneTransiton;
        levelTransiton.Close();

        yield return new WaitForSeconds(1f);

        DOTween.KillAll();

        var loadScene = SceneManager.LoadSceneAsync(sceneType.ToString());
        loadScene.allowSceneActivation = false;
        while (!loadScene.isDone)
        {
            if (loadScene.progress >= 0.9f)
            {
                loadScene.allowSceneActivation = true;
                EventManager.SceneChanged(sceneType);
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);

        levelTransiton.Open();
        EventManager.SceneChanged(sceneType);
    }
}
