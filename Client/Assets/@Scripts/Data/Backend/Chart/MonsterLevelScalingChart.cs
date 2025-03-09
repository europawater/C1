using LitJson;
using System.Collections.Generic;

public class MonsterLevelScalingChart : BaseChart<MonsterLevelScalingData>
{
	public Dictionary<int, MonsterLevelScalingData> DataDict { get; private set; } = new Dictionary<int, MonsterLevelScalingData>();
	
	public override string GetChartFileName()
	{
		return "MonsterLevelScalingChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach (JsonData data in jsonData)
		{
			MonsterLevelScalingData monsterLevelScalingData = new MonsterLevelScalingData(data);
			DataDict.Add(monsterLevelScalingData.TemplateID, monsterLevelScalingData);
		}
	}
}
