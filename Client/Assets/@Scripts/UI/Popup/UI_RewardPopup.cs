using System;
using UnityEngine;

public class UI_RewardPopup : UI_Popup
{
	private enum GameObjects
	{ 
		Content_Reward,
	}

	private enum Buttons
	{
		Button_Config,
	}

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindButtons(typeof(Buttons));

		// Bind Event
		GetButton((int)Buttons.Button_Config).onClick.AddListener(OnClickConfig);
	}

	public void SetInfo()
	{
		RefreshUI();
	}

	private void RefreshUI()
	{ 
	
	}

	#region UI Event

	private void OnClickConfig()
	{
		ClosePopupUI();
	}

	#endregion
}
