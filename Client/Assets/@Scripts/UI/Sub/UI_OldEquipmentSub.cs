using System.Collections.Generic;
using UnityEngine;

public class UI_OldEquipmentSub : UI_Sub
{
	private enum GameObjects
	{
		UI_EquipmentSlot,
		StatArea,
	}

	private enum Texts
	{ 
		Text_GradeAndName,
	}

	private Equipment _equipment;
	private UI_EquipmentSlot _equipmentSlot;
	private List<UI_EquipmentStatSlot> _statSlotList = new List<UI_EquipmentStatSlot>();

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));

		// Init
		_equipmentSlot = GetGameObject((int)GameObjects.UI_EquipmentSlot).GetComponent<UI_EquipmentSlot>();
		
		_statSlotList.Clear();
		foreach(Transform child in GetGameObject((int)GameObjects.StatArea).transform)
		{
			UI_EquipmentStatSlot statSlot = child.GetComponent<UI_EquipmentStatSlot>();
			if (statSlot != null)
			{
				_statSlotList.Add(statSlot);
			}
		}
	}

	public void SetInfo(Equipment equipment)
	{
		_equipment = equipment;

		RefreshUI();
	}

	private void RefreshUI()
	{
		GetText((int)Texts.Text_GradeAndName).text = $"{_equipment.EquipmentData.Remark}";

		_equipmentSlot.SetInfo(UI_EquipmentSlot.EquipmentSlotToUse.EquipmentPopup, _equipment);

		int index = 0;
		foreach(var validEquipmentValue in _equipment.ValidEquipmentValueDict)
		{
			_statSlotList[index].SetInfo(validEquipmentValue.Key, validEquipmentValue.Value, validEquipmentValue.Value);
			index++;
		}
	}
}
