using BackEnd;
using LitJson;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SkillGameData : BaseGameData
{
	public SkillInfo DefaultSkillInfo { get; private set; }
	public Dictionary<int, SkillInfo> SkillInfoDict { get; private set; } = new Dictionary<int, SkillInfo>();
	public Dictionary<ESkillSlot, int> SkillSlotDict { get; private set; } = new Dictionary<ESkillSlot, int>();

	public override string GetTableName()
	{
		return "Skill";
	}

	public override Param GetParam()
	{
		Param param = new Param();

		param.Add("DefaultSkillInfo", DefaultSkillInfo);
		param.Add("SkillInfoDict", SkillInfoDict);
		param.Add("SkillSlotDict", SkillSlotDict);

		return param;
	}

	protected override void InitializeData()
	{
		DefaultSkillInfo = new SkillInfo(100001, EOwningState.Owned, 1, true);

		SkillInfoDict.Clear();
		foreach (SkillData skillData in Managers.Backend.Chart.SkillChart.HeroSkillDataDict.Values)
		{
			if (skillData.SkillType == ESkillType.NormalMelee || skillData.SkillType == ESkillType.NormalRange)
			{
				continue;
			}

			SkillInfo skillInfo = new SkillInfo(skillData.TemplateID, EOwningState.Unowned, 1, false);
			SkillInfoDict.Add(skillData.TemplateID, skillInfo);
		}

		SkillSlotDict.Clear();
		SkillSlotDict.Add(ESkillSlot.SkillSlot01, 0);
		SkillSlotDict.Add(ESkillSlot.SkillSlot02, 0);
		SkillSlotDict.Add(ESkillSlot.SkillSlot03, 0);
		SkillSlotDict.Add(ESkillSlot.SkillSlot04, 0);
		SkillSlotDict.Add(ESkillSlot.SkillSlot05, 0);
	}

	protected override void SetServerDataToLocal(JsonData gameDataJson)
	{
		DefaultSkillInfo = new SkillInfo(gameDataJson["DefaultSkillInfo"]);

		SkillInfoDict.Clear();
		foreach (JsonData key in gameDataJson["SkillInfoDict"].Keys)
		{
			JsonData jsonData = gameDataJson["SkillInfoDict"][key.ToString()];
			SkillInfo skillInfo = new SkillInfo(jsonData);
			SkillInfoDict.Add(skillInfo.TemplateID, skillInfo);
		}

		SkillSlotDict.Clear();
		SkillSlotDict.Add(ESkillSlot.SkillSlot01, (int)gameDataJson["SkillSlotDict"]["SkillSlot01"]);
		SkillSlotDict.Add(ESkillSlot.SkillSlot02, (int)gameDataJson["SkillSlotDict"]["SkillSlot02"]);
		SkillSlotDict.Add(ESkillSlot.SkillSlot03, (int)gameDataJson["SkillSlotDict"]["SkillSlot03"]);
		SkillSlotDict.Add(ESkillSlot.SkillSlot04, (int)gameDataJson["SkillSlotDict"]["SkillSlot04"]);
		SkillSlotDict.Add(ESkillSlot.SkillSlot05, (int)gameDataJson["SkillSlotDict"]["SkillSlot05"]);
	}

	protected override void UpdateData()
	{
		base.UpdateData();

		Managers.Event.TriggerEvent(EEventType.OnSkillChanged);
	}

	#region Contents

	public void EquipSkill(ESkillSlot skillSlot, int templateID)
	{
		int skillTemplatedID = 0;
		if (!SkillSlotDict.TryGetValue(skillSlot, out skillTemplatedID))
		{
			return;
		}

		for (int slotIndex = 0; slotIndex < SkillSlotDict.Count; slotIndex++)
		{
			if (templateID != 0 && SkillSlotDict[(ESkillSlot)slotIndex] == templateID)
			{
				SkillInfoDict[templateID].SetIsEquipped(false);
				SkillSlotDict[(ESkillSlot)slotIndex] = 0;
			}
		}

		if (skillTemplatedID == 0)
		{
			SkillInfoDict[templateID].SetIsEquipped(true);
			SkillSlotDict[skillSlot] = templateID;
		}
		else
		{
			SkillInfoDict[skillTemplatedID].SetIsEquipped(false);

			if (templateID != 0)
			{
				SkillInfoDict[templateID].SetIsEquipped(true);
			}

			SkillSlotDict[skillSlot] = templateID;
		}

		Managers.Game.UpdateSkillSlot();

		UpdateData();
	}

	#endregion
}
