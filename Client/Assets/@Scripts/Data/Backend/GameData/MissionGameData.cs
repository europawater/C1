using BackEnd;
using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MissionGameData : BaseGameData
{
    public DateTime LastDayMissionResetDate { get; private set; }
    public DateTime LastWeekMissionResetData { get; private set; }
    public Dictionary<int, MissionInfo> NormalMissionInfoDict { get; private set; } = new Dictionary<int, MissionInfo>();
    public Dictionary<int, MissionInfo> DayMissionInfoDict { get; private set; } = new Dictionary<int, MissionInfo>();
    public Dictionary<int, MissionInfo> WeekMissionInfoDict { get; private set; } = new Dictionary<int, MissionInfo>();

    public override string GetTableName()
    {
        return "Mission";
    }

    public override Param GetParam()
    {
        Param param = new Param();

        param.Add("LastDayMissionResetDate", LastDayMissionResetDate.ToString("yyyy-MM-dd HH:mm:ss"));
        param.Add("LastWeekMissionResetData", LastWeekMissionResetData.ToString("yyyy-MM-dd HH:mm:ss"));
        param.Add("NormalMissionInfoDict", NormalMissionInfoDict);
        param.Add("DayMissionInfoDict", DayMissionInfoDict);
        param.Add("WeekMissionInfoDict", WeekMissionInfoDict);

        return param;
    }

    protected override void InitializeData()
    {
        LastDayMissionResetDate = DateTime.Today.AddHours(9);
        LastWeekMissionResetData = GetMondayOfCurrentWeek().AddHours(9);

        NormalMissionInfoDict.Clear();
        foreach (MissionData missionData in Managers.Backend.Chart.MissionChart.NormalMissionDataDict.Values)
        {
            List<EOwningState> emptyPoinStepOwningStateList = new List<EOwningState>();
            foreach (int pointStep in missionData.PointStepList)
            {
                emptyPoinStepOwningStateList.Add(EOwningState.Unowned);
            }

            MissionInfo missionInfo = new MissionInfo(missionData.TemplateID, EOwningState.Unowned, 0, emptyPoinStepOwningStateList);
            NormalMissionInfoDict.Add(missionData.TemplateID, missionInfo);
        }

        DayMissionInfoDict.Clear();
        foreach (MissionData missionData in Managers.Backend.Chart.MissionChart.DayMissionDataDict.Values)
        {
            List<EOwningState> emptyPoinStepOwningStateList = new List<EOwningState>();
            foreach (int pointStep in missionData.PointStepList)
            {
                emptyPoinStepOwningStateList.Add(EOwningState.Unowned);
            }

            MissionInfo missionInfo = new MissionInfo(missionData.TemplateID, EOwningState.Unowned, 0, emptyPoinStepOwningStateList);
            DayMissionInfoDict.Add(missionData.TemplateID, missionInfo);
        }

        WeekMissionInfoDict.Clear();
        foreach (MissionData missionData in Managers.Backend.Chart.MissionChart.WeekMissionDataDict.Values)
        {
            List<EOwningState> emptyPoinStepOwningStateList = new List<EOwningState>();
            foreach (int pointStep in missionData.PointStepList)
            {
                emptyPoinStepOwningStateList.Add(EOwningState.Unowned);
            }

            MissionInfo missionInfo = new MissionInfo(missionData.TemplateID, EOwningState.Unowned, 0, emptyPoinStepOwningStateList);
            WeekMissionInfoDict.Add(missionData.TemplateID, missionInfo);
        }

        Managers.Game.MissionInit();
    }

    protected override void SetServerDataToLocal(JsonData gameDataJson)
    {
        LastDayMissionResetDate = DateTime.Parse(gameDataJson["LastDayMissionResetDate"].ToString());
        LastWeekMissionResetData = DateTime.Parse(gameDataJson["LastWeekMissionResetData"].ToString());

        NormalMissionInfoDict.Clear();
        foreach (JsonData key in gameDataJson["NormalMissionInfoDict"].Keys)
        {
            JsonData jsonData = gameDataJson["NormalMissionInfoDict"][key.ToString()];
            MissionInfo missionInfo = new MissionInfo(jsonData);
            NormalMissionInfoDict.Add(int.Parse(key.ToString()), missionInfo);
        }

        DayMissionInfoDict.Clear();
        foreach (JsonData key in gameDataJson["DayMissionInfoDict"].Keys)
        {
            JsonData jsonData = gameDataJson["DayMissionInfoDict"][key.ToString()];
            MissionInfo missionInfo = new MissionInfo(jsonData);
            DayMissionInfoDict.Add(int.Parse(key.ToString()), missionInfo);
        }

        WeekMissionInfoDict.Clear();
        foreach (JsonData key in gameDataJson["WeekMissionInfoDict"].Keys)
        {
            JsonData jsonData = gameDataJson["WeekMissionInfoDict"][key.ToString()];
            MissionInfo missionInfo = new MissionInfo(jsonData);
            WeekMissionInfoDict.Add(int.Parse(key.ToString()), missionInfo);
        }

        Managers.Game.MissionInit();
    }

    protected override void UpdateData()
    {
        Managers.Event.TriggerEvent(EEventType.OnMissionChanged);

        base.UpdateData();
    }

    #region Contents

    public void ResetDayMission()
    {
        LastDayMissionResetDate = DateTime.Today.AddHours(9);

        foreach (MissionInfo missionInfo in NormalMissionInfoDict.Values)
        {
			missionInfo.SetOwningState(EOwningState.Unowned);
            missionInfo.ResetStackPoint();
		}

		foreach (MissionInfo missionInfo in DayMissionInfoDict.Values)
		{
			missionInfo.SetOwningState(EOwningState.Unowned);
			missionInfo.ResetPointStepOwningStateList();
			missionInfo.ResetStackPoint();
		}

		UpdateData();
    }

    public void ResetWeekMission()
    {
        LastWeekMissionResetData = GetMondayOfCurrentWeek().AddHours(9);

		foreach (MissionInfo missionInfo in WeekMissionInfoDict.Values)
		{
			missionInfo.SetOwningState(EOwningState.Unowned);
			missionInfo.ResetPointStepOwningStateList();
			missionInfo.ResetStackPoint();
		}

		UpdateData();
    }

    private DateTime GetMondayOfCurrentWeek()
    {
        DateTime today = DateTime.Today;
        int daysToMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return today.AddDays(-daysToMonday);
    }

    public void AddNormalMissionPoint(EMissionGoal missionGoal, int point)
    {
        foreach (MissionInfo missionInfo in NormalMissionInfoDict.Values)
        {
            if (missionInfo.OwningState == EOwningState.Owned)
            {
                continue;
            }

            MissionData missionData = Managers.Backend.Chart.MissionChart.NormalMissionDataDict[missionInfo.TemplateID];
            if (missionData.MissionGoal != missionGoal)
            {
                continue;
            }

            if (missionInfo.StackedPoint + point >= missionData.MaxPoint)
            {
                missionInfo.AddStackedPoint(missionData.MaxPoint - missionInfo.StackedPoint);
            }
            else
            {
                missionInfo.AddStackedPoint(point);
            }
        }

        UpdateData();
    }

    public void CompleteNormalMission(int templateID)
    {
        if (!NormalMissionInfoDict.TryGetValue(templateID, out MissionInfo missionInfo))
        {
            return;
        }

        missionInfo.SetOwningState(EOwningState.Owned);

        foreach (MissionInfo dayMissionInfo in DayMissionInfoDict.Values)
        {
            dayMissionInfo.AddStackedPoint(missionInfo.Point);
        }

        foreach (MissionInfo weekMissionInfo in WeekMissionInfoDict.Values)
        {
            weekMissionInfo.AddStackedPoint(missionInfo.Point);
        }

        UpdateData();
    }

    public void CompleteDayMission(int templateID)
    {
        if (!DayMissionInfoDict.TryGetValue(templateID, out MissionInfo missionInfo))
        {
            return;
        }

        if(!Managers.Backend.Chart.MissionChart.DayMissionDataDict.TryGetValue(templateID, out MissionData missionData))
        {
            return;
        }

        List<Reward> rewardList = new List<Reward>();
        for (int index = 0; index < missionData.PointStepList.Count; index++)
        { 
            if(missionInfo.StackedPoint >= missionData.PointStepList[index] && missionInfo.PointStepOwningStateList[index] == EOwningState.Unowned)
            {
                missionInfo.PointStepOwningStateList[index] = EOwningState.Owned;
                rewardList.Add(new Reward(missionData.RewardTypeList[index], missionData.RewardValueList[index]));
            }
        }

        UI_RewardPopup rewardPopup = Managers.UI.ShowPopupUI<UI_RewardPopup>();
        rewardPopup.SetInfo(rewardList);

        UpdateData();
    }

    public void CompleteWeekMission(int templateID)
    {
        if (!WeekMissionInfoDict.TryGetValue(templateID, out MissionInfo missionInfo))
        {
            return;
        }

        if (!Managers.Backend.Chart.MissionChart.WeekMissionDataDict.TryGetValue(templateID, out MissionData missionData))
        {
            return;
        }

        List<Reward> rewardList = new List<Reward>();
        for (int index = 0; index < missionData.PointStepList.Count; index++)
        {
            if (missionInfo.StackedPoint >= missionData.PointStepList[index] && missionInfo.PointStepOwningStateList[index] == EOwningState.Unowned)
            {
                missionInfo.PointStepOwningStateList[index] = EOwningState.Owned;
                rewardList.Add(new Reward(missionData.RewardTypeList[index], missionData.RewardValueList[index]));
            }
        }

        UI_RewardPopup rewardPopup = Managers.UI.ShowPopupUI<UI_RewardPopup>();
        rewardPopup.SetInfo(rewardList);

        UpdateData();
    }

    #endregion
}
