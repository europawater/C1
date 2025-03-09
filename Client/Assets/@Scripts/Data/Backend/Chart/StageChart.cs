using LitJson;
using System.Collections.Generic;

public class StageChart : BaseChart<StageData>
{
	public List<StageData> DataList { get; private set; } = new List<StageData>();

	public override string GetChartFileName()
	{
		return "StageChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach (JsonData data in jsonData)
		{
			StageData stageData = new StageData(data);
			DataList.Add(stageData);
		}
	}
}
