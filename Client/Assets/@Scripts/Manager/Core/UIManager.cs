using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UIManager
{
    private int _order = 10;
    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

    private UI_Scene _sceneUI = null;
    public UI_Scene SceneUI
    {
        set { _sceneUI = value; }
        get { return _sceneUI; }
    }

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
            { 
                root = new GameObject { name = "@UI_Root" };
            }
            return root;
        }
    }

    public void SetCanvas(GameObject gameObject, bool sort = true, int sortOrder = 0)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(gameObject);
        if (canvas == null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
        }

        CanvasScaler cs = gameObject.GetOrAddComponent<CanvasScaler>();
        if (cs != null)
        {
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1440, 2560);
        }

        gameObject.GetOrAddComponent<GraphicRaycaster>();

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = sortOrder;
        }
    }

    public T GetSceneUI<T>() where T : UI_Base
    {
        return _sceneUI as T;
    }

    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
        { 
            name = typeof(T).Name;
        }

        GameObject gameObject = Managers.Resource.Instantiate($"{name}");
        if (parent != null)
        { 
            gameObject.transform.SetParent(parent);
        }

        Canvas canvas = gameObject.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(gameObject);
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
        { 
            name = typeof(T).Name;
        }

        GameObject gameObject = Managers.Resource.Instantiate(name, parent);
        gameObject.transform.SetParent(parent);

        return Util.GetOrAddComponent<T>(gameObject);
    }

    public T ShowBaseUI<T>(string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
        { 
            name = typeof(T).Name;
        }

        GameObject gameObject = Managers.Resource.Instantiate(name);
        T baseUI = Util.GetOrAddComponent<T>(gameObject);

        gameObject.transform.SetParent(Root.transform);

        return baseUI;
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
        { 
            name = typeof(T).Name;
        }

        GameObject gameObject = Managers.Resource.Instantiate(name);
        T sceneUI = Util.GetOrAddComponent<T>(gameObject);
        _sceneUI = sceneUI;

        gameObject.transform.SetParent(Root.transform);

        return sceneUI;
    }

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
        { 
            name = typeof(T).Name;
        }

        GameObject gameObject = Managers.Resource.Instantiate(name);
        T popup = Util.GetOrAddComponent<T>(gameObject);
        _popupStack.Push(popup);

        gameObject.transform.SetParent(Root.transform);

        Managers.Sound.Play(ESound.Effect, "SFX_popup");

        return popup;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
        { 
            return;
        }

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
        { 
            return;
        }

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
        { 
            ClosePopupUI();
        }
    }

    public int GetPopupCount()
    {
        return _popupStack.Count;
    }

    public void Clear()
    {
        CloseAllPopupUI();
        _sceneUI = null;
    }
}
