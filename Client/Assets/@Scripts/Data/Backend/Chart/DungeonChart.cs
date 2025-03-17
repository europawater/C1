using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class DungeonChart : BaseChart<DungeonData>
{
	public Dictionary<int, DungeonData> CubeDungeonDataDict { get; private set; } = new Dictionary<int, DungeonData>();
	public Dictionary<int, DungeonData> GoldDungeonDataDict { get; private set; } = new Dictionary<int, DungeonData>();
	public Dictionary<int, DungeonData> DiamondDungeonDataDict { get; private set; } = new Dictionary<int, DungeonData>();

	public override string GetChartFileName()
	{
		return "DungeonChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach (JsonData data in jsonData)
		{
			DungeonData dungeonData = new DungeonData(data);
			switch (dungeonData.DungeonType)
			{
				case Define.EDungeonType.Cube:
					CubeDungeonDataDict.Add(dungeonData.DungeonLevel, dungeonData);
					break;
				case Define.EDungeonType.Gold:
					GoldDungeonDataDict.Add(dungeonData.DungeonLevel, dungeonData);
					break;
				case Define.EDungeonType.Diamond:
					DiamondDungeonDataDict.Add(dungeonData.DungeonLevel, dungeonData);
					break;

				default:
					break;
			}
		}
	}
}
