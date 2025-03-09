using LitJson;
using System.Collections.Generic;

public class RewardChart : BaseChart<RewardData>
{
	public Dictionary<int, RewardData> DataDict { get; private set; } = new Dictionary<int, RewardData>();

	public override string GetChartFileName()
	{
		return "RewardChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach (JsonData data in jsonData)
		{
			RewardData rewardData = new RewardData(data);
			DataDict.Add(rewardData.TemplateID, rewardData);
		}
	}
}
