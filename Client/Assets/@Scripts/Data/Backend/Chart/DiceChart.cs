using LitJson;
using System.Collections.Generic;

public class DiceChart : BaseChart<DiceData>
{
	public Dictionary<int, DiceData> DataDict { get; private set; } = new Dictionary<int, DiceData>();

	public override string GetChartFileName()
	{
		return "DiceChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach (JsonData data in jsonData)
		{
			DiceData diceData = new DiceData(data);
			DataDict.Add(diceData.TemplateID, diceData);
		}
	}
}
