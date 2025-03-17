using BackEnd;
using LitJson;
using Spine;
using System.Collections.Generic;
using System.Linq;
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
		DefaultSkillInfo = new SkillInfo(100001, EOwningState.Owned, 1, true, 0);

		SkillInfoDict.Clear();
		foreach (SkillData skillData in Managers.Backend.Chart.SkillChart.HeroSkillDataDict.Values)
		{
			if (skillData.SkillType == ESkillType.NormalMelee || skillData.SkillType == ESkillType.NormalRange)
			{
				continue;
			}

			SkillInfo skillInfo = new SkillInfo(skillData.TemplateID, EOwningState.Unowned, 1, false, 0);
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
		int skillTemplateID = 0;
		if (!SkillSlotDict.TryGetValue(skillSlot, out skillTemplateID))
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

		if (skillTemplateID == 0)
		{
			SkillInfoDict[templateID].SetIsEquipped(true);
			SkillSlotDict[skillSlot] = templateID;
		}
		else
		{
			SkillInfoDict[skillTemplateID].SetIsEquipped(false);

			if (templateID != 0)
			{
				SkillInfoDict[templateID].SetIsEquipped(true);
			}

			SkillSlotDict[skillSlot] = templateID;
		}

		Managers.Game.UpdateSkillSlot();

		UpdateData();
	}

	public void EquipRecommendedSkill()
	{
		foreach (var slot in SkillSlotDict)
		{
			if (slot.Value == 0)
			{
				continue;
			}

			SkillInfoDict[slot.Value].SetIsEquipped(false);
		}

		SkillSlotDict.Clear();
		SkillSlotDict.Add(ESkillSlot.SkillSlot01, 0);
		SkillSlotDict.Add(ESkillSlot.SkillSlot02, 0);
		SkillSlotDict.Add(ESkillSlot.SkillSlot03, 0);
		SkillSlotDict.Add(ESkillSlot.SkillSlot04, 0);
		SkillSlotDict.Add(ESkillSlot.SkillSlot05, 0);

		int stack = 0;
		int index = 0;
		foreach (var skillInfo in SkillInfoDict.OrderByDescending(kvp => kvp.Key))
		{
			if (stack >= 5)
			{
				break;
			}

			if (skillInfo.Value.OwningState == EOwningState.Owned)
			{
				SkillInfoDict[skillInfo.Value.TemplateID].SetIsEquipped(true);
				SkillSlotDict[(ESkillSlot)index] = skillInfo.Value.TemplateID;
				stack++;
				index++;
			}
		}

		Managers.Game.UpdateSkillSlot();

		UpdateData();
	}

	public void LevelUpSkills()
	{
		foreach (SkillInfo skillInfo in SkillInfoDict.Values)
		{
			if (skillInfo.CanLevelUp)
			{ 
				skillInfo.LevelUp();
			}
		}

		UpdateData();
	}

	public List<int> DrawSkill()
	{
		List<int> drawSkillList = new List<int>();
		for (int count = 0; count < 10; count++)
		{
			int randomValue = Util.GetRandomInt(0, SkillInfoDict.Count - 1);
			int key = SkillInfoDict.Keys.ElementAt(randomValue);
			drawSkillList.Add(key);
			if (SkillInfoDict[key].OwningState == EOwningState.Unowned)
			{
				SkillInfoDict[key].SetOwningState(EOwningState.Owned);
			}
			else if(SkillInfoDict[key].OwningState == EOwningState.Owned)
			{
				SkillInfoDict[key].AddPieceCount(10);
			}
		}

		UpdateData();

		return drawSkillList;
	}

	#endregion
}
