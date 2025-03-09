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

		return param;
	}

	protected override void InitializeData()
	{
		CurrencyDict.Clear();
		CurrencyDict.Add(ECurrency.Gold, 10000);
		CurrencyDict.Add(ECurrency.Diamond, 0);
	}

	protected override void SetServerDataToLocal(JsonData gameDataJson)
	{
		int goldAmount = int.Parse(gameDataJson["Gold"].ToString());
		int diamondAmount = int.Parse(gameDataJson["Diamond"].ToString());

		CurrencyDict.Clear();
		CurrencyDict.Add(ECurrency.Gold, goldAmount);
		CurrencyDict.Add(ECurrency.Diamond, diamondAmount);
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

		UpdateData();
	}

	#endregion
}
