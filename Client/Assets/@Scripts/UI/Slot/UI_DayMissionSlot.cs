using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_DayMissionSlot : UI_Slot
{
	private enum GameObjects
	{
		DayMissionTakeArea,
	}

	private enum Texts
	{
		Text_DayMissionCount1,
		Text_DayMissionCount2,
		Text_DayMissionCount3,
		Text_DayMissionCount4,
		Text_DayMissionCount5,
	}

	private enum Buttons
	{ 
		Button_AllTake,
	}

	private enum Sliders
	{
		Slider_DayMissionExp,
	}

	private List<TMP_Text> _dayMissionCountTextList = new List<TMP_Text>();
	private List<UI_MissionRewardSlot> _missionRewardSlotUIList = new List<UI_MissionRewardSlot>();

	private Mission _mission;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));
		BindButtons(typeof(Buttons));
		BindSliders(typeof(Sliders));

		// Init
		_dayMissionCountTextList.Clear();
		_dayMissionCountTextList.Add(GetText((int)Texts.Text_DayMissionCount1));
		_dayMissionCountTextList.Add(GetText((int)Texts.Text_DayMissionCount2));
		_dayMissionCountTextList.Add(GetText((int)Texts.Text_DayMissionCount3));
		_dayMissionCountTextList.Add(GetText((int)Texts.Text_DayMissionCount4));
		_dayMissionCountTextList.Add(GetText((int)Texts.Text_DayMissionCount5));

		_missionRewardSlotUIList.Clear();
		foreach (Transform child in GetGameObject((int)GameObjects.DayMissionTakeArea).transform)
		{
			UI_MissionRewardSlot slotUI = child.GetComponent<UI_MissionRewardSlot>();
			_missionRewardSlotUIList.Add(slotUI);
		}

		// Bind Event
		GetButton((int)Buttons.Button_AllTake).gameObject.BindEvent(OnAllTakeButtonClick);
	}

	public void SetInfo(Mission mission)
	{
		_mission = mission;

		RefreshUI();
	}

	private void RefreshUI()
	{
		for(int index = 0; index < _mission.MissionData.PointStepList.Count; index++)
		{
			_dayMissionCountTextList[index].text = $"{_mission.MissionData.PointStepList[index]:N0}";
		}

		int rewardCount = 0;
		for (int index = 0; index < _missionRewardSlotUIList.Count; index++)
		{
			bool isActive = true;
			if (_mission.MissionInfo.StackedPoint < _mission.MissionData.PointStepList[index])
			{
				isActive = false;
			}

			if (_mission.MissionInfo.PointStepOwningStateList[index] == EOwningState.Owned)
			{ 
				isActive = false;
            }

			if(isActive)
            {
                rewardCount++;
            }

            _missionRewardSlotUIList[index].SetInfo(_mission.MissionData.RewardTypeList[index], _mission.MissionData.RewardValueList[index], isActive);
		}

		GetButton((int)Buttons.Button_AllTake).interactable = _mission.MissionInfo.OwningState == EOwningState.Unowned && rewardCount > 0;

        GetSlider((int)Sliders.Slider_DayMissionExp).value = (float)_mission.MissionInfo.StackedPoint / _mission.MissionData.MaxPoint;
	}

	#region UI Event

	private void OnAllTakeButtonClick(PointerEventData data)
	{
        if (GetButton((int)Buttons.Button_AllTake).interactable)
        {
            Managers.Backend.GameData.Mission.CompleteDayMission(_mission.MissionInfo.TemplateID);
        }
    }

	#endregion
}
