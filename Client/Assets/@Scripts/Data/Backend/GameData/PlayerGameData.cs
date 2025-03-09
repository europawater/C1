using BackEnd;
using LitJson;
using UnityEngine;
using static Define;

public class PlayerGameData : BaseGameData
{
	public int PlayerLevel { get; private set; }
	public int EXP { get; private set; }
	public int MonsterKillCount { get; private set; }

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
		param.Add("MonsterKillCount", MonsterKillCount);
		return param;
	}

	protected override void InitializeData()
	{
		PlayerLevel = 1;
		EXP = 0;
		MonsterKillCount = 0;
	}

	protected override void SetServerDataToLocal(JsonData gameDataJson)
	{
		PlayerLevel = int.Parse(gameDataJson["PlayerLevel"].ToString());
		EXP = int.Parse(gameDataJson["EXP"].ToString());
		MonsterKillCount = int.Parse(gameDataJson["MonsterKillCount"].ToString());
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

	public void AddMonsterKillCount(int amount)
	{
		MonsterKillCount += amount;

		UpdateData();
	}

	#endregion
}
