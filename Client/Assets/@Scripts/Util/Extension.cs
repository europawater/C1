using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public static class Extension
{
    public static void BindEvent(this GameObject gameObject, Action<PointerEventData> action = null, EUIEvent type = EUIEvent.Click)
    {
        UI_Base.BindEvent(gameObject, action, type);
    }

    public static void DestroyChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }
    }

    public static void SetActiveChildren(this Transform transform, bool active)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(active);
        }
    }
}
