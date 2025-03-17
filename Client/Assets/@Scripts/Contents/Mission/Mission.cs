using UnityEngine;
using static Define;

public class Mission
{
	public MissionData MissionData { get; private set; }
	public MissionInfo MissionInfo { get; private set; }

	public Mission(MissionInfo missionInfo)
	{
		MissionData = Managers.Backend.Chart.MissionChart.MissionDataDict[missionInfo.TemplateID];
		MissionInfo = missionInfo;
	}
}
