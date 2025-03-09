using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using static Define;

public abstract class BaseScene : MonoBehaviour
{
    public EScene SceneType { get; protected set; } = EScene.Title;

    protected virtual void Awake()
    {
        // Default Setting
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
        GraphicsSettings.transparencySortAxis = new Vector3(0.0f, 1.0f, 0.0f);
    }

    protected virtual void Start()
    {
    }

    public abstract void Clear();
}
