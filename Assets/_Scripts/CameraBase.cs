using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBase : MonoBehaviour
{
    protected virtual void Awake()
    {
        if (UIManager.Instance)
        {
            Canvas canvas = UIManager.Instance.GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
        }
    }
}
