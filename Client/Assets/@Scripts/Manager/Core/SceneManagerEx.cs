using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class SceneManagerEx
{
    public BaseScene CurrentScene { get; private set; }

    public void SetCurrentScene(BaseScene scene)
    {
        CurrentScene = scene;
    }

    public void LoadScene(EScene type, bool isAsync = true, Action onCompleted = null)
    {
        Managers.Clear();

        if (isAsync)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GetSceneName(type));
            asyncOperation.completed += (AsyncOperation obj) =>
            {
                onCompleted?.Invoke();
            };
        }
        else
        {
            SceneManager.LoadScene(GetSceneName(type));
            onCompleted?.Invoke();
        }
    }

    private string GetSceneName(EScene type)
    {
        string name = Enum.GetName(typeof(EScene), type);
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
