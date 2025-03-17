using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class BuddyChart : BaseChart<BuddyData>
{
	public Dictionary<int, BuddyData> DataDict { get; private set; } = new Dictionary<int, BuddyData>();

	public override string GetChartFileName()
	{
		return "BuddyChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach (JsonData data in jsonData)
		{
			BuddyData buddyData = new BuddyData(data);
			DataDict.Add(buddyData.TemplateID, buddyData);
		}
	}
}
