using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_DrawPopup : UI_Popup
{
	private enum GameObjects
	{
		Content_Reward,
	}

	private enum Buttons
	{
		Button_Config,
	}

	public enum DrawPopupState
	{
		Normal,
		Skill,
		Buddy,
	}

	private DrawPopupState _drawPopupState;
	private List<int> _rewardIdList = new List<int>();
	private List<UI_RewardSlot> _rewardSlotUIList = new List<UI_RewardSlot>();

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindButtons(typeof(Buttons));

		// Init
		_rewardSlotUIList.Clear();
		foreach(Transform child in GetGameObject((int)GameObjects.Content_Reward).transform)
		{
			UI_RewardSlot rewardSlotUI = child.GetComponent<UI_RewardSlot>();
			if (rewardSlotUI != null)
			{
				_rewardSlotUIList.Add(rewardSlotUI);
			}
		}

		// Bind Event
		GetButton((int)Buttons.Button_Config).gameObject.BindEvent(OnClickConfig);
	}

	public void SetInfo(DrawPopupState drawPopupState, List<int> rewardIdList)
	{
		_drawPopupState = drawPopupState;
		_rewardIdList = rewardIdList;

		RefreshUI();
	}

	private void RefreshUI()
	{
		int index = 0;
		foreach (int id in _rewardIdList)
		{
			_rewardSlotUIList[index].SetInfo(_drawPopupState, id);
			index++;
		}
	}

	#region UI Event

	private void OnClickConfig(PointerEventData data)
	{
		ClosePopupUI();
	}

	#endregion
}
