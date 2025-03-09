using BackEnd;
using LitJson;
using UnityEngine;
using static Define;

public class DiceGameData : BaseGameData
{
	public int DiceLevel { get; private set; }
	public int DiceCount { get; private set; }

	public override string GetTableName()
	{
		return "Dice";
	}

	public override Param GetParam()
	{
		Param param = new Param();

		param.Add("DiceLevel", DiceLevel);
		param.Add("DiceCount", DiceCount);

		return param;
	}

	protected override void InitializeData()
	{
		DiceLevel = 1;
		DiceCount = 10;
	}

	protected override void SetServerDataToLocal(JsonData gameDataJson)
	{
		DiceLevel = int.Parse(gameDataJson["DiceLevel"].ToString());
		DiceCount = int.Parse(gameDataJson["DiceCount"].ToString());
	}


	protected override void UpdateData()
	{
		base.UpdateData();
		Managers.Event.TriggerEvent(EEventType.OnDiceChanged);
	}

	#region Contents
	
	public void AddDiceCount(int count)
	{
		DiceCount += count;
		
		UpdateData();
	}

	public void SubDiceCount(int count)
	{
		DiceCount -= count;
	
		UpdateData();
	}

	public void LevelUp()
	{ 
		DiceLevel++;
	}

	#endregion
}
