using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class TitleScene : BaseScene
{
    protected override void Awake()
    {
        base.Awake();

        SceneType = EScene.Title;
        Managers.Scene.SetCurrentScene(this);

        // Resource Load
        Managers.Resource.LoadAllAsync<Object>("Preload", (key, count, totalcout) =>
        {
            if (count == totalcout)
            {
                Object eventSystem = FindAnyObjectByType(typeof(EventSystem));
                if (eventSystem == null)
                {
                    eventSystem = Managers.Resource.Instantiate("EventSystem");
                    eventSystem.name = "EventSystem";
                    DontDestroyOnLoad(eventSystem);
                }

                // Backend
                Managers.Backend.Init(() =>
                {
					// Ads
					Managers.Ads.Init(() =>
					{
						OnBackendInitSuccess();
					});
                });
            }
        });
    }

    private void OnBackendInitSuccess()
    {
        Managers.Sound.Init();

		// UI
		UI_TitleScene sceneUI = Managers.UI.ShowSceneUI<UI_TitleScene>();
        sceneUI.SetInfo();
    }

    public override void Clear()
    {
    }
}
