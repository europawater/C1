using LitJson;
using System.Collections.Generic;

public class MonsterChart : BaseChart<MonsterData>
{
	public Dictionary<int, MonsterData> DataDict { get; private set; } = new Dictionary<int, MonsterData>();

	public override string GetChartFileName()
	{
		return "MonsterChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach (JsonData data in jsonData)
		{
			MonsterData monsterData = new MonsterData(data);
			DataDict.Add(monsterData.TemplateID, monsterData);
		}
	}
}
