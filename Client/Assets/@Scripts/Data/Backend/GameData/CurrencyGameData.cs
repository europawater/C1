using BackEnd;
using LitJson;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CurrencyGameData : BaseGameData
{
	public Dictionary<ECurrency, int> CurrencyDict { get; private set; } = new Dictionary<ECurrency, int>();

	public override string GetTableName()
	{
		return "Currency";
	}

	public override Param GetParam()
	{
		Param param = new Param();

		param.Add("Gold", CurrencyDict[ECurrency.Gold]);
		param.Add("Diamond", CurrencyDict[ECurrency.Diamond]);
		param.Add("DungeonTicket", CurrencyDict[ECurrency.DungeonTicket]);

		return param;
	}

	protected override void InitializeData()
	{
		CurrencyDict.Clear();
		CurrencyDict.Add(ECurrency.Gold, 10000);
		CurrencyDict.Add(ECurrency.Diamond, 0);
		CurrencyDict.Add(ECurrency.DungeonTicket, 6);
	}

	protected override void SetServerDataToLocal(JsonData gameDataJson)
	{
		int goldAmount = int.Parse(gameDataJson["Gold"].ToString());
		int diamondAmount = int.Parse(gameDataJson["Diamond"].ToString());
		int dungeonTicket = int.Parse(gameDataJson["DungeonTicket"].ToString());

		CurrencyDict.Clear();
		CurrencyDict.Add(ECurrency.Gold, goldAmount);
		CurrencyDict.Add(ECurrency.Diamond, diamondAmount);
		CurrencyDict.Add(ECurrency.DungeonTicket, dungeonTicket);
	}

	protected override void UpdateData()
	{
		base.UpdateData();

		Managers.Event.TriggerEvent(EEventType.OnCurrencyChanged);
	}

	#region Contents

	public void AddAmount(ECurrency currencyType, int amount)
	{
		CurrencyDict[currencyType] += amount;

		UpdateData();
	}

	public void RemoveAmount(ECurrency currencyType, int amount)
	{
		CurrencyDict[currencyType] = CurrencyDict[currencyType] >= amount ? CurrencyDict[currencyType] - amount : 0;

        if (currencyType == ECurrency.Gold)
        {
            Managers.Backend.GameData.Mission.AddNormalMissionPoint(EMissionGoal.ConsumGold, amount);
        }

        UpdateData();
	}

	#endregion
}
