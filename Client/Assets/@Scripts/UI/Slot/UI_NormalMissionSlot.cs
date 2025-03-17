using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_NormalMissionSlot : UI_Slot
{
    private enum Texts
    {
        Text_NormalMissionListIcon,
        Text_MissonTitle,
        Text_NormalMissionCount,
    }

    private enum Buttons
    {
        Button_Take,
    }

    private enum Sliders
    {
        Slider_NormalMission,
    }

    private Mission _mission;

    protected override void Awake()
    {
        base.Awake();

        // Bind
        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        BindSliders(typeof(Sliders));

        // Bind Event
        GetButton((int)Buttons.Button_Take).gameObject.BindEvent(OnTakeButtonClick);
    }

    public void SetInfo(Mission mission)
    {
        _mission = mission;

        RefreshUI();
    }

    private void RefreshUI()
    {
        GetText((int)Texts.Text_NormalMissionListIcon).text = $"+{_mission.MissionData.Point:N0}";
        GetText((int)Texts.Text_MissonTitle).text = $"{_mission.MissionData.Remark}";
        GetText((int)Texts.Text_NormalMissionCount).text = $"{_mission.MissionInfo.StackedPoint:N0}/{_mission.MissionData.MaxPoint:N0}";

        GetButton((int)Buttons.Button_Take).interactable = _mission.MissionInfo.OwningState == EOwningState.Unowned && _mission.MissionInfo.StackedPoint >= _mission.MissionData.MaxPoint;

        GetSlider((int)Sliders.Slider_NormalMission).value = (float)_mission.MissionInfo.StackedPoint / _mission.MissionData.MaxPoint;
    }

    #region UI Event

    private void OnTakeButtonClick(PointerEventData data)
    {
        if (GetButton((int)Buttons.Button_Take).interactable)
        {
            Managers.Backend.GameData.Mission.CompleteNormalMission(_mission.MissionInfo.TemplateID);
        }
    }

    #endregion
}
