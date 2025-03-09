using LitJson;
using System.Collections.Generic;

public class MasteryChart : BaseChart<MasteryData>
{
	public Dictionary<int, MasteryData> DataDict { get; private set; } = new Dictionary<int, MasteryData>();

	public override string GetChartFileName()
	{
		return "MasteryChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach(JsonData data in jsonData)
		{
			MasteryData masteryData = new MasteryData(data);
			DataDict.Add(masteryData.TemplateID, masteryData);
		}
	}
}
