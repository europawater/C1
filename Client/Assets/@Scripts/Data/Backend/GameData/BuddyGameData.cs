using BackEnd;
using LitJson;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class BuddyGameData : BaseGameData
{
	public Dictionary<int, BuddyInfo> BuddyInfoDict { get; private set; } = new Dictionary<int, BuddyInfo>();
	public Dictionary<EBuddySlot, int> BuddySlotDict { get; private set; } = new Dictionary<EBuddySlot, int>();

	public override string GetTableName()
	{
		return "Buddy";
	}

	public override Param GetParam()
	{
		Param param = new Param();

		param.Add("BuddyInfoDict", BuddyInfoDict);
		param.Add("BuddySlotDict", BuddySlotDict);

		return param;
	}

	protected override void InitializeData()
	{
		BuddyInfoDict.Clear();
		foreach (BuddyData buddyData in Managers.Backend.Chart.BuddyChart.DataDict.Values)
		{
			BuddyInfo buddyInfo = new BuddyInfo(buddyData.TemplateID, EOwningState.Unowned, 1, false, 0);
			BuddyInfoDict.Add(buddyData.TemplateID, buddyInfo);
		}

		BuddySlotDict.Clear();
		BuddySlotDict.Add(EBuddySlot.BuddySlot01, 0);
		BuddySlotDict.Add(EBuddySlot.BuddySlot02, 0);
	}

	protected override void SetServerDataToLocal(JsonData gameDataJson)
	{
		BuddyInfoDict.Clear();
		foreach (JsonData key in gameDataJson["BuddyInfoDict"].Keys)
		{
			JsonData jsonData = gameDataJson["BuddyInfoDict"][key.ToString()];
			BuddyInfo buddyInfo = new BuddyInfo(jsonData);
			BuddyInfoDict.Add(int.Parse(key.ToString()), buddyInfo);
		}

		BuddySlotDict.Clear();
		BuddySlotDict.Add(EBuddySlot.BuddySlot01, (int)gameDataJson["BuddySlotDict"]["BuddySlot01"]);
		BuddySlotDict.Add(EBuddySlot.BuddySlot02, (int)gameDataJson["BuddySlotDict"]["BuddySlot02"]);
	}

	protected override void UpdateData()
	{
		base.UpdateData();

		Managers.Event.TriggerEvent(EEventType.OnBuddyChanged);
	}

	#region Contents

	public void EquipBuddy(EBuddySlot buddySlot, int templateID)
	{
		int buddyTemplateID = 0;
		if (!BuddySlotDict.TryGetValue(buddySlot, out buddyTemplateID))
		{
			return;
		}

		for (int SlodIndex = 0; SlodIndex < BuddySlotDict.Count; SlodIndex++)
		{
			if (templateID != 0 && BuddySlotDict[(EBuddySlot)SlodIndex] == templateID)
			{
				BuddyInfoDict[templateID].SetIsEquipped(false);
				BuddySlotDict[(EBuddySlot)SlodIndex] = 0;
			}
		}

		if (buddyTemplateID == 0)
		{
			BuddyInfoDict[templateID].SetIsEquipped(true);
			BuddySlotDict[buddySlot] = templateID;
		}
		else
		{
			BuddyInfoDict[buddyTemplateID].SetIsEquipped(false);

			if (templateID != 0)
			{
				BuddyInfoDict[templateID].SetIsEquipped(true);
			}

			BuddySlotDict[buddySlot] = templateID;
		}

		Managers.Game.UpdateBuddySlot();

		UpdateData();
	}

	public void UnEquipAllBuddy()
	{
		foreach (var slot in BuddySlotDict)
		{
			if(slot.Value == 0)
			{
				continue;
			}

			BuddyInfoDict[slot.Value].SetIsEquipped(false);
		}

		BuddySlotDict.Clear();
		BuddySlotDict.Add(EBuddySlot.BuddySlot01, 0);
		BuddySlotDict.Add(EBuddySlot.BuddySlot02, 0);

		Managers.Game.UpdateBuddySlot();

		UpdateData();
	}

	public void LevelUpBuddy(int templateID)
	{
		BuddyInfo buddyInfo = BuddyInfoDict[templateID];
		if (buddyInfo.CanLevelUp)
		{
			buddyInfo.LevelUp();
		}

		UpdateData();
	}

	public void LevelUpBuddys()
	{ 
		foreach(BuddyInfo buddyInfo in BuddyInfoDict.Values)
		{
			if (buddyInfo.CanLevelUp)
			{
				buddyInfo.LevelUp();
			}
		}

		UpdateData();
	}

	public List<int> DrawBuddy()
	{
		List<int> drawBuddyList = new List<int>();
		for (int count = 0; count < 10; count++)
		{
			int randomValue = Util.GetRandomInt(0, BuddyInfoDict.Count - 1);
			int key = BuddyInfoDict.Keys.ElementAt(randomValue);
			drawBuddyList.Add(key);
			if (BuddyInfoDict[key].OwningState == EOwningState.Unowned)
			{
				BuddyInfoDict[key].SetOwningState(EOwningState.Owned);
			}
			else if (BuddyInfoDict[key].OwningState == EOwningState.Owned)
			{
				BuddyInfoDict[key].AddPieceCount(10);
			}
		}

		UpdateData();

		return drawBuddyList;
	}

	#endregion
}
