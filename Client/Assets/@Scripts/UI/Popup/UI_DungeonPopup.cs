using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_DungeonPopup : UI_Popup
{
	private enum GameObjects
	{
		CloseArea,

		DungeonList,
	}

	private enum Buttons
	{ 
		Button_Close,
	}

	private List<UI_DungeonSlot> _dungeonSlotUIList = new List<UI_DungeonSlot>();

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindButtons(typeof(Buttons));

		// Init
		foreach (Transform child in GetGameObject((int)GameObjects.DungeonList).transform)
		{ 
			UI_DungeonSlot dungeonSlotUI = child.GetComponent<UI_DungeonSlot>();
			if (dungeonSlotUI != null)
			{ 
				_dungeonSlotUIList.Add(dungeonSlotUI);
			}
		}

		// Bind Event
		GetGameObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
		GetButton((int)Buttons.Button_Close).gameObject.BindEvent(OnClickCloseArea);
	}

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.OnCurrencyChanged, RefreshUI);
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.OnCurrencyChanged, RefreshUI);
    }

    public void SetInfo()
	{
		RefreshUI();
	}

	private void RefreshUI()
	{
		int dungeonSlotUIIndex = 0;
		foreach (var dungeon in Managers.Backend.GameData.Player.DungeonLevelDict)
		{
			_dungeonSlotUIList[dungeonSlotUIIndex].SetInfo(dungeon.Key, dungeon.Value);
			dungeonSlotUIIndex++;
		}
	}

	#region UI Event

	private void OnClickCloseArea(PointerEventData data)
	{
		ClosePopupUI();
	}

	#endregion
}
