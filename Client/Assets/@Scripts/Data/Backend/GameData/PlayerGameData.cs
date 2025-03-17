using BackEnd;
using LitJson;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerGameData : BaseGameData
{
	public int PlayerLevel { get; private set; }
	public int EXP { get; private set; }
	public bool IsMale { get; private set; }
	public int MonsterKillCount { get; private set; }
	public int StageLevel { get; private set; }
	public Dictionary<EDungeonType, int> DungeonLevelDict { get; private set; } = new Dictionary<EDungeonType, int>();

	public int NeedToLevelUpEXP
	{
		get
		{
			if (!Managers.Backend.Chart.PlayerLevelChart.DataDict.TryGetValue(PlayerLevel, out PlayerLevelData data))
			{
				return int.MaxValue;
			}

			return data.NeedToLevelUpEXP;
		}
	}

	public override string GetTableName()
	{
		return "Player";
	}

	public override Param GetParam()
	{
		Param param = new Param();

		param.Add("PlayerLevel", PlayerLevel);
		param.Add("EXP", EXP);
		param.Add("IsMale", IsMale);
		param.Add("MonsterKillCount", MonsterKillCount);
		param.Add("StageLevel", StageLevel);
		param.Add("CubeDungeonLevel", DungeonLevelDict[EDungeonType.Cube]);
		param.Add("GoldDungeonLevel", DungeonLevelDict[EDungeonType.Gold]);
		param.Add("DiamondDungeonLevel", DungeonLevelDict[EDungeonType.Diamond]);

		return param;
	}

	protected override void InitializeData()
	{
		PlayerLevel = 1;
		EXP = 0;
		IsMale = true;
		MonsterKillCount = 0;
		StageLevel = 1;

		DungeonLevelDict.Clear();
		DungeonLevelDict.Add(EDungeonType.Cube, 1);
		DungeonLevelDict.Add(EDungeonType.Gold, 1);
		DungeonLevelDict.Add(EDungeonType.Diamond, 1);
	}

	protected override void SetServerDataToLocal(JsonData gameDataJson)
	{
		PlayerLevel = int.Parse(gameDataJson["PlayerLevel"].ToString());
		EXP = int.Parse(gameDataJson["EXP"].ToString());
		IsMale = bool.Parse(gameDataJson["IsMale"].ToString());
		MonsterKillCount = int.Parse(gameDataJson["MonsterKillCount"].ToString());
		StageLevel = int.Parse(gameDataJson["StageLevel"].ToString());

		DungeonLevelDict.Clear();
		DungeonLevelDict.Add(EDungeonType.Cube, int.Parse(gameDataJson["CubeDungeonLevel"].ToString()));
		DungeonLevelDict.Add(EDungeonType.Gold, int.Parse(gameDataJson["GoldDungeonLevel"].ToString()));
		DungeonLevelDict.Add(EDungeonType.Diamond, int.Parse(gameDataJson["DiamondDungeonLevel"].ToString()));
	}

	protected override void UpdateData()
	{
		base.UpdateData();

		Managers.Event.TriggerEvent(EEventType.OnPlayerChanged);
	}

	#region Contents

	public void AddEXP(int amount)
	{
		EXP += amount;
		while (EXP >= NeedToLevelUpEXP)
		{
			EXP -= NeedToLevelUpEXP;
			LevelUp();
		}

		UpdateData();
	}

	private void LevelUp()
	{
		PlayerLevel++;

		// TODO: Create Popup..
		//
	}

	public void SetIsMale(bool isMale)
	{
		if(IsMale == isMale)
		{
			return;
		}

		IsMale = isMale;		
		
		UpdateData();
	}

	public void AddMonsterKillCount(int amount)
	{
		MonsterKillCount += amount;

		UpdateData();
	}

	public void AddStageLevel(int amount)
	{
		StageLevel += amount;
	
		UpdateData();
	}

	public void DungeonLevelUp(EDungeonType dungeonType)
	{
		DungeonLevelDict[dungeonType]++;

		UpdateData();
	}

	#endregion
}
