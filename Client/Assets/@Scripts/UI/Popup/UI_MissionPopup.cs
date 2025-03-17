using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_MissionPopup : UI_Popup
{
    private enum GameObjects
    { 
        CloseArea,

		NormalMissonListArea,
		UI_WeekMissionSlot,
		UI_DayMissionSlot,
	}

    private enum Buttons
    {
		Button_Close,
	}

	private List<UI_NormalMissionSlot> _normalMissionSlotUIList = new List<UI_NormalMissionSlot>();
	private UI_DayMissionSlot _dayMissionSlotUI;
	private UI_WeekMissionSlot _weekMissionSlotUI;

	protected override void Awake()
	{
		base.Awake();

        // Bind 
        BindGameObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));

		// Init
		_normalMissionSlotUIList.Clear();
		GetGameObject((int)GameObjects.NormalMissonListArea).transform.DestroyChildren();
		for (int index = 0; index < Managers.Backend.Chart.MissionChart.NormalMissionDataDict.Count; index++)
		{ 
			UI_NormalMissionSlot slotUI = Managers.UI.MakeSubItem<UI_NormalMissionSlot>(GetGameObject((int)GameObjects.NormalMissonListArea).transform);
			_normalMissionSlotUIList.Add(slotUI);
		}

		_dayMissionSlotUI = GetGameObject((int)GameObjects.UI_DayMissionSlot).GetComponent<UI_DayMissionSlot>();
		_weekMissionSlotUI = GetGameObject((int)GameObjects.UI_WeekMissionSlot).GetComponent<UI_WeekMissionSlot>();

		// Bind Event
		GetGameObject((int)GameObjects.CloseArea).BindEvent(OnCloseAreaClick);
		GetButton((int)Buttons.Button_Close).gameObject.BindEvent(OnCloseAreaClick);
	}

    private void OnEnable()
    {
		Managers.Event.AddEvent(EEventType.OnMissionChanged, RefreshUI);
    }

    private void OnDisable()
    {
		Managers.Event.RemoveEvent(EEventType.OnMissionChanged, RefreshUI);
    }

    public void SetInfo()
    {
		RefreshUI();
	}

	private void RefreshUI()
	{
		int index = 0;
		foreach (Mission normallMission in Managers.Game.NormalMissionList)
		{
			_normalMissionSlotUIList[index].SetInfo(normallMission);
			index++;
		}

		_dayMissionSlotUI.SetInfo(Managers.Game.DayMissionList.First());
		_weekMissionSlotUI.SetInfo(Managers.Game.WeekMissionList.First());
	}

	#region UI Event

	private void OnCloseAreaClick(PointerEventData data)
	{
		ClosePopupUI();
	}

	#endregion
}
