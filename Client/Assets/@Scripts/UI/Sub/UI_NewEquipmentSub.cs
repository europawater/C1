using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_NewEquipmentSub : UI_Sub
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

	private enum Buttons
	{
		Button_Dismantle,
		Button_Equip,
	}

	private Equipment _oldEquipment;
	private Equipment _newEquipment;
	private UI_EquipmentSlot _equipmentSlot;
	private List<UI_EquipmentStatSlot> _statSlotList = new List<UI_EquipmentStatSlot>();

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));
		BindButtons(typeof(Buttons));

		// Init
		_equipmentSlot = GetGameObject((int)GameObjects.UI_EquipmentSlot).GetComponent<UI_EquipmentSlot>();

		_statSlotList.Clear();
		foreach (Transform child in GetGameObject((int)GameObjects.StatArea).transform)
		{
			UI_EquipmentStatSlot statSlot = child.GetComponent<UI_EquipmentStatSlot>();
			if (statSlot != null)
			{
				_statSlotList.Add(statSlot);
			}
		}

		// Bind Event
		GetButton((int)Buttons.Button_Dismantle).gameObject.BindEvent(OnClickDismantle);
		GetButton((int)Buttons.Button_Equip).gameObject.BindEvent(OnClickEquip);
	}

	public void SetInfo(Equipment oldEquipment, Equipment newEquipment)
	{
		_oldEquipment = oldEquipment;
		_newEquipment = newEquipment;

		RefreshUI();
	}

	private void RefreshUI()
	{
		GetText((int)Texts.Text_GradeAndName).text = $"{_newEquipment.EquipmentData.Remark}";

		_equipmentSlot.SetInfo(UI_EquipmentSlot.EquipmentSlotToUse.EquipmentPopup, _newEquipment);

		int index = 0;
		foreach (var validEquipmentValue in _newEquipment.ValidEquipmentValueDict)
		{
			float compareValue = _oldEquipment == null ? validEquipmentValue.Value : _oldEquipment.ValidEquipmentValueDict[validEquipmentValue.Key];
			_statSlotList[index].SetInfo(validEquipmentValue.Key, validEquipmentValue.Value, compareValue);
			index++;
		}
	}

	#region UI Event

	private void OnClickDismantle(PointerEventData data)
	{
		Managers.Game.HandleRemoveNewEquipment();
	}

	private void OnClickEquip(PointerEventData data)
	{
		Managers.Game.HandleEquipNewEquipment();
	}

	#endregion
}
