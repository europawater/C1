using LitJson;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MissionChart : BaseChart<MissionData>
{
	public Dictionary<int, MissionData> MissionDataDict { get; private set; } = new Dictionary<int, MissionData>();
	public Dictionary<int, MissionData> NormalMissionDataDict { get; private set; } = new Dictionary<int, MissionData>();
	public Dictionary<int, MissionData> DayMissionDataDict { get; private set; } = new Dictionary<int, MissionData>();
	public Dictionary<int, MissionData> WeekMissionDataDict { get; private set; } = new Dictionary<int, MissionData>();

	public override string GetChartFileName()
	{
		return "MissionChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach (JsonData data in jsonData)
		{
			MissionData missionData = new MissionData(data);
			MissionDataDict.Add(missionData.TemplateID, missionData);
			switch (missionData.MissionType)
			{
				case EMissionType.Normal:
					NormalMissionDataDict.Add(missionData.TemplateID, missionData);
					break;
				case EMissionType.Day:
					DayMissionDataDict.Add(missionData.TemplateID, missionData);
					break;
				case EMissionType.Week:
					WeekMissionDataDict.Add(missionData.TemplateID, missionData);
					break;

				default:
					break;
			}
		}
	}
}
