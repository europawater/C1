using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelChart : BaseChart<PlayerLevelData>
{
	public Dictionary<int, PlayerLevelData> DataDict { get; private set; } = new Dictionary<int, PlayerLevelData>();

	public override string GetChartFileName()
	{
		return "PlayerLevelChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach (JsonData data in jsonData)
		{ 
			PlayerLevelData playerLevelData = new PlayerLevelData(data);
			DataDict.Add(playerLevelData.TemplateID, playerLevelData);
		}
	}
}
