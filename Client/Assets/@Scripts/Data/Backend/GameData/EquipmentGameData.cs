using BackEnd;
using LitJson;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EquipmentGameData : BaseGameData
{
	public Dictionary<EEquipmentPart, EquipmentInfo> EquipmentInfoDict { get; private set; } = new Dictionary<EEquipmentPart, EquipmentInfo>();

	public override string GetTableName()
	{
		return "Equipment";
	}

	public override Param GetParam()
	{
		Param param = new Param();

		param.Add("EquipmentInfoDict", EquipmentInfoDict);

		return param;
	}

	protected override void InitializeData()
	{
		// 아이템이 없을 경우라도 Data형식을 위해서 형식에 맞게 넣어주어야합니다.
		EquipmentInfo emptyInfo = new EquipmentInfo(0, 0, 0);

		EquipmentInfoDict.Clear();
		EquipmentInfoDict.Add(EEquipmentPart.Weapon, emptyInfo);
		EquipmentInfoDict.Add(EEquipmentPart.Helmet, emptyInfo);
		EquipmentInfoDict.Add(EEquipmentPart.Armor, emptyInfo);
		EquipmentInfoDict.Add(EEquipmentPart.Pants, emptyInfo);
		EquipmentInfoDict.Add(EEquipmentPart.Pauldrons, emptyInfo);
		EquipmentInfoDict.Add(EEquipmentPart.Gloves, emptyInfo);
		EquipmentInfoDict.Add(EEquipmentPart.Necklace, emptyInfo);
		EquipmentInfoDict.Add(EEquipmentPart.Ring, emptyInfo);

		Managers.Game.EquipmentInit();
	}

	protected override void SetServerDataToLocal(JsonData gameDataJson)
	{
		EquipmentInfoDict.Clear();
		EquipmentInfoDict.Add(EEquipmentPart.Weapon, new EquipmentInfo(gameDataJson["EquipmentInfoDict"]["Weapon"]));
		EquipmentInfoDict.Add(EEquipmentPart.Helmet, new EquipmentInfo(gameDataJson["EquipmentInfoDict"]["Helmet"]));
		EquipmentInfoDict.Add(EEquipmentPart.Armor, new EquipmentInfo(gameDataJson["EquipmentInfoDict"]["Armor"]));
		EquipmentInfoDict.Add(EEquipmentPart.Pants, new EquipmentInfo(gameDataJson["EquipmentInfoDict"]["Pants"]));
		EquipmentInfoDict.Add(EEquipmentPart.Pauldrons, new EquipmentInfo(gameDataJson["EquipmentInfoDict"]["Pauldrons"]));
		EquipmentInfoDict.Add(EEquipmentPart.Gloves, new EquipmentInfo(gameDataJson["EquipmentInfoDict"]["Gloves"]));
		EquipmentInfoDict.Add(EEquipmentPart.Necklace, new EquipmentInfo(gameDataJson["EquipmentInfoDict"]["Necklace"]));
		EquipmentInfoDict.Add(EEquipmentPart.Ring, new EquipmentInfo(gameDataJson["EquipmentInfoDict"]["Ring"]));

		Managers.Game.EquipmentInit();
	}

	protected override void UpdateData()
	{
		base.UpdateData();

		Managers.Event.TriggerEvent(EEventType.OnEquipmentChanged);
	}

	#region Contents

	public void EquipEquipment(Equipment equipment)
	{
		if(equipment == null)
		{
			UpdateData();

			return;
		}

		EquipmentInfoDict[equipment.EquipmentData.EquipmentPart] = equipment.EquipmentInfo;

		UpdateData();
	}

	public void EnchantEquipment(Equipment equipment)
	{
		if (equipment == null)
		{
			UpdateData();
	
			return;
		}

		equipment.EquipmentInfo.AddEnchantLevel(1);
		equipment.UpdateStat();

		UpdateData();
	}

	public void FailEnchant(Equipment equipment)
	{
		if (equipment == null)
		{
			UpdateData();

			return;
		}

		equipment.EquipmentInfo.SubEnchantSafe(1);

		UpdateData();
	}

	#endregion
}
