using LitJson;
using System.Collections.Generic;

public class HeroChart : BaseChart<HeroData>
{
	public Dictionary<int, HeroData> DataDict { get; private set; } = new Dictionary<int, HeroData>();
	
	public override string GetChartFileName()
	{
		return "HeroChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach (JsonData data in jsonData)
		{
			HeroData heroData = new HeroData(data);
			DataDict.Add(heroData.TemplateID, heroData);
		}
	}
}
