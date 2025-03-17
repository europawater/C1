using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_WeekMissionSlot : UI_Slot
{
	private enum GameObjects
	{ 
		WeekMissionTakeArea,
	}

	private enum Texts
	{
		Text_WeekMissionCount1,
		Text_WeekMissionCount2,
		Text_WeekMissionCount3,
		Text_WeekMissionCount4,
		Text_WeekMissionCount5,
	}

	private enum Buttons
	{
		Button_AllTake,
	}

	private enum Sliders
	{
		Slider_WeekMissionExp,
	}

	private List<TMP_Text> _weekMissionCountTextList = new List<TMP_Text>();
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
		_weekMissionCountTextList.Clear();
		_weekMissionCountTextList.Add(GetText((int)Texts.Text_WeekMissionCount1));
		_weekMissionCountTextList.Add(GetText((int)Texts.Text_WeekMissionCount2));
		_weekMissionCountTextList.Add(GetText((int)Texts.Text_WeekMissionCount3));
		_weekMissionCountTextList.Add(GetText((int)Texts.Text_WeekMissionCount4));
		_weekMissionCountTextList.Add(GetText((int)Texts.Text_WeekMissionCount5));

		_missionRewardSlotUIList.Clear();
		foreach (Transform child in GetGameObject((int)GameObjects.WeekMissionTakeArea).transform)
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
			_weekMissionCountTextList[index].text = _mission.MissionData.PointStepList[index].ToString();
		}

		int rewardCount = 0;
		for(int index = 0; index< _missionRewardSlotUIList.Count; index++)
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

            if (isActive)
            {
                rewardCount++;
            }

            _missionRewardSlotUIList[index].SetInfo(_mission.MissionData.RewardTypeList[index], _mission.MissionData.RewardValueList[index], isActive);
		}

        GetButton((int)Buttons.Button_AllTake).interactable = _mission.MissionInfo.OwningState == EOwningState.Unowned && rewardCount > 0;

        GetSlider((int)Sliders.Slider_WeekMissionExp).value = (float)_mission.MissionInfo.StackedPoint / _mission.MissionData.MaxPoint;
	}

	#region UI Event

	private void OnAllTakeButtonClick(PointerEventData data)
	{
		if(GetButton((int)Buttons.Button_AllTake).interactable)
        {
            Managers.Backend.GameData.Mission.CompleteWeekMission(_mission.MissionInfo.TemplateID);
        }
    }

	#endregion
}
