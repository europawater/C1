using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

	private enum Images
	{
		Image_ShieldIcon1,
		Image_ShieldIcon2,
		Image_ShieldIcon3,
	}

	private enum Buttons
	{
		Button_Dismantle,
		Button_Equip,
	}

	private Equipment _oldEquipment;
	private UI_EquipmentSlot _equipmentSlot;
	private List<UI_EquipmentStatSlot> _statSlotList = new List<UI_EquipmentStatSlot>();
	private List<Image> _shieldImageList = new List<Image>();

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));
		BindImages(typeof(Images));
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

		_shieldImageList.Clear();
		_shieldImageList.Add(GetImage((int)Images.Image_ShieldIcon1));
		_shieldImageList.Add(GetImage((int)Images.Image_ShieldIcon2));
		_shieldImageList.Add(GetImage((int)Images.Image_ShieldIcon3));

		// Bind Event
		GetButton((int)Buttons.Button_Dismantle).gameObject.BindEvent(OnClickDismantle);
		GetButton((int)Buttons.Button_Equip).gameObject.BindEvent(OnClickEquip);
	}

	public void SetInfo(Equipment oldEquipment)
	{
		_oldEquipment = oldEquipment;

		RefreshUI();
	}

	private void RefreshUI()
	{
		GetText((int)Texts.Text_GradeAndName).text = $"{Managers.Game.NewEquipment.EquipmentData.Remark}";

		_equipmentSlot.SetInfo(UI_EquipmentSlot.EquipmentSlotToUse.EquipmentPopup, Managers.Game.NewEquipment);

		int index = 0;
		foreach (var validEquipmentValue in Managers.Game.NewEquipment.ValidEquipmentValueDict)
		{
			float compareValue = _oldEquipment == null ? validEquipmentValue.Value : _oldEquipment.ValidEquipmentValueDict[validEquipmentValue.Key];
			_statSlotList[index].SetInfo(validEquipmentValue.Key, validEquipmentValue.Value, compareValue);
			index++;
		}

		for (int count = 0; count < _shieldImageList.Count; count++)
		{
			if (count < Managers.Game.NewEquipment.EnchantSafe)
			{
				_shieldImageList[count].color = Color.white;
			}
			else
			{
				_shieldImageList[count].color = Color.gray;
			}
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
